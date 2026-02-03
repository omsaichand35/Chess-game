using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Chess
{
    public class StockfishEngine : IDisposable
    {
        private readonly Process process;
        private readonly StreamWriter input;
        private readonly StreamReader output;

        public StockfishEngine(string pathToEngine)
        {
            process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pathToEngine,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            input = process.StandardInput;
            output = process.StandardOutput;

            SendCommand("uci");
            WaitFor("uciok");
            
            // Enable WDL (Win/Draw/Loss) statistics
            SendCommand("setoption name UCI_ShowWDL value true");
            WaitForReady();
        }

        public void SetPosition(string fen)
        {
            SendCommand($"position fen {fen}");
        }

        public void SetSkillLevel(int level)
        {
            SendCommand($"setoption name Skill Level value {level}");
            WaitForReady();
        }

        public void SetEloLimit(int elo)
        {
            SendCommand("setoption name UCI_LimitStrength value true");
            SendCommand($"setoption name UCI_Elo value {elo}");
            WaitForReady();
        }

        public void DisableEloLimit()
        {
            SendCommand("setoption name UCI_LimitStrength value false");
            WaitForReady();
        }

        public string GetBestMove(int depth = 10)
        {
            SendCommand($"go depth {depth}");
            var timeout = DateTime.Now.AddSeconds(30);
            
            while(DateTime.Now < timeout)
            {
                var line = output.ReadLine();
                if (line == null) continue;
                if (line.StartsWith("bestmove"))
                {
                    return line.Split(' ')[1];
                }
            }
            
            throw new TimeoutException("Stockfish did not respond within 30 seconds");
        }

        public (string move, int win, int draw, int loss) GetBestMoveWithWDL(int depth)
        {
            SendCommand($"go depth {depth}");

            int w = 0, d = 0, l = 0;
            var timeout = DateTime.Now.AddSeconds(30);

            while (DateTime.Now < timeout)
            {
                var line = output.ReadLine();
                if (line == null) 
                {
                    System.Threading.Thread.Sleep(10);
                    continue;
                }

                if (line.Contains(" wdl "))
                {
                    var parts = line.Split("wdl")[1].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3)
                    {
                        int.TryParse(parts[0], out w);
                        int.TryParse(parts[1], out d);
                        int.TryParse(parts[2], out l);
                    }
                }

                if (line.StartsWith("bestmove"))
                {
                    var move = line.Split(' ')[1];
                    return (move, w, d, l);
                }
            }
            
            throw new TimeoutException("Stockfish did not respond within 30 seconds");
        }

        private void SendCommand(string cmd)
        {
            input.WriteLine(cmd);
            input.Flush();
        }

        private void WaitFor(string expected)
        {
            var timeout = DateTime.Now.AddSeconds(10);
            while (DateTime.Now < timeout)
            {
                var line = output.ReadLine();
                if (line != null && line.Contains(expected))
                    break;
            }
        }

        private void WaitForReady()
        {
            SendCommand("isready");
            WaitFor("readyok");
        }

        public void Dispose()
        {
            try
            {
                SendCommand("quit");
                if (!process.WaitForExit(1000))
                {
                    process.Kill();
                }
                process.Dispose();
            }
            catch { }
        }
    }
}
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Chess
{
    public class OnlineGameManager : IDisposable
    {
        private TcpListener? listener;
        private TcpClient? client;
        private StreamReader? reader;
        private StreamWriter? writer;
        private CancellationTokenSource? cancellationTokenSource;
        private bool isDisposed;

        public event Action<string>? MoveReceived;
        public event Action<string>? StatusChanged;

        public bool IsHost { get; private set; }

        public async Task HostAsync(int port)
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                IsHost = true;
                StatusChanged?.Invoke($"Waiting for opponent on port {port}...");

                client = await listener.AcceptTcpClientAsync();
                if (isDisposed) return;

                SetupStreams();
                StatusChanged?.Invoke("Opponent connected.");
                StartReceiveLoop();
            }
            catch (ObjectDisposedException)
            {
                // Expected when shutting down
            }
            catch (SocketException ex)
            {
                if (!isDisposed)
                    StatusChanged?.Invoke($"Socket error: {ex.Message}");
            }
        }

        public async Task JoinAsync(string host, int port)
        {
            try
            {
                client = new TcpClient();
                IsHost = false;
                StatusChanged?.Invoke("Connecting...");

                await client.ConnectAsync(host, port);
                if (isDisposed) return;

                SetupStreams();
                StatusChanged?.Invoke("Connected to host.");
                StartReceiveLoop();
            }
            catch (ObjectDisposedException)
            {
                // Expected when shutting down
            }
            catch (SocketException ex)
            {
                if (!isDisposed)
                    StatusChanged?.Invoke($"Socket error: {ex.Message}");
            }
        }

        public Task SendMoveAsync(string move)
        {
            if (writer == null)
                return Task.CompletedTask;

            return writer.WriteLineAsync($"move {move}");
        }

        private void SetupStreams()
        {
            if (client == null)
                return;

            var stream = client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream) { AutoFlush = true };
        }

        private void StartReceiveLoop()
        {
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            _ = Task.Run(async () =>
            {
                try
                {
                    while (!token.IsCancellationRequested && reader != null)
                    {
                        var line = await reader.ReadLineAsync();
                        if (line == null)
                            break;

                        if (line.StartsWith("move "))
                        {
                            var move = line.Substring(5).Trim();
                            if (!string.IsNullOrWhiteSpace(move))
                                MoveReceived?.Invoke(move);
                        }
                    }
                }
                catch (IOException) when (isDisposed || token.IsCancellationRequested)
                {
                    // Ignore expected shutdown exceptions
                }
                catch (ObjectDisposedException)
                {
                    // Ignore expected shutdown exceptions
                }
                catch (Exception ex)
                {
                    if (!isDisposed)
                        StatusChanged?.Invoke($"Connection lost: {ex.Message}");
                }
            }, token);
        }

        public void Dispose()
        {
            if (isDisposed) return;
            isDisposed = true;

            try
            {
                cancellationTokenSource?.Cancel();
                writer?.Dispose();
                reader?.Dispose();
                client?.Close();
                listener?.Stop();
            }
            catch
            {
            }
        }
    }
}

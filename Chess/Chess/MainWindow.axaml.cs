using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Chess
{
    public partial class MainWindow : Window
    {
        private readonly Dictionary<string, Bitmap> pieceBitmaps = new();
        private Piece?[,] board = new Piece?[8, 8];
        private int selectedRow = -1;
        private int selectedCol = -1;
        private PieceColor currentTurn = PieceColor.White;
        private bool gameStarted = false;
        private bool whiteKingMoved;
        private bool blackKingMoved;
        private bool whiteRookA_Moved;
        private bool whiteRookH_Moved;
        private bool blackRookA_Moved;
        private bool blackRookH_Moved;
        private Move? lastMove;
        private HashSet<(int Row, int Col)> legalMoveHighlights = new();
        private List<Piece> whiteCapturedPieces = new();
        private List<Piece> blackCapturedPieces = new();
        private readonly Dictionary<string, int> repetitionCounts = new();

        // New fields for game modes and board orientation
        private GameMode currentGameMode = GameMode.None;
        private PieceColor playerColor = PieceColor.White;
        private bool boardFlipped = false;
        private bool waitingForColorSelection = false;
        private bool waitingForOnlineSetup = false;
        private CancellationTokenSource? aiVsAiCancellation;
        private string aiWhiteName = "Alpha";
        private string aiBlackName = "Beta";
        private string aiWhiteMood = "Calm";
        private string aiBlackMood = "Calm";
        private DifficultyLevel? aiWhiteLevel;
        private DifficultyLevel? aiBlackLevel;

        // Online multiplayer fields
        private OnlineGameManager? onlineManager;
        private bool isOnlineGame = false;

        public MainWindow()
        {
            InitializeComponent();
            ChessBoard.IsVisible = false;
            BoardArea.IsVisible = false;
            ResetWdlDisplay();
        }

        private void StartGameClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // Don't start if waiting for color selection or online setup
            if (waitingForColorSelection || waitingForOnlineSetup) return;
            if (currentGameMode == GameMode.AiVsAi) return;

            // Ensure Game UI is visible
            if (HomeView != null) HomeView.IsVisible = false;
            if (GameUI != null) GameUI.IsVisible = true;
            if (ChessBoard != null) ChessBoard.IsVisible = true;
            if (BoardArea != null) BoardArea.IsVisible = true;

            gameStarted = true;
            InitializeBoard();
            DrawBoard();
            UpdateTitle();

            // If playing as black against computer, let computer make first move
            if (currentGameMode == GameMode.PlayComputer && playerColor == PieceColor.Black)
            {
                MakeComputerMove();
            }
        }

        private void PlayComputerClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            currentGameMode = GameMode.PlayComputer;
            ShowColorSelectionDialog();
        }

        private void PlayFriendClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            currentGameMode = GameMode.PlayFriend;
            ShowColorSelectionDialog();
        }

        private void PlayOnlineClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            currentGameMode = GameMode.Online;
            ShowOnlineSetupDialog();
        }

        private void PlayAiVsAiClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            currentGameMode = GameMode.AiVsAi;
            if (AiVsAiSetupDialog != null)
            {
                AiVsAiSetupDialog.IsVisible = true;
            }
        }

        private void ShowColorSelectionDialog()
        {
            waitingForColorSelection = true;
            if (ColorSelectionDialog != null)
            {
                ColorSelectionDialog.IsVisible = true;
            }

            // Update button text based on game mode
            if (currentGameMode == GameMode.PlayFriend)
            {
                // For friend mode, change the text
                if (SelectBlackButton?.Content is StackPanel blackPanel)
                {
                    var textBlocks = blackPanel.Children.OfType<StackPanel>().FirstOrDefault()?.Children.OfType<TextBlock>();
                    if (textBlocks != null)
                    {
                        var subtitle = textBlocks.Skip(1).FirstOrDefault();
                        if (subtitle != null) subtitle.Text = "Player 2 moves first";
                    }
                }
            }
        }

        private void SelectWhiteColor(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            playerColor = PieceColor.White;
            boardFlipped = false;
            HideColorSelectionAndStartGame();
        }

        private void SelectBlackColor(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            playerColor = PieceColor.Black;
            boardFlipped = true;
            HideColorSelectionAndStartGame();
        }

        private void SelectRandomColor(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var random = new Random();
            playerColor = random.Next(2) == 0 ? PieceColor.White : PieceColor.Black;
            boardFlipped = playerColor == PieceColor.Black;
            HideColorSelectionAndStartGame();
        }

        private void CancelColorSelection(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            waitingForColorSelection = false;
            currentGameMode = GameMode.None;
            if (ColorSelectionDialog != null)
            {
                ColorSelectionDialog.IsVisible = false;
            }
        }

        private void HideColorSelectionAndStartGame()
        {
            waitingForColorSelection = false;
            if (ColorSelectionDialog != null)
            {
                ColorSelectionDialog.IsVisible = false;
            }

            // Switch to Game UI view
            if (HomeView != null) HomeView.IsVisible = false;
            if (GameUI != null) GameUI.IsVisible = true;
            if (ChessBoard != null) ChessBoard.IsVisible = true;
            if (BoardArea != null) BoardArea.IsVisible = true;

            // Initialize the game but don't start yet (they still need to click Start Game button)
            InitializeBoard();
            DrawBoard();
        }

        private void NewGameClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // This button is in the sidebar - show color selection for computer play
            currentGameMode = GameMode.PlayComputer;
            ShowColorSelectionDialog();
        }

        private void HomeClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            gameStarted = false;
            aiVsAiCancellation?.Cancel();
            // logic to stop engine if running...

            CleanupOnlineGame();

            if (HomeView != null) HomeView.IsVisible = true;
            if (GameUI != null) GameUI.IsVisible = false;
            if (ChessBoard != null) ChessBoard.IsVisible = false;
            if (BoardArea != null) BoardArea.IsVisible = false;

            Title = "Chess";
        }

        private void UpdateTitle()
        {
            Title = $"Chess - {GetSideName(currentTurn)} to move";
        }

        private string GetSideName(PieceColor color)
        {
            if (currentGameMode == GameMode.AiVsAi)
                return color == PieceColor.White ? aiWhiteName : aiBlackName;
            return color.ToString();
        }

        private string GetPositionKey()
        {
            return $"{ToFEN(currentTurn)}|{whiteKingMoved}{whiteRookA_Moved}{whiteRookH_Moved}{blackKingMoved}{blackRookA_Moved}{blackRookH_Moved}";
        }

        private bool CheckThreefoldRepetition()
        {
            var key = GetPositionKey();
            repetitionCounts[key] = repetitionCounts.TryGetValue(key, out var count) ? count + 1 : 1;

            if (repetitionCounts[key] >= 3)
            {
                gameStarted = false;
                aiVsAiCancellation?.Cancel();
                ShowGameOver("Draw by threefold repetition!");
                return true;
            }

            return false;
        }

        // ---------------- DRAWING ----------------

        private void DrawBoard()
        {
            ChessBoard.Children.Clear();

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    int row = r, col = c;

                    // Flip board if needed (player is black or it's friend mode and current turn should be at bottom)
                    int displayRow = boardFlipped ? 7 - row : row;
                    int displayCol = boardFlipped ? 7 - col : col;

                    var square = new Border
                    {
                        Background = ((row + col) % 2 == 0) ? Brushes.Beige : Brushes.SaddleBrown,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1)
                    };

                    if (lastMove.HasValue &&
                        ((lastMove.Value.FromRow == row && lastMove.Value.FromCol == col) ||
                         (lastMove.Value.ToRow == row && lastMove.Value.ToCol == col)))
                    {
                        square.Background = new SolidColorBrush(Color.FromRgb(246, 246, 105));
                    }

                    if (legalMoveHighlights.Contains((row, col)))
                    {
                        square.Background = new SolidColorBrush(Color.FromRgb(186, 202, 68));
                    }

                    if (row == selectedRow && col == selectedCol)
                    {
                        square.BorderBrush = Brushes.Red;
                        square.BorderThickness = new Thickness(3);
                    }

                    var piece = board[row, col];
                    if (piece != null)
                    {
                        square.Child = new Image
                        {
                            Source = GetPieceBitmap(piece),
                            Width = 60,
                            Height = 60,
                            Stretch = Avalonia.Media.Stretch.Uniform
                        };
                    }

                    square.PointerPressed += (_, _) => OnSquareClicked(row, col);

                    Grid.SetRow(square, displayRow);
                    Grid.SetColumn(square, displayCol);
                    ChessBoard.Children.Add(square);
                }
            }
        }

        private Bitmap GetPieceBitmap(Piece piece)
        {
            var key = GetPieceAssetKey(piece);
            if (pieceBitmaps.TryGetValue(key, out var bitmap))
            {
                return bitmap;
            }

            var uri = new Uri($"avares://Chess/Assests/Pieces/{key}.png");
            using var stream = AssetLoader.Open(uri);
            bitmap = new Bitmap(stream);
            pieceBitmaps[key] = bitmap;
            return bitmap;
        }

        private static string GetPieceAssetKey(Piece piece)
        {
            char color = piece.Color == PieceColor.White ? 'w' : 'b';
            char type = piece.Type switch
            {
                PieceType.Pawn => 'p',
                PieceType.Rook => 'r',
                PieceType.Knight => 'n',
                PieceType.Bishop => 'b',
                PieceType.Queen => 'q',
                PieceType.King => 'k',
                _ => 'p'
            };

            return $"{color}{type}";
        }

        private static int GetPieceValue(PieceType type)
        {
            return type switch
            {
                PieceType.Pawn => 1,
                PieceType.Knight => 3,
                PieceType.Bishop => 3,
                PieceType.Rook => 5,
                PieceType.Queen => 9,
                _ => 0
            };
        }

        // ---------------- INPUT ----------------

        private void OnSquareClicked(int row, int col)
        {
            if (!gameStarted) return;

            if (currentGameMode == GameMode.AiVsAi)
                return;

            // In computer mode, only allow moves when it's the player's turn
            if (currentGameMode == GameMode.PlayComputer && currentTurn != playerColor)
                return;

            // In online mode, only allow moves when it's the local player's turn
            if (currentGameMode == GameMode.Online && currentTurn != playerColor)
                return;

            if (selectedRow == -1)
            {
                var piece = board[row, col];
                if (piece == null || piece.Color != currentTurn)
                    return;

                selectedRow = row;
                selectedCol = col;
                legalMoveHighlights = GetLegalMovesForSquare(row, col);
                DrawBoard();
                return;
            }

            if (board[row, col] != null && board[row, col]!.Color == currentTurn)
            {
                selectedRow = row;
                selectedCol = col;
                legalMoveHighlights = GetLegalMovesForSquare(row, col);
                DrawBoard();
                return;
            }

            if (TryMove(selectedRow, selectedCol, row, col))
            {
                selectedRow = -1;
                selectedCol = -1;
                legalMoveHighlights.Clear();
            }

            DrawBoard();
        }

        // ---------------- GAME LOGIC ----------------

        private bool TryMove(int fr, int fc, int tr, int tc)
        {
            var move = new Move
            {
                FromRow = fr,
                FromCol = fc,
                ToRow = tr,
                ToCol = tc
            };

            if (!IsPseudoLegalMove(move))
                return false;

            ApplyMove(ref move);

            if (IsKingInCheck(currentTurn))
            {
                UndoMove(ref move);
                return false;
            }

            if (IsPawnPromotion(move))
                PromotePawn(move);

            lastMove = move;

            currentTurn = currentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
            UpdateTitle();
            UpdateCapturedPiecesDisplay();

            if (CheckThreefoldRepetition())
                return true;

            // Send move to online opponent
            if (currentGameMode == GameMode.Online && onlineManager != null)
            {
                var uciMove = ToUciMove(move.FromRow, move.FromCol, move.ToRow, move.ToCol);
                _ = onlineManager.SendMoveAsync(uciMove);
            }

            // Handle friend mode - rotate board after each move
            if (currentGameMode == GameMode.PlayFriend)
            {
                boardFlipped = !boardFlipped;
            }

            // Handle computer mode - only make computer move if it's the computer's turn
            if (currentGameMode == GameMode.PlayComputer && currentTurn != playerColor)
            {
                MakeComputerMove();
            }

            if (IsCheckmate(currentTurn))
                ShowGameOver($"{currentTurn} is checkmated!");
            else if (IsStalemate(currentTurn))
                ShowGameOver("Stalemate!");

            // Check for threefold repetition draw
            CheckThreefoldRepetition();

            return true;
        }

        private void MakeComputerMove()
        {
            var selectedDifficulty = GetSelectedDifficulty();
            var depth = selectedDifficulty?.Depth ?? GetDepthFromDifficulty();
            var skill = selectedDifficulty?.Skill;
            var limitStrength = selectedDifficulty?.LimitStrength ?? false;
            var elo = selectedDifficulty?.Elo;

            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    Console.WriteLine($"Starting Stockfish engine (Depth: {depth}, Skill: {skill?.ToString() ?? "None"}, ELO: {elo?.ToString() ?? "Unlimited"})...");

                    using var engine = new StockfishEngine(
                        @"C:\Users\omsai\Downloads\stockfish-windows-x86-64-avx2\stockfish\stockfish-windows-x86-64-avx2.exe"
                    );

                    Console.WriteLine("Engine initialized successfully");

                    string fen = ToFEN(currentTurn);
                    Console.WriteLine($"Position FEN: {fen}");
                    engine.SetPosition(fen);

                    // Set ELO limit if specified
                    if (limitStrength && elo.HasValue)
                    {
                        Console.WriteLine($"Setting ELO limit to {elo.Value}");
                        engine.SetEloLimit(elo.Value);
                    }
                    else
                    {
                        // Use skill level for unlimited strength or as fallback
                        if (skill.HasValue)
                        {
                            Console.WriteLine($"Setting skill level to {skill.Value}");
                            engine.SetSkillLevel(skill.Value);
                        }
                    }

                    Console.WriteLine("Calculating best move...");
                    var result = engine.GetBestMoveWithWDL(depth);
                    Console.WriteLine($"Best move: {result.move}, WDL: {result.win}/{result.draw}/{result.loss}");

                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        UpdateWdlDisplay(result.win, result.draw, result.loss, currentTurn);
                        ApplyEngineMove(result.move);
                        currentTurn = currentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
                        UpdateTitle();
                        UpdateCapturedPiecesDisplay();
                        DrawBoard();

                        if (CheckThreefoldRepetition())
                            return;

                        if (IsCheckmate(currentTurn))
                            ShowGameOver($"{currentTurn} is checkmated!");
                        else if (IsStalemate(currentTurn))
                            ShowGameOver("Stalemate!");
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Engine error: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");

                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        ShowGameOver($"Engine error: {ex.Message}\n\nPlease check the console for details.");
                    });
                }
            });
        }

        private int GetDepthFromDifficulty()
        {
            return GetSelectedDifficulty().Depth;
        }

        private DifficultyLevel GetSelectedDifficulty()
        {
            if (DifficultyBox?.SelectedIndex is int index && index >= 0)
            {
                var level = index + 1;
                var match = DifficultySettings.Levels.Find(entry => entry.Level == level);
                if (match != null)
                    return match;
            }

            if (DifficultyBox?.SelectedItem is ComboBoxItem item && item.Tag != null)
            {
                if (int.TryParse(item.Tag.ToString(), out var level))
                {
                    var match = DifficultySettings.Levels.Find(entry => entry.Level == level);
                    if (match != null)
                        return match;
                }
            }

            if (DifficultyBox?.SelectedItem is string name)
            {
                var match = DifficultySettings.Levels.Find(entry => entry.Name == name);
                if (match != null)
                    return match;
            }

            return DifficultySettings.Levels[0];
        }

        private List<Move> GenerateLegalMoves(PieceColor color)
        {
            var legalMoves = new List<Move>();

            for (int fr = 0; fr < 8; fr++)
            {
                for (int fc = 0; fc < 8; fc++)
                {
                    var piece = board[fr, fc];
                    if (piece == null || piece.Color != color)
                        continue;

                    for (int tr = 0; tr < 8; tr++)
                    {
                        for (int tc = 0; tc < 8; tc++)
                        {
                            var move = new Move
                            {
                                FromRow = fr,
                                FromCol = fc,
                                ToRow = tr,
                                ToCol = tc
                            };

                            if (!IsPseudoLegalMove(move))
                                continue;

                            ApplyMove(ref move);
                            bool illegal = IsKingInCheck(color);
                            UndoMove(ref move);

                            if (!illegal)
                                legalMoves.Add(move);
                        }
                    }
                }
            }

            return legalMoves;
        }

        private string ToFEN(PieceColor sideToMove)
        {
            string fen = "";

            for (int r = 0; r < 8; r++)
            {
                int empty = 0;

                for (int c = 0; c < 8; c++)
                {
                    var p = board[r, c];
                    if (p == null)
                    {
                        empty++;
                        continue;
                    }

                    if (empty > 0)
                    {
                        fen += empty;
                        empty = 0;
                    }

                    char ch = p.Type switch
                    {
                        PieceType.Pawn => 'p',
                        PieceType.Rook => 'r',
                        PieceType.Knight => 'n',
                        PieceType.Bishop => 'b',
                        PieceType.Queen => 'q',
                        PieceType.King => 'k',
                        _ => '?'
                    };

                    fen += p.Color == PieceColor.White
                        ? char.ToUpper(ch)
                        : ch;
                }

                if (empty > 0)
                    fen += empty;

                if (r < 7) fen += "/";
            }

            fen += sideToMove == PieceColor.White ? " w " : " b ";
            fen += "- - 0 1";

            return fen;
        }

        private string ToUciMove(int fromRow, int fromCol, int toRow, int toCol)
        {
            char fileFrom = (char)('a' + fromCol);
            char rankFrom = (char)('8' - fromRow);
            char fileTo = (char)('a' + toCol);
            char rankTo = (char)('8' - toRow);
            return $"{fileFrom}{rankFrom}{fileTo}{rankTo}";
        }

        private void ApplyEngineMove(string move)
        {
            if (move.Length < 4) return;

            int fromCol = move[0] - 'a';
            int fromRow = 8 - (move[1] - '0');
            int toCol = move[2] - 'a';
            int toRow = 8 - (move[3] - '0');

            var engineMove = new Move
            {
                FromRow = fromRow,
                FromCol = fromCol,
                ToRow = toRow,
                ToCol = toCol
            };

            ApplyMove(ref engineMove);

            if (IsPawnPromotion(engineMove))
                PromotePawn(engineMove);

            lastMove = engineMove;
        }

        private bool IsCheckmate(PieceColor color)
        {
            return IsKingInCheck(color) && GenerateLegalMoves(color).Count == 0;
        }

        private bool IsStalemate(PieceColor color)
        {
            return !IsKingInCheck(color) && GenerateLegalMoves(color).Count == 0;
        }

        private bool IsPawnPromotion(Move move)
        {
            var piece = board[move.ToRow, move.ToCol];
            if (piece == null || piece.Type != PieceType.Pawn)
                return false;
            if (piece.Color == PieceColor.White && move.ToRow == 0)
                return true;
            if (piece.Color == PieceColor.Black && move.ToRow == 7)
                return true;
            return false;
        }

        private void PromotePawn(Move move)
        {
            var pawn = board[move.ToRow, move.ToCol];
            if (pawn == null || pawn.Type != PieceType.Pawn)
                return;
            board[move.ToRow, move.ToCol] = new Piece(PieceType.Queen, pawn.Color);
        }

        // ---------------- MOVE RULES ----------------

        private bool IsPseudoLegalMove(Move move)
        {
            if (move.ToRow < 0 || move.ToRow > 7 || move.ToCol < 0 || move.ToCol > 7)
                return false;

            var piece = board[move.FromRow, move.FromCol];
            if (piece == null)
                return false;

            var target = board[move.ToRow, move.ToCol];
            if (target != null && target.Color == piece.Color)
                return false;

            int dr = move.ToRow - move.FromRow;
            int dc = move.ToCol - move.FromCol;

            switch (piece.Type)
            {
                case PieceType.Pawn:
                    int dir = piece.Color == PieceColor.White ? -1 : 1;
                    int start = piece.Color == PieceColor.White ? 6 : 1;

                    if (dc == 0 && dr == dir && target == null)
                        return true;

                    if (dc == 0 && move.FromRow == start && dr == 2 * dir &&
                        board[move.FromRow + dir, move.FromCol] == null && target == null)
                        return true;

                    if (Math.Abs(dc) == 1 && dr == dir && target != null)
                        return true;

                    return false;

                case PieceType.Rook:
                    return StraightClear(move);

                case PieceType.Bishop:
                    return DiagonalClear(move);

                case PieceType.Queen:
                    return StraightClear(move) || DiagonalClear(move);

                case PieceType.Knight:
                    return (Math.Abs(dr) == 2 && Math.Abs(dc) == 1) ||
                           (Math.Abs(dr) == 1 && Math.Abs(dc) == 2);

                case PieceType.King:
                    if (Math.Abs(dr) <= 1 && Math.Abs(dc) <= 1)
                        return true;

                    if (dr == 0 && Math.Abs(dc) == 2 && target == null)
                        return CanCastle(piece.Color, dc > 0);

                    return false;
            }

            return false;
        }

        private bool StraightClear(Move move)
        {
            if (move.FromRow != move.ToRow && move.FromCol != move.ToCol)
                return false;

            int dr = Math.Sign(move.ToRow - move.FromRow);
            int dc = Math.Sign(move.ToCol - move.FromCol);

            int r = move.FromRow + dr;
            int c = move.FromCol + dc;

            while (r != move.ToRow || c != move.ToCol)
            {
                if (board[r, c] != null)
                    return false;
                r += dr;
                c += dc;
            }

            return true;
        }

        private bool DiagonalClear(Move move)
        {
            if (Math.Abs(move.FromRow - move.ToRow) != Math.Abs(move.FromCol - move.ToCol))
                return false;

            int dr = Math.Sign(move.ToRow - move.FromRow);
            int dc = Math.Sign(move.ToCol - move.FromCol);

            int r = move.FromRow + dr;
            int c = move.FromCol + dc;

            while (r != move.ToRow)
            {
                if (board[r, c] != null)
                    return false;
                r += dr;
                c += dc;
            }

            return true;
        }

        private bool IsKingInCheck(PieceColor color)
        {
            int kr = -1, kc = -1;

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    if (board[r, c]?.Type == PieceType.King && board[r, c]!.Color == color)
                    {
                        kr = r;
                        kc = c;
                        break;
                    }
                }

                if (kr != -1)
                    break;
            }

            if (kr == -1)
                return false;

            return IsSquareAttacked(color, kr, kc);
        }

        private bool CanCastle(PieceColor color, bool kingSide)
        {
            int row = color == PieceColor.White ? 7 : 0;

            if (color == PieceColor.White && whiteKingMoved) return false;
            if (color == PieceColor.Black && blackKingMoved) return false;

            if (kingSide)
            {
                if (color == PieceColor.White && whiteRookH_Moved) return false;
                if (color == PieceColor.Black && blackRookH_Moved) return false;

                if (board[row, 5] != null || board[row, 6] != null) return false;
                if (!IsSquareAttacked(color, row, 4) &&
                    !IsSquareAttacked(color, row, 5) &&
                    !IsSquareAttacked(color, row, 6))
                {
                    return board[row, 7]?.Type == PieceType.Rook && board[row, 7]?.Color == color;
                }
            }
            else
            {
                if (color == PieceColor.White && whiteRookA_Moved) return false;
                if (color == PieceColor.Black && blackRookA_Moved) return false;

                if (board[row, 1] != null || board[row, 2] != null || board[row, 3] != null) return false;
                if (!IsSquareAttacked(color, row, 4) &&
                    !IsSquareAttacked(color, row, 3) &&
                    !IsSquareAttacked(color, row, 2))
                {
                    return board[row, 0]?.Type == PieceType.Rook && board[row, 0]?.Color == color;
                }
            }

            return false;
        }

        private bool IsSquareAttacked(PieceColor color, int row, int col)
        {
            var enemy = color == PieceColor.White ? PieceColor.Black : PieceColor.White;

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    var piece = board[r, c];
                    if (piece == null || piece.Color != enemy)
                        continue;

                    if (IsPieceAttackingSquare(piece, r, c, row, col))
                        return true;
                }
            }

            return false;
        }

        private bool IsPieceAttackingSquare(Piece piece, int fromRow, int fromCol, int toRow, int toCol)
        {
            int dr = toRow - fromRow;
            int dc = toCol - fromCol;

            switch (piece.Type)
            {
                case PieceType.Pawn:
                    int dir = piece.Color == PieceColor.White ? -1 : 1;
                    return dr == dir && Math.Abs(dc) == 1;
                case PieceType.Rook:
                    return StraightClear(new Move { FromRow = fromRow, FromCol = fromCol, ToRow = toRow, ToCol = toCol });
                case PieceType.Bishop:
                    return DiagonalClear(new Move { FromRow = fromRow, FromCol = fromCol, ToRow = toRow, ToCol = toCol });
                case PieceType.Queen:
                    return StraightClear(new Move { FromRow = fromRow, FromCol = fromCol, ToRow = toRow, ToCol = toCol }) ||
                           DiagonalClear(new Move { FromRow = fromRow, FromCol = fromCol, ToRow = toRow, ToCol = toCol });
                case PieceType.Knight:
                    return (Math.Abs(dr) == 2 && Math.Abs(dc) == 1) ||
                           (Math.Abs(dr) == 1 && Math.Abs(dc) == 2);
                case PieceType.King:
                    return Math.Abs(dr) <= 1 && Math.Abs(dc) <= 1;
            }

            return false;
        }

        // ---------------- APPLY / UNDO ----------------

        private void ApplyMove(ref Move move)
        {
            var piece = board[move.FromRow, move.FromCol];
            move.Captured = board[move.ToRow, move.ToCol];
            move.WhiteKingMovedBefore = whiteKingMoved;
            move.BlackKingMovedBefore = blackKingMoved;
            move.WhiteRookA_MovedBefore = whiteRookA_Moved;
            move.WhiteRookH_MovedBefore = whiteRookH_Moved;
            move.BlackRookA_MovedBefore = blackRookA_Moved;
            move.BlackRookH_MovedBefore = blackRookH_Moved;

            // Track captured pieces (only for actual game moves, not simulation)
            if (move.Captured != null && gameStarted)
            {
                if (move.Captured.Color == PieceColor.White)
                {
                    blackCapturedPieces.Add(move.Captured);
                }
                else
                {
                    whiteCapturedPieces.Add(move.Captured);
                }
            }

            board[move.ToRow, move.ToCol] = piece;
            board[move.FromRow, move.FromCol] = null;

            if (piece?.Type == PieceType.King)
            {
                if (piece.Color == PieceColor.White)
                    whiteKingMoved = true;
                else
                    blackKingMoved = true;

                if (Math.Abs(move.ToCol - move.FromCol) == 2)
                {
                    move.IsCastling = true;
                    int row = move.FromRow;

                    if (move.ToCol > move.FromCol)
                    {
                        move.RookFromRow = row;
                        move.RookFromCol = 7;
                        move.RookToRow = row;
                        move.RookToCol = 5;
                    }
                    else
                    {
                        move.RookFromRow = row;
                        move.RookFromCol = 0;
                        move.RookToRow = row;
                        move.RookToCol = 3;
                    }

                    move.RookPiece = board[move.RookFromRow, move.RookFromCol];
                    board[move.RookToRow, move.RookToCol] = move.RookPiece;
                    board[move.RookFromRow, move.RookFromCol] = null;

                    if (piece.Color == PieceColor.White)
                    {
                        if (move.RookFromCol == 0) whiteRookA_Moved = true;
                        if (move.RookFromCol == 7) whiteRookH_Moved = true;
                    }
                    else
                    {
                        if (move.RookFromCol == 0) blackRookA_Moved = true;
                        if (move.RookFromCol == 7) blackRookH_Moved = true;
                    }
                }
            }

            if (piece?.Type == PieceType.Rook)
            {
                if (piece.Color == PieceColor.White)
                {
                    if (move.FromRow == 7 && move.FromCol == 0) whiteRookA_Moved = true;
                    if (move.FromRow == 7 && move.FromCol == 7) whiteRookH_Moved = true;
                }
                else
                {
                    if (move.FromRow == 0 && move.FromCol == 0) blackRookA_Moved = true;
                    if (move.FromRow == 0 && move.FromCol == 7) blackRookH_Moved = true;
                }
            }

            // Track repetitions
            if (gameStarted && move.Captured == null)
            {
                string positionKey = GetPositionKey();
                if (repetitionCounts.ContainsKey(positionKey))
                {
                    repetitionCounts[positionKey]++;
                }
                else
                {
                    repetitionCounts[positionKey] = 1;
                }
            }
        }

        private void UndoMove(ref Move move)
        {
            board[move.FromRow, move.FromCol] = board[move.ToRow, move.ToCol];
            board[move.ToRow, move.ToCol] = move.Captured;

            // Remove from captured pieces list if this was a real capture (only during game)
            if (move.Captured != null && gameStarted)
            {
                if (move.Captured.Color == PieceColor.White)
                {
                    blackCapturedPieces.Remove(move.Captured);
                }
                else
                {
                    whiteCapturedPieces.Remove(move.Captured);
                }
            }

            if (move.IsCastling)
            {
                board[move.RookFromRow, move.RookFromCol] = move.RookPiece;
                board[move.RookToRow, move.RookToCol] = null;
            }

            whiteKingMoved = move.WhiteKingMovedBefore;
            blackKingMoved = move.BlackKingMovedBefore;
            whiteRookA_Moved = move.WhiteRookA_MovedBefore;
            whiteRookH_Moved = move.WhiteRookH_MovedBefore;
            blackRookA_Moved = move.BlackRookA_MovedBefore;
            blackRookH_Moved = move.BlackRookH_MovedBefore;
        }

        // ---------------- SETUP ----------------

        private void InitializeBoard()
        {
            board = new Piece?[8, 8];
            selectedRow = -1;
            selectedCol = -1;
            currentTurn = PieceColor.White;
            whiteKingMoved = false;
            blackKingMoved = false;
            whiteRookA_Moved = false;
            whiteRookH_Moved = false;
            blackRookA_Moved = false;
            blackRookH_Moved = false;
            lastMove = null;
            legalMoveHighlights.Clear();
            whiteCapturedPieces.Clear();
            blackCapturedPieces.Clear();
            repetitionCounts.Clear();
            ResetWdlDisplay();
            UpdateCapturedPiecesDisplay();

            // Set board orientation based on player color
            if (currentGameMode == GameMode.PlayComputer)
            {
                boardFlipped = (playerColor == PieceColor.Black);
            }
            else if (currentGameMode == GameMode.PlayFriend)
            {
                boardFlipped = false; // Start with white at bottom
            }
            else if (currentGameMode == GameMode.Online)
            {
                boardFlipped = (playerColor == PieceColor.Black);
            }

            for (int c = 0; c < 8; c++)
            {
                board[1, c] = new Piece(PieceType.Pawn, PieceColor.Black);
                board[6, c] = new Piece(PieceType.Pawn, PieceColor.White);
            }

            board[0, 0] = board[0, 7] = new Piece(PieceType.Rook, PieceColor.Black);
            board[7, 0] = board[7, 7] = new Piece(PieceType.Rook, PieceColor.White);

            board[0, 1] = board[0, 6] = new Piece(PieceType.Knight, PieceColor.Black);
            board[7, 1] = board[7, 6] = new Piece(PieceType.Knight, PieceColor.White);

            board[0, 2] = board[0, 5] = new Piece(PieceType.Bishop, PieceColor.Black);
            board[7, 2] = board[7, 5] = new Piece(PieceType.Bishop, PieceColor.White);

            board[0, 3] = new Piece(PieceType.Queen, PieceColor.Black);
            board[7, 3] = new Piece(PieceType.Queen, PieceColor.White);

            board[0, 4] = new Piece(PieceType.King, PieceColor.Black);
            board[7, 4] = new Piece(PieceType.King, PieceColor.White);

            repetitionCounts[GetPositionKey()] = 1;
        }

        private HashSet<(int Row, int Col)> GetLegalMovesForSquare(int fromRow, int fromCol)
        {
            var results = new HashSet<(int Row, int Col)>();
            var piece = board[fromRow, fromCol];
            if (piece == null || piece.Color != currentTurn)
                return results;

            for (int tr = 0; tr < 8; tr++)
            {
                for (int tc = 0; tc < 8; tc++)
                {
                    var move = new Move
                    {
                        FromRow = fromRow,
                        FromCol = fromCol,
                        ToRow = tr,
                        ToCol = tc
                    };

                    if (!IsPseudoLegalMove(move))
                        continue;

                    ApplyMove(ref move);
                    bool illegal = IsKingInCheck(currentTurn);
                    UndoMove(ref move);

                    if (!illegal)
                        results.Add((tr, tc));
                }
            }

            return results;
        }

        private void ResetWdlDisplay()
        {
            if (EvaluationFill != null)
            {
                EvaluationFill.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                EvaluationFill.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
            }

            if (WdlWinText != null) WdlWinText.Text = "0";
            if (WdlDrawText != null) WdlDrawText.Text = "0";
            if (WdlLossText != null) WdlLossText.Text = "0";
        }

        private void UpdateWdlDisplay(int win, int draw, int loss, PieceColor sideToMove)
        {
            int total = win + draw + loss;
            if (total <= 0)
                return;

            double whiteScore = sideToMove == PieceColor.White
                ? win + draw * 0.5
                : loss + draw * 0.5;

            double whiteRatio = whiteScore / total;
            double blackRatio = 1 - whiteRatio;

            if (EvaluationFill != null)
            {
                EvaluationFill.RowDefinitions[0].Height = new GridLength(blackRatio, GridUnitType.Star);
                EvaluationFill.RowDefinitions[1].Height = new GridLength(whiteRatio, GridUnitType.Star);
            }

            if (WdlWinText != null) WdlWinText.Text = win.ToString();
            if (WdlDrawText != null) WdlDrawText.Text = draw.ToString();
            if (WdlLossText != null) WdlLossText.Text = loss.ToString();
        }

        private void UpdateCapturedPiecesDisplay()
        {
            // Update White's captured pieces (pieces taken by White from Black)
            if (WhiteCapturedPieces != null)
            {
                WhiteCapturedPieces.Children.Clear();
                var sortedWhite = whiteCapturedPieces.OrderByDescending(p => GetPieceValue(p.Type)).ToList();
                foreach (var piece in sortedWhite)
                {
                    var img = new Image
                    {
                        Source = GetPieceBitmap(piece),
                        Width = 24,
                        Height = 24,
                        Stretch = Avalonia.Media.Stretch.Uniform,
                        Opacity = 0.8
                    };
                    WhiteCapturedPieces.Children.Add(img);
                }
            }

            // Update Black's captured pieces (pieces taken by Black from White)
            if (BlackCapturedPieces != null)
            {
                BlackCapturedPieces.Children.Clear();
                var sortedBlack = blackCapturedPieces.OrderByDescending(p => GetPieceValue(p.Type)).ToList();
                foreach (var piece in sortedBlack)
                {
                    var img = new Image
                    {
                        Source = GetPieceBitmap(piece),
                        Width = 24,
                        Height = 24,
                        Stretch = Avalonia.Media.Stretch.Uniform,
                        Opacity = 0.8
                    };
                    BlackCapturedPieces.Children.Add(img);
                }
            }

            // Calculate material advantage
            int whiteTotal = whiteCapturedPieces.Sum(p => GetPieceValue(p.Type));
            int blackTotal = blackCapturedPieces.Sum(p => GetPieceValue(p.Type));
            int whiteLead = whiteTotal - blackTotal;
            int blackLead = blackTotal - whiteTotal;

            // Display material advantage
            if (WhiteMaterialAdvantage != null)
            {
                if (whiteLead > 0)
                {
                    WhiteMaterialAdvantage.Text = $"+{whiteLead}";
                    WhiteMaterialAdvantage.IsVisible = true;
                }
                else
                {
                    WhiteMaterialAdvantage.IsVisible = false;
                }
            }

            if (BlackMaterialAdvantage != null)
            {
                if (blackLead > 0)
                {
                    BlackMaterialAdvantage.Text = $"+{blackLead}";
                    BlackMaterialAdvantage.IsVisible = true;
                }
                else
                {
                    BlackMaterialAdvantage.IsVisible = false;
                }
            }
        }

        private async void ShowGameOver(string message)
        {
            await new Window
            {
                Width = 300,
                Height = 150,
                Title = "Game Over",
                Content = new TextBlock
                {
                    Text = message,
                    FontSize = 16,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Center
                }
            }.ShowDialog(this);
        }

        private void CancelAiVsAiSetup(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (AiVsAiSetupDialog != null)
            {
                AiVsAiSetupDialog.IsVisible = false;
            }
            currentGameMode = GameMode.None;
        }

        private void StartAiVsAiMatch(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (AiVsAiSetupDialog != null)
            {
                AiVsAiSetupDialog.IsVisible = false;
            }

            aiWhiteName = string.IsNullOrWhiteSpace(AiWhiteNameBox?.Text) ? "Alpha" : AiWhiteNameBox.Text.Trim();
            aiBlackName = string.IsNullOrWhiteSpace(AiBlackNameBox?.Text) ? "Beta" : AiBlackNameBox.Text.Trim();
            aiWhiteMood = GetComboBoxText(AiWhiteMoodBox, "Calm");
            aiBlackMood = GetComboBoxText(AiBlackMoodBox, "Calm");
            aiWhiteLevel = GetDifficultyFromComboBox(AiWhiteLevelBox);
            aiBlackLevel = GetDifficultyFromComboBox(AiBlackLevelBox);

            playerColor = PieceColor.White;
            boardFlipped = false;
            StartAiVsAiGame();
        }

        private string GetComboBoxText(ComboBox? comboBox, string fallback)
        {
            if (comboBox?.SelectedItem is ComboBoxItem item && item.Content is string text)
                return text;
            if (comboBox?.SelectedItem is string value)
                return value;
            return fallback;
        }

        private DifficultyLevel GetDifficultyFromComboBox(ComboBox? comboBox)
        {
            if (comboBox?.SelectedItem is ComboBoxItem item && item.Tag != null && int.TryParse(item.Tag.ToString(), out var level))
            {
                var match = DifficultySettings.Levels.Find(entry => entry.Level == level);
                if (match != null)
                    return match;
            }

            if (comboBox?.SelectedIndex is int index && index >= 0 && index < DifficultySettings.Levels.Count)
                return DifficultySettings.Levels[index];

            return DifficultySettings.Levels[0];
        }

        private void StartAiVsAiGame()
        {
            gameStarted = true;
            InitializeBoard();
            DrawBoard();
            UpdateTitle();

            if (OpponentNameText != null) OpponentNameText.Text = aiBlackName;
            if (PlayerNameText != null) PlayerNameText.Text = aiWhiteName;

            if (HomeView != null) HomeView.IsVisible = false;
            if (GameUI != null) GameUI.IsVisible = true;
            if (ChessBoard != null) ChessBoard.IsVisible = true;
            if (BoardArea != null) BoardArea.IsVisible = true;

            aiVsAiCancellation?.Cancel();
            aiVsAiCancellation = new CancellationTokenSource();
            _ = RunAiVsAiLoopAsync(aiVsAiCancellation.Token);
        }

        private async System.Threading.Tasks.Task RunAiVsAiLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested && gameStarted)
            {
                var level = currentTurn == PieceColor.White ? aiWhiteLevel ?? DifficultySettings.Levels[0] : aiBlackLevel ?? DifficultySettings.Levels[0];
                var mood = currentTurn == PieceColor.White ? aiWhiteMood : aiBlackMood;

                await MakeAiMoveAsync(level, mood, token);

                if (IsCheckmate(currentTurn) || IsStalemate(currentTurn))
                    break;

                await System.Threading.Tasks.Task.Delay(500, token);
            }
        }

        private async System.Threading.Tasks.Task MakeAiMoveAsync(DifficultyLevel level, string mood, CancellationToken token)
        {
            try
            {
                var (depth, skill, limitStrength, elo) = ApplyMood(level, mood);

                var result = await System.Threading.Tasks.Task.Run(() =>
                {
                    using var engine = new StockfishEngine(
                        @"C:\Users\omsai\Downloads\stockfish-windows-x86-64-avx2\stockfish\stockfish-windows-x86-64-avx2.exe"
                    );

                    string fen = ToFEN(currentTurn);
                    engine.SetPosition(fen);

                    if (limitStrength && elo.HasValue)
                        engine.SetEloLimit(elo.Value);
                    else if (skill.HasValue)
                        engine.SetSkillLevel(skill.Value);

                    var engineResult = engine.GetBestMoveWithWDL(depth);
                    return (engineResult.move, (int?)engineResult.win, (int?)engineResult.draw, (int?)engineResult.loss);
                }, token);

                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (result.Item2.HasValue && result.Item3.HasValue && result.Item4.HasValue)
                        UpdateWdlDisplay(result.Item2.Value, result.Item3.Value, result.Item4.Value, currentTurn);

                    ApplyEngineMove(result.move);
                    currentTurn = currentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
                    UpdateTitle();
                    UpdateCapturedPiecesDisplay();
                    DrawBoard();

                    if (CheckThreefoldRepetition())
                        return;

                    if (IsCheckmate(currentTurn))
                        ShowGameOver($"{currentTurn} is checkmated!");
                    else if (IsStalemate(currentTurn))
                        ShowGameOver("Stalemate!");
                });
            }
            catch (Exception ex)
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    ShowGameOver($"AI error: {ex.Message}");
                });
            }
        }

        private (int depth, int? skill, bool limitStrength, int? elo) ApplyMood(DifficultyLevel level, string mood)
        {
            int depth = level.Depth;
            int? skill = level.Skill;
            bool limitStrength = level.LimitStrength;
            int? elo = level.Elo;

            switch (mood)
            {
                case "Aggressive":
                    depth += 1;
                    if (skill.HasValue) skill = Math.Min(skill.Value + 1, 20);
                    break;
                case "Defensive":
                    depth += 1;
                    break;
                case "Random":
                    depth = Math.Max(1, depth - 1);
                    if (skill.HasValue) skill = Math.Max(0, skill.Value - 1);
                    break;
            }

            return (depth, skill, limitStrength, elo);
        }

        private void ShowOnlineSetupDialog()
        {
            waitingForOnlineSetup = true;
            if (OnlineSetupDialog != null)
            {
                OnlineSetupDialog.IsVisible = true;
            }
        }

        private async void HostOnlineGameClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var port = ParsePort(RoomCodeBox?.Text) ?? 5000;
            await StartOnlineGameAsync(isHost: true, host: null, port: port);
        }

        private async void JoinOnlineGameClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var (host, port) = ParseHostAndPort(RoomCodeBox?.Text);
            if (string.IsNullOrWhiteSpace(host) || port == null)
            {
                ShowGameOver("Please enter a valid IP:PORT to join.");
                return;
            }

            await StartOnlineGameAsync(isHost: false, host: host, port: port.Value);
        }

        private void CancelOnlineSetup(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            waitingForOnlineSetup = false;
            if (OnlineSetupDialog != null)
            {
                OnlineSetupDialog.IsVisible = false;
            }
            currentGameMode = GameMode.None;
        }

        private async System.Threading.Tasks.Task StartOnlineGameAsync(bool isHost, string? host, int port)
        {
            waitingForOnlineSetup = false;
            if (OnlineSetupDialog != null)
            {
                OnlineSetupDialog.IsVisible = false;
            }

            isOnlineGame = true;
            onlineManager?.Dispose();
            onlineManager = new OnlineGameManager();
            onlineManager.MoveReceived += OnOnlineMoveReceived;
            onlineManager.StatusChanged += message => Avalonia.Threading.Dispatcher.UIThread.Post(() => Title = $"Chess - {message}");

            try
            {
                if (isHost)
                {
                    playerColor = PieceColor.White;
                    boardFlipped = false;
                    await onlineManager.HostAsync(port);
                }
                else
                {
                    playerColor = PieceColor.Black;
                    boardFlipped = true;
                    await onlineManager.JoinAsync(host!, port);
                }

                StartOnlineGame();
            }
            catch (Exception ex)
            {
                ShowGameOver($"Online error: {ex.Message}");
                CleanupOnlineGame();
            }
        }

        private void StartOnlineGame()
        {
            gameStarted = true;
            InitializeBoard();
            DrawBoard();
            UpdateTitle();

            if (HomeView != null) HomeView.IsVisible = false;
            if (GameUI != null) GameUI.IsVisible = true;
            if (ChessBoard != null) ChessBoard.IsVisible = true;
            if (BoardArea != null) BoardArea.IsVisible = true;
        }

        private void CleanupOnlineGame()
        {
            isOnlineGame = false;
            onlineManager?.Dispose();
            onlineManager = null;
        }

        private void OnOnlineMoveReceived(string move)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                ApplyEngineMove(move);
                currentTurn = currentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
                UpdateTitle();
                UpdateCapturedPiecesDisplay();
                DrawBoard();

                if (CheckThreefoldRepetition())
                    return;

                if (IsCheckmate(currentTurn))
                    ShowGameOver($"{currentTurn} is checkmated!");
                else if (IsStalemate(currentTurn))
                    ShowGameOver("Stalemate!");
            });
        }

        private static int? ParsePort(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            if (int.TryParse(text.Trim(), out var port) && port > 0 && port < 65536)
                return port;

            return null;
        }

        private static (string? host, int? port) ParseHostAndPort(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return (null, null);

            var parts = text.Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2 && int.TryParse(parts[1], out var port))
                return (parts[0].Trim(), port);

            return (null, null);
        }
    }
}

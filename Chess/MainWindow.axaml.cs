using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Markup.Xaml;
using System;

namespace Chess
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            InitializeBoard();
            DrawBoard();
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            Title = $"Chess - {currentTurn} to move";
        }

        private static string GetPieceGlyph(Piece piece)
        {
            return piece.Type switch
            {
                PieceType.Pawn => piece.Color == PieceColor.White ? "♙" : "♟",
                PieceType.Rook => piece.Color == PieceColor.White ? "♖" : "♜",
                PieceType.Knight => piece.Color == PieceColor.White ? "♘" : "♞",
                PieceType.Bishop => piece.Color == PieceColor.White ? "♗" : "♝",
                PieceType.Queen => piece.Color == PieceColor.White ? "♕" : "♛",
                PieceType.King => piece.Color == PieceColor.White ? "♔" : "♚",
                _ => string.Empty,
            };
        }

        private Piece[,] board = new Piece[8, 8];
        private int selectedRow = -1;
        private int selectedCol = -1;
        private PieceColor currentTurn = PieceColor.White;

        private void DrawBoard()
        {
            ChessBoard.Children.Clear();

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    int r = row;
                    int c = col;

                    var square = new Border
                    {
                        Background = ((r + c) % 2 == 0)
                            ? Brushes.Beige
                            : Brushes.SaddleBrown,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1)
                    };

                    var piece = board[r, c];
                    if (piece != null)
                    {
                        var pieceText = new TextBlock
                        {
                            Text = GetPieceGlyph(piece),
                            FontSize = 32,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                        };
                        square.Child = pieceText;
                    }

                    if (r == selectedRow && c == selectedCol)
                    {
                        square.BorderBrush = Brushes.Red;
                        square.BorderThickness = new Thickness(3);
                    }


                    square.PointerPressed += (s, e) =>
                    {
                        OnSquareClicked(r, c);
                    };

                    Grid.SetRow(square, r);
                    Grid.SetColumn(square, c);

                    ChessBoard.Children.Add(square);
                }
            }
        }

        private void InitializeBoard()
        {
            for (int col = 0; col < 8; col++)
            {
                board[1, col] = new Piece(PieceType.Pawn, PieceColor.Black);
                board[6, col] = new Piece(PieceType.Pawn, PieceColor.White);
            }

            //Rooks
            board[0, 0] = new Piece(PieceType.Rook, PieceColor.Black);
            board[0, 7] = new Piece(PieceType.Rook, PieceColor.Black);
            board[7, 0] = new Piece(PieceType.Rook, PieceColor.White);
            board[7, 7] = new Piece(PieceType.Rook, PieceColor.White);

            //Kings
            board[0, 4] = new Piece(PieceType.King, PieceColor.Black);
            board[7, 4] = new Piece(PieceType.King, PieceColor.White);

            //Queens
            board[0, 3] = new Piece(PieceType.Queen, PieceColor.Black);
            board[7, 3] = new Piece(PieceType.Queen, PieceColor.White);

            //Bishops
            board[0, 2] = new Piece(PieceType.Bishop, PieceColor.Black);
            board[0, 5] = new Piece(PieceType.Bishop, PieceColor.Black);
            board[7, 2] = new Piece(PieceType.Bishop, PieceColor.White);
            board[7, 5] = new Piece(PieceType.Bishop, PieceColor.White);

            //Knights
            board[0, 6] = new Piece(PieceType.Knight, PieceColor.Black);
            board[0, 1] = new Piece(PieceType.Knight, PieceColor.Black);
            board[7, 6] = new Piece(PieceType.Knight, PieceColor.White);
            board[7, 1] = new Piece(PieceType.Knight, PieceColor.White);

        }

        private void OnSquareClicked(int row, int col)
        {
            if (selectedRow == -1)
            {
                var piece = board[row, col];

                if (piece == null)
                {
                    return;
                }

                if (piece.Color != currentTurn)
                {
                    return;
                }

                selectedRow = row;
                selectedCol = col;
                DrawBoard();
            }

            else
            {
                // If user taps another of their own pieces, switch selection instead of moving
                var tappedPiece = board[row, col];
                if (tappedPiece != null && tappedPiece.Color == currentTurn)
                {
                    selectedRow = row;
                    selectedCol = col;
                    DrawBoard();
                    return;
                }

                bool move = TryMove(selectedRow, selectedCol, row, col);

                if (move)
                {
                    // flip turn only if a move was made
                    currentTurn = currentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;
                    // clear selection after a successful move
                    selectedRow = -1;
                    selectedCol = -1;
                    UpdateTitle();
                }

                // redraw board (keep selection if move was not made)
                DrawBoard();
            }

        }

        private bool TryMove(int fromRow, int fromCol, int toRow, int toCol)
        {
            var piece = board[fromRow, fromCol];
            if (piece == null)
            {
                return false;
            }

            if (piece.Type == PieceType.Pawn)
            {
                int direction = piece.Color == PieceColor.White ? -1 : 1;
                // simple forward move
                if (fromCol == toCol && toRow == fromRow + direction && board[toRow, toCol] == null)
                {
                    board[toRow, toCol] = piece;
                    board[fromRow, fromCol] = null;
            return false;
                }
                // capture diagonally
                if (Math.Abs(toCol - fromCol) == 1 && toRow == fromRow + direction && board[toRow, toCol] != null && board[toRow, toCol].Color != piece.Color)
                {
                    board[toRow, toCol] = piece;
                    board[fromRow, fromCol] = null;
                    return true;
                }
                return false;
            }
            else if (piece.Type == PieceType.Rook)
            {
                // Rook moves any number of squares horizontally or vertically
                if (fromRow == toRow && fromCol != toCol)
                {
                    int step = toCol > fromCol ? 1 : -1;
                    // Check path is clear (excluding destination)
                    for (int c = fromCol + step; c != toCol; c += step)
                    {
                        if (board[fromRow, c] != null)
                            return false; // blocked
                    }
                    // Destination must be empty or occupied by opponent
                    var dest = board[toRow, toCol];
                    if (dest == null || dest.Color != piece.Color)
                    {
                        board[toRow, toCol] = piece;
                        board[fromRow, fromCol] = null;
                        return true;
                    }
                }
                else if (fromCol == toCol && fromRow != toRow)
                {
                    int step = toRow > fromRow ? 1 : -1;
                    for (int r = fromRow + step; r != toRow; r += step)
                    {
                        if (board[r, fromCol] != null)
                            return false; // blocked
                    }
                    var dest = board[toRow, toCol];
                    if (dest == null || dest.Color != piece.Color)
                    {
                        board[toRow, toCol] = piece;
                        board[fromRow, fromCol] = null;
                        return true;
                    }
                }
            }

            else if (piece.Type == PieceType.Bishop)
            {
                if (fromRow != toRow && fromCol != toCol && Math.Abs(fromCol - toCol) == Math.Abs(fromRow - toRow))
                {
                    int stepCount = Math.Abs(fromCol - toCol);
                    for (int i = 0; i < stepCount; i++)
                    {
                        int r = fromRow + (toRow > fromRow ? 1 : -1) * (i + 1);
                        int c = fromCol + (toCol > fromCol ? 1 : -1) * (i + 1);
                        if (i < stepCount - 1 && board[r, c] != null)
                            return false; // blocked
                    }
                    var dest = board[toRow, toCol];
                    if (dest == null || dest.Color != piece.Color)
                    {
                        board[toRow, toCol] = piece;
                        board[fromRow, fromCol] = null;
                        return true;
                    }
                }
            }

            else if (piece.Type == PieceType.Knight)
            {
                if ((Math.Abs(fromRow - toRow) == 2 && Math.Abs(fromCol - toCol) == 1) ||
                    (Math.Abs(fromRow - toRow) == 1 && Math.Abs(fromCol - toCol) == 2))
                {
                    var dest = board[toRow, toCol];
                    if (dest == null || dest.Color != piece.Color)
                    {
                        board[toRow, toCol] = piece;
                        board[fromRow, fromCol] = null;
                        return true;
                    }
                }
            }

            else if (piece.Type == PieceType.Queen)
            {

                // Rook moves any number of squares horizontally or vertically
                if (fromRow == toRow && fromCol != toCol)
                {
                    int step = toCol > fromCol ? 1 : -1;
                    // Check path is clear (excluding destination)
                    for (int c = fromCol + step; c != toCol; c += step)
                    {
                        if (board[fromRow, c] != null)
                            return false; // blocked
                    }
                    // Destination must be empty or occupied by opponent
                    var dest = board[toRow, toCol];
                    if (dest == null || dest.Color != piece.Color)
                    {
                        board[toRow, toCol] = piece;
                        board[fromRow, fromCol] = null;
                        return true;
                    }
                }
                else if (fromCol == toCol && fromRow != toRow)
                {
                    int step = toRow > fromRow ? 1 : -1;
                    for (int r = fromRow + step; r != toRow; r += step)
                    {
                        if (board[r, fromCol] != null)
                            return false; // blocked
                    }
                    var dest = board[toRow, toCol];
                    if (dest == null || dest.Color != piece.Color)
                    {
                        board[toRow, toCol] = piece;
                        board[fromRow, fromCol] = null;
                        return true;
                    }
                }
                else if (fromRow != toRow && fromCol != toCol && Math.Abs(fromCol - toCol) == Math.Abs(fromRow - toRow))
                {
                    int stepCount = Math.Abs(fromCol - toCol);
                    for (int i = 0; i < stepCount; i++)
                    {
                        int r = fromRow + (toRow > fromRow ? 1 : -1) * (i + 1);
                        int c = fromCol + (toCol > fromCol ? 1 : -1) * (i + 1);
                        if (i < stepCount - 1 && board[r, c] != null)
                            return false; // blocked
                    }
                    var dest = board[toRow, toCol];
                    if (dest == null || dest.Color != piece.Color)
                    {
                        board[toRow, toCol] = piece;
                        board[fromRow, fromCol] = null;
                        return true;
                    }
                }
            }

            return true;
        }
    }
}

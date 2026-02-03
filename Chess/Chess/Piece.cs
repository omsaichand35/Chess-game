using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public enum PieceType
    {
        Pawn,
        Rook,
        King,
        Queen,
        Bishop,
        Knight
    }

    public enum PieceColor
    {
        White,
        Black
    }

    public class  Piece
    {
        public PieceType Type { get; }
        public PieceColor Color { get; }

        public Piece(PieceType type, PieceColor color)
        {
            Type = type;
            Color = color;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public struct Move
    {
        public int FromRow, FromCol;
        public int ToRow, ToCol;
        public Piece? Captured;
        public bool IsCastling;
        public int RookFromRow, RookFromCol;
        public int RookToRow, RookToCol;
        public Piece? RookPiece;
        public bool WhiteKingMovedBefore;
        public bool BlackKingMovedBefore;
        public bool WhiteRookA_MovedBefore;
        public bool WhiteRookH_MovedBefore;
        public bool BlackRookA_MovedBefore;
        public bool BlackRookH_MovedBefore;
    }
}
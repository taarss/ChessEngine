using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Model
{
    public class PreviousMove
    {
        private Piece.Piece capturedPiece;
        private Move move;

        public PreviousMove(Move move)
        {
            this.move = move;
        }
        public PreviousMove(Move move, Piece.Piece capturedPiece)
        {
            this.capturedPiece = capturedPiece;
            this.move = move;
        }

        public Piece.Piece CapturedPiece { get => capturedPiece; set => capturedPiece = value; }
        public Move Move { get => move; set => move = value; }
    }
}

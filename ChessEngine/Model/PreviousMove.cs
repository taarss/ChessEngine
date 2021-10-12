using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Model
{
    public class PreviousMove
    {
        private CapturedPiece capturedPiece;
        private Move move;

        public PreviousMove(CapturedPiece capturedPiece, Move move)
        {
            this.capturedPiece = capturedPiece;
            this.move = move;
        }

        public CapturedPiece CapturedPiece { get => capturedPiece; set => capturedPiece = value; }
        public Move Move { get => move; set => move = value; }
    }
}

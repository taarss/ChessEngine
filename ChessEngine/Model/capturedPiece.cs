using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Model.Piece;

namespace ChessEngine.Model
{
    public class CapturedPiece
    {
        private int index;
        private Piece.Piece piece;

        public CapturedPiece(int index, Piece.Piece piece)
        {
            this.index = index;
            this.piece = piece;
        }

        public int Index { get => index; set => index = value; }
        public Piece.Piece Piece { get => piece; set => piece = value; }
    }
}

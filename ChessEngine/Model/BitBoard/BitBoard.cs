using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Model.BitBoard;

namespace ChessEngine.Model.BitBoard
{
    public class BitBoard
    {
        public int[] Square;

        public BitBoard()
        {
            Square = new int[64];
            Square[0] = BitPiece.White | BitPiece.Bishop;
        }
    }
}

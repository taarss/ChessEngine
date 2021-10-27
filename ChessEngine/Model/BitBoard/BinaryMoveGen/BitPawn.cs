using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Model.BitBoard.BinaryMoveGen
{
    public class BitPawn
    {

        public List<BitMove> GenerateMoves(BitBoard bitBoard)
        {
            return GeneratePawnMoves()
        }

        public List<BitMove> GeneratePawnMoves(string binaryString, int startSquare, int[] squares)
        {
            
            int[] dirs = (binaryString.Substring(0, 2) == "01" ? true : false) ? new[] { -9, -7 } : new[] { 9, 7 };
            int[] pushDir = (binaryString.Substring(0, 2) == "01" ? true : false) ? new[] { -8, -16 } : new[] { 8, 16 };
            string color = (binaryString.Substring(0, 2) == "01" ? "11" : "01");
            int[] enPassentCheckList = new int[] { -1, 1 };
            int startRank = (binaryString.Substring(0, 2) == "01" ? true : false) ? 1 : 6;    
            List<BitMove> moves = new();

            if (BoardRepresentation.RankIndex(startSquare) == startRank)
            {
                if (squares[startSquare + pushDir[1]] == 0)
                {
                    moves.Add(new BitMove(startSquare, startSquare + pushDir[1]));
                }
            }
            foreach (var possibleCapture in dirs)
            {
                if (possibleCapture > 0 && possibleCapture < 64 && squares[startSquare + possibleCapture] != 0)
                {
                    if (BitPiece.Colour(squares[startSquare + possibleCapture]) == Convert.ToInt32(color))
                    {
                        moves.Add(new BitMove(startSquare, startSquare + pushDir[1]));
                    }
                }
            }
            return moves;


        }
    }
}

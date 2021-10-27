using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Model;
using ChessEngine.Model.Piece;
using ChessEngine.ViewModel;

namespace ChessEngine.Model.AI
{
    public class MoveOrder
    {
        private Evaluate evaluation = new();
        int[] moveScores;
        const int maxMoveCount = 218;

        public MoveOrder()
        {
            moveScores = new int[maxMoveCount];
        }


        

        List<Move> Sort(List<Move> moves)
        {
            // Sort the moves list based on scores
            for (int i = 0; i < moves.Count - 1; i++)
            {
                for (int j = i + 1; j > 0; j--)
                {
                    int swapIndex = j - 1;
                    if (moveScores[swapIndex] < moveScores[j])
                    {
                        (moves[j], moves[swapIndex]) = (moves[swapIndex], moves[j]);
                        (moveScores[j], moveScores[swapIndex]) = (moveScores[swapIndex], moveScores[j]);
                    }
                }
            }
            return moves;
        }
    }
}

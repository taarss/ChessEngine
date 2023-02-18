using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Model;
using ChessEngine.Model.Piece;
using ChessEngine.ViewModel;
using ChessEngine.Model.BitBoard;

namespace ChessEngine.Model.AI
{
    public class MoveOrder
    {
        private Evaluation evaluation = new();
        int[] moveScores;
        const int maxMoveCount = 218;

        public MoveOrder()
        {
            moveScores = new int[maxMoveCount];
        }


        public List<BitMove> OrderMoves(List<BitMove> moves)
        {
            BoardViewModel board = (BoardViewModel)App.Current.Resources["boardViewModel"];
            for (int i = 0; i < moves.Count; i++)
            {
                int score = 0;
                int movePieceType = board.BitBoard.Square[i];
                int capturePieceType = board.BitBoard.Square[i];

                //Favor capturing opponents most valuable pieces with our least valuable pieces
                if (capturePieceType != 0)
                {
                    score = 10 * GetPieceValue(capturePieceType) - GetPieceValue(movePieceType);
                }

                foreach (var attackedSquare in board.AttackMap)
                {
                    if (attackedSquare.TargetSquare == moves[i].TargetSquare)
                    {
                        score -= GetPieceValue(movePieceType);
                        break;
                    }
                }
                moveScores[i] = score;
            }
            return Sort(moves);

        }
        static int GetPieceValue(int pieceType)
        {
            switch (pieceType)
            {
                case BitPiece.Queen:
                    return Evaluation.queenValue;
                case BitPiece.Rook:
                    return Evaluation.rookValue;
                case BitPiece.Knight:
                    return Evaluation.knightValue;
                case BitPiece.Bishop:
                    return Evaluation.bishopValue;
                case BitPiece.Pawn:
                    return Evaluation.pawnValue;
                default:
                    return 0;
            }
        }
        List<BitMove> Sort(List<BitMove> moves)
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
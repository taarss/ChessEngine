using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Model;
using ChessEngine.ViewModel;

namespace ChessEngine.Model.AI
{
    public class AI
    {
        private Evaluate evaluate = new();
        private Move bestMove;

        public Move BestMove { get => bestMove; set => bestMove = value; }

        public int SearchMoves(int depth, int alpha, int beta)
        {
            BoardViewModel board = (BoardViewModel)App.Current.Resources["boardViewModel"];
            List<Move> moves = Check.GenerateAllLegelMoves();
            if (depth == 0)
            {
                return Evaluate.EvaluateMaterial();
            }

            if (moves.Count == 0)
            {
                return Int32.MinValue;
            }

            foreach (var move in moves)
            {
                board.MoveLogic.MakePseudoMove(move);
                int evaluation = -SearchMoves(depth - 1, -beta, -alpha);
                board.MoveLogic.UnmakeMove();
                if (evaluation >= beta)
                {
                    //Move was too good so the oppenent will avoid this position
                    //Skip this branch
                    return beta;
                }
                if (evaluation > alpha)
                {
                    bestMove = move;
                }
                alpha = Math.Max(alpha, evaluation);

            }

            return alpha;
        }
    }
}

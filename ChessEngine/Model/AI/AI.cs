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
		/*
        private Evaluate evaluate = new();
        private Move bestMove;
        const int transpositionTableSize = 64000;
        const int immediateMateScore = 100000;
        const int positiveInfinity = 9999999;
        const int negativeInfinity = -positiveInfinity;

        public Move BestMove { get => bestMove; set => bestMove = value; }

        public int SearchMoves(int depth, int alpha, int beta)
        {
            BoardViewModel board = (BoardViewModel)App.Current.Resources["boardViewModel"];
            if (depth == 0)
            {
                return Evaluate.EvaluateMaterial();
            }
            List<Move> moves = Check.GenerateAllLegelMoves();
            if (moves.Count == 0)
            {
                return negativeInfinity;
            }

            foreach (Move move in moves)
            {
                board.MoveLogic.MakePseudoMove(move);
                int evaluation = -SearchMoves(depth - 1, -beta, -alpha);
                board.MoveLogic.UnmakeMove();
                if (evaluation >= beta)
                {
                    bestMove = move;

                }
                alpha = Math.Max(alpha, evaluation);
            }
            return alpha;

        }*/
		const int transpositionTableSize = 64000;
		const int immediateMateScore = 100000;
		const int positiveInfinity = 9999999;
		const int negativeInfinity = -positiveInfinity;

		Move bestMoveThisIteration;
		int bestEvalThisIteration;
		Move bestMove;
		int bestEval;
		int currentIterativeSearchDepth;
		bool abortSearch;

		Move invalidMove;
		BoardViewModel board = (BoardViewModel)App.Current.Resources["boardViewModel"];
		Evaluate evaluate = new();

		// Diagnostics
		int numNodes;
		int numQNodes;
		int numCutoffs;
		int numTranspositions;
		int numPositionsEvaluated = 0;
		System.Diagnostics.Stopwatch searchStopwatch;


		public void StartSearch()
		{

			// Initialize search settings
			bestEvalThisIteration = bestEval = 0;

			currentIterativeSearchDepth = 0;
			abortSearch = false;

			// Iterative deepening. This means doing a full search with a depth of 1, then with a depth of 2, and so on.
			// This allows the search to be aborted at any time, while still yielding a useful result from the last search.
			
			SearchMoves(4, 0, negativeInfinity, positiveInfinity);
			bestMove = bestMoveThisIteration;
			bestEval = bestEvalThisIteration;
			

		}

		public (Move move, int eval) GetSearchResult()
		{
			return (bestMove, bestEval);
		}

		public void EndSearch()
		{
			abortSearch = true;
		}

		int SearchMoves(int depth, int plyFromRoot, int alpha, int beta)
		{
			if (abortSearch)
			{
				return 0;
			}

			if (plyFromRoot > 0)
			{
				
				// Skip this position if a mating sequence has already been found earlier in
				// the search, which would be shorter than any mate we could find from here.
				// This is done by observing that alpha can't possibly be worse (and likewise
				// beta can't  possibly be better) than being mated in the current position.
				alpha = Math.Max(alpha, -immediateMateScore + plyFromRoot);
				beta = Math.Min(beta, immediateMateScore - plyFromRoot);
				if (alpha >= beta)
				{
					int evaluation = QuiescenceSearch(alpha, beta);
					return evaluation;
				}
			}

			

			if (depth == 0)
			{
				return Evaluate.EvaluateMaterial();
			}

			//List<Move> moves = Check.GenerateAllLegelMoves();
			board = (BoardViewModel)App.Current.Resources["boardViewModel"];
			List<Move> moves = board.MoveLogic.GenerateMoves();
			MoveOrder moveOrder = new();
			moves = moveOrder.OrderMoves(moves);
			// Detect checkmate and stalemate when no legal moves are available
			if (moves.Count == 0)
			{			
				return -negativeInfinity;
			}

			Move bestMoveInThisPosition = invalidMove;
			board = (BoardViewModel)App.Current.Resources["boardViewModel"];
			for (int i = 0; i < moves.Count; i++)
			{
				board.MoveLogic.MakeMove(moves[i]);
				int eval = -SearchMoves(depth - 1, plyFromRoot + 1, -beta, -alpha);
				board.MoveLogic.UnmakeMove();
				numNodes++;

				// Move was *too* good, so opponent won't allow this position to be reached
				// (by choosing a different move earlier on). Skip remaining moves.
				if (eval >= beta)
				{
					numCutoffs++;
					return beta;
				}

				// Found a new best move in this position
				if (eval > alpha)
				{
					bestMoveInThisPosition = moves[i];

					alpha = eval;
					if (plyFromRoot == 0)
					{
						bestMoveThisIteration = moves[i];
						bestEvalThisIteration = eval;
					}
				}
			}
			return alpha;
		}

		// Search capture moves until a 'quiet' position is reached.
		int QuiescenceSearch(int alpha, int beta)
		{
			MoveOrder moveOrder = new();
			// A player isn't forced to make a capture (typically), so see what the evaluation is without capturing anything.
			// This prevents situations where a player ony has bad captures available from being evaluated as bad,
			// when the player might have good non-capture moves available.
			int eval = Evaluate.EvaluateMaterial();
			numPositionsEvaluated++;
			if (eval >= beta)
			{
				return beta;
			}
			if (eval > alpha)
			{
				alpha = eval;
			}
			board = (BoardViewModel)App.Current.Resources["boardViewModel"];
			var moves = board.MoveLogic.GenerateMoves();
			moves = moveOrder.OrderMoves(moves);
			for (int i = 0; i < moves.Count; i++)
			{
				board.MoveLogic.MakeMove(moves[i]);
				eval = -QuiescenceSearch(-beta, -alpha);
				board.MoveLogic.UnmakeMove();
				numQNodes++;

				if (eval >= beta)
				{
					numCutoffs++;
					return beta;
				}
				if (eval > alpha)
				{
					alpha = eval;
				}
			}

			return alpha;
		}




	}
}

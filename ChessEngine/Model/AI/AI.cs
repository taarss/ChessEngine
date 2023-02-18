using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Model;
using ChessEngine.ViewModel;
using ChessEngine.Model.BitBoard;
using ChessEngine.Model.AI;

namespace ChessEngine.Model.AI
{
	public class AI
	{
		const int immediateMateScore = 100000;
		const int positiveInfinity = 9999999;
		const int negativeInfinity = -positiveInfinity;

		BitMoveGeneration moveGeneration = new();
		BitMove bestMoveThisIteration;
		int bestEvalThisIteration;
		BitMove bestMove;
		int bestEval;
		bool abortSearch;

		BitMove invalidMove;
		BoardViewModel board = (BoardViewModel)App.Current.Resources["boardViewModel"];
		Evaluation evaluate = new();

		// Diagnostics
		int numNodes;
		int numQNodes;
		int numCutoffs;
		int numTranspositions;
		int numPositionsEvaluated = 0;
		System.Diagnostics.Stopwatch searchStopwatch = new();
		MoveOrder moveOrder = new();

		public void StartSearch()
		{

			// Initialize search settings
			bestEvalThisIteration = bestEval = 0;

			abortSearch = false;

			// Iterative deepening. This means doing a full search with a depth of 1, then with a depth of 2, and so on.
			// This allows the search to be aborted at any time, while still yielding a useful result from the last search.

			board = (BoardViewModel)App.Current.Resources["boardViewModel"];
			board.MoveLogic.SetViewModel();
			searchStopwatch.Start();
			SearchMoves(4, 0, negativeInfinity, positiveInfinity);
			searchStopwatch.Stop();
			Console.WriteLine(searchStopwatch.ElapsedMilliseconds);
			bestMove = bestMoveThisIteration;
			bestEval = bestEvalThisIteration;
		}

		public BitMove GetSearchResult()
		{
			return bestMove;
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

			if (depth == 0)
			{
				return evaluate.Evaluate(board.BitBoard);
			}

			//List<Move> moves = Check.GenerateAllLegelMoves();
			board.AttackMap = new();
			List<BitMove> moves = moveGeneration.GenerateMoves(board.BitBoard);

			moves = moveOrder.OrderMoves(moves);
			// Detect checkmate and stalemate when no legal moves are available
			if (moves.Count == 0)
			{
				return -negativeInfinity;
			}

			BitMove bestMoveInThisPosition = invalidMove;
			for (int i = 0; i < moves.Count; i++)
			{
				board.BitBoard.MakeMove(moves[i]);
				int eval = -SearchMoves(depth - 1, plyFromRoot + 1, -beta, -alpha);
				board.MoveLogic.UnmakeMove();
				numNodes++;

				// Move was *too* good, so opponent won't allow this position to be reached
				// (by choosing a different move earlier on). Skip remaining moves.

				if (eval > beta)
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





	}
}
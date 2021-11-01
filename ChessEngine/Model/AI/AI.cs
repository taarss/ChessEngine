using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.ViewModel;
using ChessEngine.Model.BitBoard;
using ChessEngine.Model.BitBoard.BinaryMoveGen;
namespace ChessEngine.Model.AI
{
    public class AI
    {
		const int transpositionTableSize = 64000;
		const int immediateMateScore = 100000;
		const int positiveInfinity = 9999999;
		const int negativeInfinity = -positiveInfinity;

		public event System.Action<Move> onSearchComplete;

		PseudoLegalMoveGenerator moveGenerator;

		BitMove bestMoveThisIteration;
		int bestEvalThisIteration;
		BitMove bestMove;
		int bestEval;
		int currentIterativeSearchDepth;
		bool abortSearch;

		BitMove invalidMove;
		BitBoard.BitBoard board;
		Evaluate evaluation;

		// Diagnostics
		int numNodes;
		int numQNodes;
		int numCutoffs;
		int numTranspositions;
		System.Diagnostics.Stopwatch searchStopwatch;

		public AI(BitBoard.BitBoard board)
		{
			this.board = board;
			evaluation = new Evaluate();
			moveGenerator = new PseudoLegalMoveGenerator();
			invalidMove = BitMove.InvalidMove;
		}

		public void StartSearch()
		{

			// Initialize search settings
			bestEvalThisIteration = bestEval = 0;
			bestMoveThisIteration = bestMove = BitMove.InvalidMove;

		

			abortSearch = false;

			
			
			
			SearchMoves(4, 0, negativeInfinity, positiveInfinity);
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
					return alpha;
				}
			}

				
			if (depth == 0)
			{
				int evaluation = QuiescenceSearch(alpha, beta);
				return evaluation;
			}

			List<BitMove> moves = moveGenerator.GenerateMoves(board);
			// Detect checkmate and stalemate when no legal moves are available
			if (moves.Count == 0)
			{
				return 0;
				
			}
			BitMove bestMoveInThisPosition = invalidMove;
			BoardViewModel vm = (BoardViewModel)App.Current.Resources["boardViewModel"];
			for (int i = 0; i < moves.Count; i++)
			{
				board = new(vm.bitBoard);
				board.MakeMove(moves[i], inSearch: true);
				int eval = -SearchMoves(depth - 1, plyFromRoot + 1, -beta, -alpha);
				board.UnmakeMove(moves[i], inSearch: true);
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
			// A player isn't forced to make a capture (typically), so see what the evaluation is without capturing anything.
			// This prevents situations where a player ony has bad captures available from being evaluated as bad,
			// when the player might have good non-capture moves available.
			int eval = evaluation.EvaluateBoard(board);
			if (eval >= beta)
			{
				return beta;
			}
			if (eval > alpha)
			{
				alpha = eval;
			}

			var moves = moveGenerator.GenerateMoves(board);
			for (int i = 0; i < moves.Count; i++)
			{
				board.MakeMove(moves[i], true);
				eval = -QuiescenceSearch(-beta, -alpha);
				board.UnmakeMove(moves[i], true);
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

		public static bool IsMateScore(int score)
		{
			const int maxMateDepth = 1000;
			return System.Math.Abs(score) > immediateMateScore - maxMateDepth;
		}

		public static int NumPlyToMateFromScore(int score)
		{
			return immediateMateScore - System.Math.Abs(score);

		}
	}
}

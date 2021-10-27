using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Model;
using ChessEngine.ViewModel;
using ChessEngine.Model.BitBoard;

namespace ChessEngine.Model.AI
{
    public class Evaluate
    {
			public const int pawnValue = 100;
			public const int knightValue = 300;
			public const int bishopValue = 320;
			public const int rookValue = 500;
			public const int queenValue = 900;

			const float endgameMaterialStart = rookValue * 2 + bishopValue + knightValue;
			BitBoard.BitBoard board;

			// Performs static evaluation of the current position.
			// The position is assumed to be 'quiet', i.e no captures are available that could drastically affect the evaluation.
			// The score that's returned is given from the perspective of whoever's turn it is to move.
			// So a positive score means the player who's turn it is to move has an advantage, while a negative score indicates a disadvantage.
			public int EvaluateBoard(BitBoard.BitBoard board)
			{
				this.board = board;
				int whiteEval = 0;
				int blackEval = 0;

				int whiteMaterial = CountMaterial(BitBoard.BitBoard.WhiteIndex);
				int blackMaterial = CountMaterial(BitBoard.BitBoard.BlackIndex);

				int whiteMaterialWithoutPawns = whiteMaterial - board.pawns[BitBoard.BitBoard.WhiteIndex].Count * pawnValue;
				int blackMaterialWithoutPawns = blackMaterial - board.pawns[BitBoard.BitBoard.BlackIndex].Count * pawnValue;
				float whiteEndgamePhaseWeight = EndgamePhaseWeight(whiteMaterialWithoutPawns);
				float blackEndgamePhaseWeight = EndgamePhaseWeight(blackMaterialWithoutPawns);

				whiteEval += whiteMaterial;
				blackEval += blackMaterial;
				whiteEval += MopUpEval(BitBoard.BitBoard.WhiteIndex, BitBoard.BitBoard.BlackIndex, whiteMaterial, blackMaterial, blackEndgamePhaseWeight);
				blackEval += MopUpEval(BitBoard.BitBoard.BlackIndex, BitBoard.BitBoard.WhiteIndex, blackMaterial, whiteMaterial, whiteEndgamePhaseWeight);

				int eval = whiteEval - blackEval;

				int perspective = (board.WhiteToMove) ? 1 : -1;
				return eval * perspective;
			}

			float EndgamePhaseWeight(int materialCountWithoutPawns)
			{
				const float multiplier = 1 / endgameMaterialStart;
				return 1 - System.Math.Min(1, materialCountWithoutPawns * multiplier);
			}

			int MopUpEval(int friendlyIndex, int opponentIndex, int myMaterial, int opponentMaterial, float endgameWeight)
			{
				int mopUpScore = 0;
				if (myMaterial > opponentMaterial + pawnValue * 2 && endgameWeight > 0)
				{

					int friendlyKingSquare = board.KingSquare[friendlyIndex];
					int opponentKingSquare = board.KingSquare[opponentIndex];
					mopUpScore += PrecomputedMoveData.centreManhattanDistance[opponentKingSquare] * 10;
					// use ortho dst to promote direct opposition
					mopUpScore += (14 - PrecomputedMoveData.NumRookMovesToReachSquare(friendlyKingSquare, opponentKingSquare)) * 4;

					return (int)(mopUpScore * endgameWeight);
				}
				return 0;
			}

			int CountMaterial(int colourIndex)
			{
				int material = 0;
				material += board.pawns[colourIndex].Count * pawnValue;
				material += board.knights[colourIndex].Count * knightValue;
				material += board.bishops[colourIndex].Count * bishopValue;
				material += board.rooks[colourIndex].Count * rookValue;
				material += board.queens[colourIndex].Count * queenValue;

				return material;
			}					
		
	}
}

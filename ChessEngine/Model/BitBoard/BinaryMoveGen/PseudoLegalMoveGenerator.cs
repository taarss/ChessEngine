using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Model.BitBoard.BinaryMoveGen
{
	public class PseudoLegalMoveGenerator
	{

		// ---- Instance variables ----
		List<BitMove> moves;
		bool isWhiteToMove;
		int friendlyColour;
		int opponentColour;
		int friendlyKingSquare;
		int friendlyColourIndex;
		int opponentColourIndex;
		BitBoard board;

		// Generates list of legal moves in current position.
		public List<BitMove> GenerateMoves(BitBoard board)
		{
			this.board = board;
			Init();
			GenerateKingMoves();
			GenerateSlidingMoves();
			GeneratePawnMoves();
			GenerateKnightMoves();

			return moves;
		}

		void Init()
		{
			moves = new List<BitMove>(64);
			isWhiteToMove = board.ColourToMove == BitPiece.White;
			friendlyColour = board.ColourToMove;
			opponentColour = board.OpponentColour;
			friendlyKingSquare = board.KingSquare[board.ColourToMoveIndex];
			friendlyColourIndex = (board.WhiteToMove) ? BitBoard.WhiteIndex : BitBoard.BlackIndex;
			opponentColourIndex = 1 - friendlyColourIndex;
		}

		void GenerateKingMoves()
		{
			for (int i = 0; i < PrecomputedMoveData.kingMoves[friendlyKingSquare].Length; i++)
			{
				int targetSquare = PrecomputedMoveData.kingMoves[friendlyKingSquare][i];
				int pieceOnTargetSquare = board.Square[targetSquare];

				// Skip squares occupied by friendly pieces
				if (BitPiece.IsColour(pieceOnTargetSquare, friendlyColour))
				{
					continue;
				}
				// Safe for king to move to this square

				moves.Add(new BitMove(friendlyKingSquare, targetSquare));


			}
		}

		void GenerateSlidingMoves()
		{
			PieceList rooks = board.rooks[friendlyColourIndex];
			for (int i = 0; i < rooks.Count; i++)
			{
				GenerateSlidingPieceMoves(rooks[i], 0, 4);
			}

			PieceList bishops = board.bishops[friendlyColourIndex];
			for (int i = 0; i < bishops.Count; i++)
			{
				GenerateSlidingPieceMoves(bishops[i], 4, 8);
			}

			PieceList queens = board.queens[friendlyColourIndex];
			for (int i = 0; i < queens.Count; i++)
			{
				GenerateSlidingPieceMoves(queens[i], 0, 8);
			}

		}

		void GenerateSlidingPieceMoves(int startSquare, int startDirIndex, int endDirIndex)
		{

			for (int directionIndex = startDirIndex; directionIndex < endDirIndex; directionIndex++)
			{
				int currentDirOffset = PrecomputedMoveData.directionOffsets[directionIndex];

				for (int n = 0; n < PrecomputedMoveData.numSquaresToEdge[startSquare][directionIndex]; n++)
				{
					int targetSquare = startSquare + currentDirOffset * (n + 1);
					int targetSquarePiece = board.Square[targetSquare];

					// Blocked by friendly piece, so stop looking in this direction
					if (BitPiece.IsColour(targetSquarePiece, friendlyColour))
					{
						break;
					}
					bool isCapture = targetSquarePiece != BitPiece.None;



					// If square not empty, can't move any further in this direction
					// Also, if this move blocked a check, further moves won't block the check
					if (isCapture)
					{
						break;
					}
				}
			}
		}

		void GenerateKnightMoves()
		{
			PieceList myKnights = board.knights[friendlyColourIndex];

			for (int i = 0; i < myKnights.Count; i++)
			{
				int startSquare = myKnights[i];

				for (int knightMoveIndex = 0; knightMoveIndex < PrecomputedMoveData.knightMoves[startSquare].Length; knightMoveIndex++)
				{
					int targetSquare = PrecomputedMoveData.knightMoves[startSquare][knightMoveIndex];
					int targetSquarePiece = board.Square[targetSquare];
					bool isCapture = BitPiece.IsColour(targetSquarePiece, opponentColour);
					
						moves.Add(new BitMove(startSquare, targetSquare));
					
				}
			}
		}

		void GeneratePawnMoves()
		{
			PieceList myPawns = board.pawns[friendlyColourIndex];
			int pawnOffset = (friendlyColour == BitPiece.White) ? -8 : 8;
			int startRank = (board.WhiteToMove) ? 1 : 6;



			for (int i = 0; i < myPawns.Count; i++)
			{
				int startSquare = myPawns[i];
				int rank = BoardRepresentation.RankIndex(startSquare);

				

					int squareOneForward = startSquare + pawnOffset;

					// Square ahead of pawn is empty: forward moves
					if (squareOneForward < 64 && squareOneForward > 0 && board.Square[squareOneForward] == BitPiece.None)
					{
						// Pawn not pinned, or is mov	ing along line of pin

						moves.Add(new BitMove(startSquare, squareOneForward));


						// Is on starting square (so can move two forward if not blocked)
						if (rank == startRank)
						{
							int squareTwoForward = squareOneForward + pawnOffset;
							if (squareTwoForward < 64 && squareTwoForward > 0 && board.Square[squareTwoForward] == BitPiece.None)
							{
								// Not in check, or pawn is interposing checking piece

								moves.Add(new BitMove(startSquare, squareTwoForward, BitMove.Flag.PawnTwoForward));

							}
						}

					
				}

				// Pawn captures.
				for (int j = 0; j < 2; j++)
				{
					// Check if square exists diagonal to pawn
					if (PrecomputedMoveData.numSquaresToEdge[startSquare][PrecomputedMoveData.pawnAttackDirections[friendlyColourIndex][j]] > 0)
					{
						// move in direction friendly pawns attack to get square from which enemy pawn would attack
						int pawnCaptureDir = PrecomputedMoveData.directionOffsets[PrecomputedMoveData.pawnAttackDirections[friendlyColourIndex][j]];
						int targetSquare = startSquare + pawnCaptureDir;
						int targetPiece = board.Square[targetSquare];

						// Regular capture
						if (BitPiece.IsColour(targetPiece, opponentColour))
						{


							moves.Add(new BitMove(startSquare, targetSquare));

						}


					}
				}
			}
		}		
	}
}

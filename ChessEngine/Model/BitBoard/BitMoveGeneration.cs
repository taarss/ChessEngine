using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Model.BitBoard
{
    public class BitMoveGeneration
    {
		List<BitMove> moves;
		bool isWhiteToMove;
		int friendlyColour;
		int opponentColour;
		int friendlyColourIndex;
		int opponentColourIndex;
		BitBoard board;

		List<int> myPawns = new();
		List<int> myKnights = new();
		List<int> myBishops = new();
		List<int> myRooks = new();
		List<int> myQueens = new();
		int myKing;


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
            if (friendlyColour == 0)
            {
				friendlyColour = 8;
            }
            else if (friendlyColour == 1)
            {
				friendlyColour = 16;

			}
			int King = 1 + friendlyColour;
			int Pawn = 2 + friendlyColour;
			int Knight = 3 + friendlyColour;
			int Bishop = 4 + friendlyColour;
			int Rook = 5 + friendlyColour;
			int Queen = 6 + friendlyColour;
			for (int i = 0; i < board.Square.Length; i++)
			{
				if (board.Square[i] != 0)
				{
					if (board.Square[i] == King)
					{
						myKing = i;
					}
					if (board.Square[i] == Pawn)
					{
						myPawns.Add(i);
					}
					if (board.Square[i] == Knight)
					{
						myKnights.Add(i);
					}
					if (board.Square[i] == Bishop)
					{
						myBishops.Add(i);
					}
					if (board.Square[i] == Rook)
					{
						myRooks.Add(i);
					}
					if (board.Square[i] == Queen)
					{
						myQueens.Add(i);
					}
				}

			}
			moves = new List<BitMove>(64);
			isWhiteToMove = board.ColourToMove == BitPiece.White;
			friendlyColour = board.ColourToMove;
			opponentColour = board.OpponentColour;
			friendlyColourIndex = (board.WhiteToMove) ? BitBoard.WhiteIndex : BitBoard.BlackIndex;
			opponentColourIndex = 1 - friendlyColourIndex;
		}

		void GenerateKingMoves()
		{
			for (int i = 0; i < PrecomputedMoveData.kingMoves[myKing].Length; i++)
			{
				int targetSquare = PrecomputedMoveData.kingMoves[myKing][i];
				int pieceOnTargetSquare = board.Square[targetSquare];

				// Skip squares occupied by friendly pieces
				if (BitPiece.IsColour(pieceOnTargetSquare, friendlyColour))
				{
					continue;
				}
				// Safe for king to move to this square

				moves.Add(new BitMove(myKing, targetSquare));


			}
		}

		void GenerateSlidingMoves()
		{
			for (int i = 0; i < myRooks.Count; i++)
			{
				GenerateSlidingPieceMoves(myRooks.ElementAt(i) , 0, 4);
			}

			for (int i = 0; i < myBishops.Count; i++)
			{
				GenerateSlidingPieceMoves(myBishops.ElementAt(i), 4, 8);
			}
			for (int i = 0; i < myQueens.Count; i++)
			{
				GenerateSlidingPieceMoves(myQueens.ElementAt(i), 0, 8);
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
					moves.Add(new BitMove(startSquare, targetSquare));


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
			for (int i = 0; i < myKnights.Count; i++)
			{
				int startSquare = myKnights.ElementAt(i);

				for (int knightMoveIndex = 0; knightMoveIndex < PrecomputedMoveData.knightMoves[startSquare].Length; knightMoveIndex++)
				{
					int targetSquare = PrecomputedMoveData.knightMoves[startSquare][knightMoveIndex];
					int targetSquarePiece = board.Square[targetSquare];
                    if (targetSquarePiece != 0)
                    {
                        if (BitPiece.IsColour(targetSquarePiece, opponentColour))
                        {
							moves.Add(new BitMove(startSquare, targetSquare));
						}
					}
                    else
                    {
						moves.Add(new BitMove(startSquare, targetSquare));
					}

				}
			}
		}

		void GeneratePawnMoves()
		{
			int pawnOffset = (friendlyColour == BitPiece.White) ? -8 : 8;
			int startRank = (board.WhiteToMove) ? 6 : 1;



			for (int i = 0; i < myPawns.Count; i++)
			{
				int startSquare = myPawns.ElementAt(i);
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

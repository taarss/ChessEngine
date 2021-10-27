using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Model.BitBoard;

namespace ChessEngine.Model.BitBoard
{
    public class BitBoard
    {
        public int[] Square;

        public const int WhiteIndex = 0;
        public const int BlackIndex = 1;

        public bool WhiteToMove;
        public int ColourToMove;
        public int OpponentColour;
        public int ColourToMoveIndex;

        // Bits 0-3 store white and black kingside/queenside castling legality
        // Bits 4-7 store file of ep square (starting at 1, so 0 = no ep square)
        // Bits 8-13 captured piece
        // Bits 14-... fifty mover counter
        Stack<uint> gameStateHistory;
        public uint currentGameState;

        public int plyCount; // Total plies played in game
        public int fiftyMoveCounter; // Num ply since last pawn move or capture

        public int[] KingSquare; // index of square of white and black king

        public PieceList[] rooks;
        public PieceList[] bishops;
        public PieceList[] queens;
        public PieceList[] knights;
        public PieceList[] pawns;

		PieceList[] allPieceLists;

        const uint whiteCastleKingsideMask = 0b1111111111111110;
        const uint whiteCastleQueensideMask = 0b1111111111111101;
        const uint blackCastleKingsideMask = 0b1111111111111011;
        const uint blackCastleQueensideMask = 0b1111111111110111;

        const uint whiteCastleMask = whiteCastleKingsideMask & whiteCastleQueensideMask;
        const uint blackCastleMask = blackCastleKingsideMask & blackCastleQueensideMask;


        public BitBoard()
        {
            Square = new int[64];
            Square[0] = BitPiece.White | BitPiece.Bishop;
        }

		PieceList GetPieceList(int pieceType, int colourIndex)
		{
			return allPieceLists[colourIndex * 8 + pieceType];
		}

		public void MakeMove(BitMove move)
		{
			uint oldEnPassantFile = (currentGameState >> 4) & 15;
			uint originalCastleState = currentGameState & 15;
			uint newCastleState = originalCastleState;
			currentGameState = 0;

			int opponentColourIndex = 1 - ColourToMoveIndex;
			int moveFrom = move.StartSquare;
			int moveTo = move.TargetSquare;

			int capturedPieceType = BitPiece.PieceType(Square[moveTo]);
			int movePiece = Square[moveFrom];
			int movePieceType = BitPiece.PieceType(movePiece);

			int moveFlag = move.MoveFlag;
			bool isPromotion = move.IsPromotion;
			bool isEnPassant = moveFlag == BitMove.Flag.EnPassantCapture;

			// Handle captures
			currentGameState |= (ushort)(capturedPieceType << 8);
			

			// Move pieces in piece lists
			if (movePieceType == BitPiece.King)
			{
				KingSquare[ColourToMoveIndex] = moveTo;
				newCastleState &= (WhiteToMove) ? whiteCastleMask : blackCastleMask;
			}
			else
			{
				GetPieceList(movePieceType, ColourToMoveIndex).MovePiece(moveFrom, moveTo);
			}

			int pieceOnTargetSquare = movePiece;

			
				// Handle other special moves (en-passant, and castling)
				switch (moveFlag)
				{
					case BitMove.Flag.EnPassantCapture:
						int epPawnSquare = moveTo + ((ColourToMove == BitPiece.White) ? -8 : 8);
						currentGameState |= (ushort)(Square[epPawnSquare] << 8); // add pawn as capture type
						Square[epPawnSquare] = 0; // clear ep capture square
						pawns[opponentColourIndex].RemovePieceAtSquare(epPawnSquare);
						break;
					case BitMove.Flag.Castling:
						bool kingside = moveTo == BoardRepresentation.g1 || moveTo == BoardRepresentation.g8;
						int castlingRookFromIndex = (kingside) ? moveTo + 1 : moveTo - 2;
						int castlingRookToIndex = (kingside) ? moveTo - 1 : moveTo + 1;

						Square[castlingRookFromIndex] = BitPiece.None;
						Square[castlingRookToIndex] = BitPiece.Rook | ColourToMove;

						rooks[ColourToMoveIndex].MovePiece(castlingRookFromIndex, castlingRookToIndex);						
						break;
				}
			

			// Update the board representation:
			Square[moveTo] = pieceOnTargetSquare;
			Square[moveFrom] = 0;

			// Pawn has moved two forwards, mark file with en-passant flag
			if (moveFlag == BitMove.Flag.PawnTwoForward)
			{
				int file = BoardRepresentation.FileIndex(moveFrom) + 1;
				currentGameState |= (ushort)(file << 4);
			}

			// Piece moving to/from rook square removes castling right for that side
			if (originalCastleState != 0)
			{
				if (moveTo == BoardRepresentation.h1 || moveFrom == BoardRepresentation.h1)
				{
					newCastleState &= whiteCastleKingsideMask;
				}
				else if (moveTo == BoardRepresentation.a1 || moveFrom == BoardRepresentation.a1)
				{
					newCastleState &= whiteCastleQueensideMask;
				}
				if (moveTo == BoardRepresentation.h8 || moveFrom == BoardRepresentation.h8)
				{
					newCastleState &= blackCastleKingsideMask;
				}
				else if (moveTo == BoardRepresentation.a8 || moveFrom == BoardRepresentation.a8)
				{
					newCastleState &= blackCastleQueensideMask;
				}
			}

			
			currentGameState |= newCastleState;
			currentGameState |= (uint)fiftyMoveCounter << 14;
			gameStateHistory.Push(currentGameState);

			// Change side to move
			WhiteToMove = !WhiteToMove;
			ColourToMove = (WhiteToMove) ? BitPiece.White : BitPiece.Black;
			OpponentColour = (WhiteToMove) ? BitPiece.Black : BitPiece.White;
			ColourToMoveIndex = 1 - ColourToMoveIndex;
			plyCount++;
			fiftyMoveCounter++;			
		}

		public void LoadStartPosition()
		{
			LoadPosition(Fen.startFen);
		}

		// Load custom position from fen string
		public void LoadPosition(string fen)
		{
			Initialize();
			var loadedPosition = Fen.PositionFromFen(fen);

			// Load pieces into board array and piece lists
			for (int squareIndex = 0; squareIndex < 64; squareIndex++)
			{
				int piece = loadedPosition.squares[squareIndex];
				Square[squareIndex] = piece;

				if (piece != BitPiece.None)
				{
					int pieceType = BitPiece.PieceType(piece);
					int pieceColourIndex = (BitPiece.IsColour(piece, BitPiece.White)) ? WhiteIndex : BlackIndex;
					if (BitPiece.IsSlidingPiece(piece))
					{
						if (pieceType == BitPiece.Queen)
						{
							queens[pieceColourIndex].AddPieceAtSquare(squareIndex);
						}
						else if (pieceType == BitPiece.Rook)
						{
							rooks[pieceColourIndex].AddPieceAtSquare(squareIndex);
						}
						else if (pieceType == BitPiece.Bishop)
						{
							bishops[pieceColourIndex].AddPieceAtSquare(squareIndex);
						}
					}
					else if (pieceType == BitPiece.Knight)
					{
						knights[pieceColourIndex].AddPieceAtSquare(squareIndex);
					}
					else if (pieceType == BitPiece.Pawn)
					{
						pawns[pieceColourIndex].AddPieceAtSquare(squareIndex);
					}
					else if (pieceType == BitPiece.King)
					{
						KingSquare[pieceColourIndex] = squareIndex;
					}
				}
			}

			// Side to move
			WhiteToMove = loadedPosition.whiteToMove;
			ColourToMove = (WhiteToMove) ? BitPiece.White : BitPiece.Black;
			OpponentColour = (WhiteToMove) ? BitPiece.Black : BitPiece.White;
			ColourToMoveIndex = (WhiteToMove) ? 0 : 1;

			// Create gamestate
			int whiteCastle = ((loadedPosition.whiteCastleKingside) ? 1 << 0 : 0) | ((loadedPosition.whiteCastleQueenside) ? 1 << 1 : 0);
			int blackCastle = ((loadedPosition.blackCastleKingside) ? 1 << 2 : 0) | ((loadedPosition.blackCastleQueenside) ? 1 << 3 : 0);
			int epState = loadedPosition.epFile << 4;
			ushort initialGameState = (ushort)(whiteCastle | blackCastle | epState);
			gameStateHistory.Push(initialGameState);
			currentGameState = initialGameState;
			plyCount = loadedPosition.plyCount;

		}


		void Initialize()
		{
			Square = new int[64];
			KingSquare = new int[2];

			gameStateHistory = new Stack<uint>();
			plyCount = 0;
			fiftyMoveCounter = 0;

			knights = new PieceList[] { new PieceList(10), new PieceList(10) };
			pawns = new PieceList[] { new PieceList(8), new PieceList(8) };
			rooks = new PieceList[] { new PieceList(10), new PieceList(10) };
			bishops = new PieceList[] { new PieceList(10), new PieceList(10) };
			queens = new PieceList[] { new PieceList(9), new PieceList(9) };
			PieceList emptyList = new PieceList(0);
			allPieceLists = new PieceList[] {
				emptyList,
				emptyList,
				pawns[WhiteIndex],
				knights[WhiteIndex],
				emptyList,
				bishops[WhiteIndex],
				rooks[WhiteIndex],
				queens[WhiteIndex],
				emptyList,
				emptyList,
				pawns[BlackIndex],
				knights[BlackIndex],
				emptyList,
				bishops[BlackIndex],
				rooks[BlackIndex],
				queens[BlackIndex],
			};
		}
	}
}

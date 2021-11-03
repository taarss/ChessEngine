using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Model.BitBoard;

namespace ChessEngine.Model.BitBoard
{
    public class BitBoard : INotifyPropertyChanged
    {
        public int[] Square;
        public const int WhiteIndex = 0;
        public const int BlackIndex = 1;

        private bool whiteToMove;
        public int ColourToMove;
        public int ColourToMoveIndex;

        // Bits 0-3 store white and black kingside/queenside castling legality
        // Bits 4-7 store file of ep square (starting at 1, so 0 = no ep square)
        // Bits 8-13 captured piece
        // Bits 14-... fifty mover counter
        Stack<uint> gameStateHistory;
        public uint currentGameState;

        public int plyCount; // Total plies played in game
        public int fiftyMoveCounter; // Num ply since last pawn move or capture
		public int OpponentColour;

        const uint whiteCastleKingsideMask = 0b1111111111111110;
        const uint whiteCastleQueensideMask = 0b1111111111111101;
        const uint blackCastleKingsideMask = 0b1111111111111011;
        const uint blackCastleQueensideMask = 0b1111111111110111;

        const uint whiteCastleMask = whiteCastleKingsideMask & whiteCastleQueensideMask;
        const uint blackCastleMask = blackCastleKingsideMask & blackCastleQueensideMask;

        public bool WhiteToMove { get => whiteToMove; set
			{
				whiteToMove = value;
				RaisePropertyChanged("WhiteToMove");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChanged(string property)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
		}
		public BitBoard()
        {
            Square = new int[64];
        }
		public BitBoard(BitBoard bitBoard)
        {
			Square = new int[64];
			Initialize();
			bitBoard.Square.CopyTo(Square, 0);
			WhiteToMove = bitBoard.WhiteToMove;
			ColourToMove = bitBoard.ColourToMove;
			ColourToMoveIndex = bitBoard.ColourToMoveIndex;
			gameStateHistory = bitBoard.gameStateHistory;
			currentGameState = bitBoard.currentGameState;
			plyCount = bitBoard.plyCount;
			fiftyMoveCounter = bitBoard.fiftyMoveCounter;
			
		}

		


		public void LoadStartPosition()
		{
			LoadPosition(Fen.startFen);
		}

		public void SwitchTurn()
        {
			WhiteToMove = !WhiteToMove; // false
			ColourToMove = (WhiteToMove) ? BitPiece.White : BitPiece.Black; // white = 8 black = 16
			ColourToMoveIndex = 1 - ColourToMoveIndex; // 0 = white 1 = black
		}

		public void ChangeTurnToWhite(bool turn)
        {
			WhiteToMove = turn;
			ColourToMove = (turn) ? BitPiece.White : BitPiece.Black; // white = 8 black = 16
			ColourToMoveIndex = (turn) ? 0 : 1;
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
			}

			// Side to move
			WhiteToMove = loadedPosition.whiteToMove;
			ColourToMove = (WhiteToMove) ? BitPiece.White : BitPiece.Black;
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
			gameStateHistory = new Stack<uint>();
			plyCount = 0;
			fiftyMoveCounter = 0;
		}
		public void MakeMove(BitMove move)
        {
			uint originalCastleState = currentGameState & 15;
			uint newCastleState = originalCastleState;
			currentGameState = 0;
			int moveFrom = move.StartSquare;
			int moveTo = move.TargetSquare;
			int capturedPieceType = Square[moveTo];
			int opponentColourIndex = 1 - ColourToMoveIndex;
			int moveFlag = move.MoveFlag;
			int movePiece = Square[moveFrom];

			currentGameState |= (ushort)(capturedPieceType << 8);
			int pieceColourIndex = (BitPiece.IsColour(movePiece, BitPiece.White)) ? WhiteIndex : BlackIndex;

            if (move.MoveFlag == BitMove.Flag.Castling)
            {
				bool kingside = moveTo == BoardRepresentation.g1 || moveTo == BoardRepresentation.g8;
				int castlingRookFromIndex = (kingside) ? moveTo + 1 : moveTo - 2;
				int castlingRookToIndex = (kingside) ? moveTo - 1 : moveTo + 1;
				Square[castlingRookFromIndex] = BitPiece.None;
				Square[castlingRookToIndex] = BitPiece.Rook | ColourToMove;
			}

			// Update the board representation:
			Square[moveTo] = movePiece;
			Square[moveFrom] = 0;

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


				currentGameState |= newCastleState;
				currentGameState |= (uint)fiftyMoveCounter << 14;
				gameStateHistory.Push(currentGameState);

				// Change side to move
				SwitchTurn();
				plyCount++;
				fiftyMoveCounter++;
			}
		}
		public void UnmakeMove(BitMove move)
        {
			uint originalCastleState = currentGameState & 0b1111;
			int capturedPieceType = ((int)currentGameState >> 8) & 63;
			int capturedPiece = (capturedPieceType == 0) ? 0 : capturedPieceType;

			int movedFrom = move.StartSquare;
			int movedTo = move.TargetSquare;
			int moveFlags = move.MoveFlag;

			bool isEnPassant = moveFlags == BitMove.Flag.EnPassantCapture;
			bool isPromotion = move.IsPromotion;
			int toSquarePieceType = Square[movedTo];
			int movedPieceType = (isPromotion) ? BitPiece.Pawn : toSquarePieceType;
			ChangeTurnToWhite(BitPiece.IsColour(Square[movedTo], 8));

			uint oldEnPassantFile = (currentGameState >> 4) & 15;

			// put back moved piece
			Square[movedFrom] = movedPieceType; // note that if move was a pawn promotion, this will put the promoted piece back instead of the pawn. Handled in special move switch
			Square[movedTo] = capturedPiece; // will be 0 if no piece was captured
			if (moveFlags == BitMove.Flag.Castling)
			{ // castles: move rook back to starting square

				bool kingside = movedTo == 6 || movedTo == 62;
				int castlingRookFromIndex = (kingside) ? movedTo + 1 : movedTo - 2;
				int castlingRookToIndex = (kingside) ? movedTo - 1 : movedTo + 1;

				Square[castlingRookToIndex] = 0;
				Square[castlingRookFromIndex] = BitPiece.Rook | ColourToMove;

			}
			gameStateHistory.Pop(); // removes current state from history
			currentGameState = gameStateHistory.Peek(); // sets current state to previous state in history

			fiftyMoveCounter = (int)(currentGameState & 4294950912) >> 14;
			int newEnPassantFile = (int)(currentGameState >> 4) & 15;

			uint newCastleState = currentGameState & 0b1111;
			plyCount--;
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using ChessEngine;
using ChessEngine.Model.BitBoard;
using ChessEngine.ViewModel;

namespace ChessEngine
{
	public class Evaluation
	{

		public const int pawnValue = 100;
		public const int knightValue = 300;
		public const int bishopValue = 320;
		public const int rookValue = 500;
		public const int queenValue = 900;

		BitBoard board;

		// Performs static evaluation of the current position.
		// The position is assumed to be 'quiet', i.e no captures are available that could drastically affect the evaluation.
		// The score that's returned is given from the perspective of whoever's turn it is to move.
		// So a positive score means the player who's turn it is to move has an advantage, while a negative score indicates a disadvantage.
		public int Evaluate(BitBoard board)
		{
			this.board = board;
			int whiteEval = 0;
			int blackEval = 0;

			int whiteMaterial = CountMaterial(8);
			int blackMaterial = CountMaterial(16);
			whiteEval += whiteMaterial;
			blackEval += blackMaterial;


			int eval = whiteEval - blackEval;

			int perspective = (board.WhiteToMove) ? 1 : -1;
			return eval * perspective;
		}

		


		int CountMaterial(int colour)
		{
			BoardViewModel board = (BoardViewModel)App.Current.Resources["boardViewModel"];
			int material = 0;
			material += board.GetAllTypePieces(2 + colour).Count * pawnValue;
			material += board.GetAllTypePieces(3 + colour).Count * knightValue;
			material += board.GetAllTypePieces(4 + colour).Count * bishopValue;
			material += board.GetAllTypePieces(5 + colour).Count * rookValue;
			material += board.GetAllTypePieces(6 + colour).Count * queenValue;

			return material;
		}		
	}
}
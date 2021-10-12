using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Model;
using ChessEngine.ViewModel;

namespace ChessEngine.Model.AI
{
    public class Evaluate
    {
        const int pawnValue = 100;
        const int knightValue = 300;
        const int bishopValue = 300;
        const int rookValue = 500;
        const int queenValue = 900;

        public static int EvaluateMaterial()
        {
            BoardViewModel board = (BoardViewModel)App.Current.Resources["boardViewModel"];
            int whiteEval = CountMaterial(true);
            int blackEval = CountMaterial(false);

            int evaluation = whiteEval - blackEval;
            int perspective = board.IsWhitesTurn ? 1 : -1;
            return evaluation * perspective;
        }

        private static int CountMaterial(bool isWhite)
        {
            BoardViewModel board = (BoardViewModel)App.Current.Resources["boardViewModel"];
            int material = 0;
            material += board.GetAllTypePieces(new Piece.Piece("Pawn", isWhite)).Count * pawnValue;
            material += board.GetAllTypePieces(new Piece.Piece("Knight", isWhite)).Count * knightValue;
            material += board.GetAllTypePieces(new Piece.Piece("Bishop", isWhite)).Count * bishopValue;
            material += board.GetAllTypePieces(new Piece.Piece("Rook", isWhite)).Count * rookValue;
            material += board.GetAllTypePieces(new Piece.Piece("Queen", isWhite)).Count * queenValue;
            return material;
        }
    }
}

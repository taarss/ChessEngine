using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Model.Piece;
using ChessEngine.ViewModel;

namespace ChessEngine.Model
{
    public class MoveLogic
    {
        List<Move> moves;

        public List<Move> GenerateMoves()
        {
            moves = new List<Move>();
            for (int startSquare = 0; startSquare < 64; startSquare++)
            {
                BoardViewModel boardViewModel = (BoardViewModel)App.Current.Resources["boardViewModel"];
                Piece.Piece piece = boardViewModel.TheGrid[startSquare].piece;
                if (piece.IsWhite == boardViewModel.IsWhitesTurn)
                {
                    if (true)
                    {

                    }
                }
            }
        }
    }
}

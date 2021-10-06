using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Model.Piece;
using ChessEngine.Model;
using ChessEngine.ViewModel;

namespace ChessEngine.Model
{
    public class Check
    {
        private BoardViewModel boardViewModel = (BoardViewModel)App.Current.Resources["boardViewModel"];

        public Piece.Piece isInCheck(List<Move> attackList, int kingPosition)
        {
            BoardViewModel temp = (BoardViewModel)App.Current.Resources["boardViewModel"];
            foreach (var attack in attackList)
            {
                if (attack.TargetSquare == kingPosition)
                {
                    return temp.TheGrid[attack.StartSquare].piece;
                }
            }
            return null;
        }

        public int getKing(bool isWhite)
        {
            BoardViewModel temp = (BoardViewModel)App.Current.Resources["boardViewModel"];
            foreach (var item in temp.Pieces)
            {
                if (item.Value.Name == "King" && item.Value.IsWhite != isWhite)
                {
                    return item.Key;
                }
            }
            return -1;
        }

        public List<Move> GenerateLegelMoves(int startindex, Piece.Piece piece)
        {
            BoardViewModel temp = (BoardViewModel)App.Current.Resources["boardViewModel"];
            List<Move> pseudoLegealMoves = temp.MoveLogic.GenerateMoveForPiece(piece, startindex);
            List<Move> legalMoves = new List<Move>();

            foreach (var moveToVerify in pseudoLegealMoves)
            {
                temp.MoveLogic.MakeMove(moveToVerify);
                temp.MoveLogic.SwitchTurn();
                List<Move> opponentResponses = temp.MoveLogic.GenerateMoves();
                bool resetList = false;
                List<Move> tempMoves = new();
                foreach (var item in opponentResponses)
                {
                    int kingIndex = getKing(temp.IsWhitesTurn);
                    if (item.TargetSquare == kingIndex)
                    {
                        resetList = true;
                        break;
                    }
                    else
                    {
                        if(!tempMoves.Contains(moveToVerify))
                            tempMoves.Add(moveToVerify);
                    }
                }
                if (resetList)
                    tempMoves = new();
                legalMoves.AddRange(tempMoves);
                
                if (opponentResponses.Count == 0)
                {
                    legalMoves.Add(moveToVerify);

                }
                temp.MoveLogic.SwitchTurn();
                temp.MoveLogic.UnmakeMove(moveToVerify);
            }
            //temp.MoveLogic.recentCaptures = new();
            return legalMoves;
        }
    }
}

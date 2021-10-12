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

        public static Piece.Piece IsInCheck(List<Move> attackList, int kingPosition)
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

        public static int GetKing(bool isWhite)
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

        public static List<Move> GenerateLegelMoves(int startindex, Piece.Piece piece)
        {
            BoardViewModel temp = (BoardViewModel)App.Current.Resources["boardViewModel"];
            List<Move> pseudoLegealMoves = temp.MoveLogic.GenerateMoveForPiece(piece, startindex);
            List<Move> legalMoves = new();

            foreach (var moveToVerify in pseudoLegealMoves)
            {
                temp.MoveLogic.MakePseudoMove(moveToVerify);
                MoveLogic.SwitchTurn();
                List<Move> opponentResponses = temp.MoveLogic.GenerateMoves();
                bool resetList = false;
                List<Move> tempMoves = new();
                foreach (var item in opponentResponses)
                {
                    int kingIndex = GetKing(temp.IsWhitesTurn);
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
                MoveLogic.SwitchTurn();
                temp.MoveLogic.UnmakeMove(moveToVerify);
            }
            //temp.MoveLogic.recentCaptures = new();
            return legalMoves;
        }

        public static List<Move> GenerateAllLegelMoves()
        {
            BoardViewModel temp = (BoardViewModel)App.Current.Resources["boardViewModel"];
            List<Move> pseudoLegealMoves = temp.MoveLogic.GenerateMoves();
            List<Move> legalMoves = new();

            foreach (var moveToVerify in pseudoLegealMoves)
            {
                temp.MoveLogic.MakePseudoMove(moveToVerify);
                MoveLogic.SwitchTurn();
                List<Move> opponentResponses = temp.MoveLogic.GenerateMoves();
                bool resetList = false;
                List<Move> tempMoves = new();
                foreach (var item in opponentResponses)
                {
                    int kingIndex = GetKing(temp.IsWhitesTurn);
                    if (item.TargetSquare == kingIndex)
                    {
                        resetList = true;
                        break;
                    }
                    else
                    {
                        if (!tempMoves.Contains(moveToVerify))
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
                MoveLogic.SwitchTurn();
                temp.MoveLogic.UnmakeMove(moveToVerify);
            }
            //temp.MoveLogic.recentCaptures = new();
            return legalMoves;
        }

    }
}

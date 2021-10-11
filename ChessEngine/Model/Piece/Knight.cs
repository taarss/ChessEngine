using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Model.Piece;
using ChessEngine.Model;
using ChessEngine.ViewModel;

namespace ChessEngine.Model.Piece
{
    public class Knight
    {
        public List<Move> GenerateKnightMove(int startSquare, Piece knight)
        {
            BoardViewModel board = (BoardViewModel)App.Current.Resources["boardViewModel"];
            List<Move> moves = new();

            //All possible move indexes if the knight is not near any edges
            int[] dirAll = new int[] { -17, -15, -10, -6, 10, 17, 15, 6 };
            //All possible move indexes if the knight on Rank 0
            int[] dirRank0 = new int[] { -6, -15, 10, 17 };
            //All possible move indexes if the knight on Rank 1
            int[] dirRank1 = new int[] { -17, -15, -6, 10, 17, 15 };
            //All possible move indexes if the knight on Rank 7
            int[] dirRank7 = new int[] { 17, -15, -10, -17, 15, 6 };
            //All possible move indexes if the knight on Rank 8
            int[] dirRank8 = new int[] { -17, -10, 15, 6 };

            switch (startSquare % 8)
            {
                case 7:
                    KnightMoveRankChecker(dirRank8, startSquare);
                    break;
                case 6:
                    KnightMoveRankChecker(dirRank7, startSquare);
                    break;
                case 0:
                    KnightMoveRankChecker(dirRank0, startSquare);
                    break;
                case 1:
                    KnightMoveRankChecker(dirRank1, startSquare);
                    break;
                default:
                    KnightMoveRankChecker(dirAll, startSquare);
                    break;
            }
        }

        private List<Move>[] KnightMoveRankChecker(int[] dir, int startSquare)
        {
            BoardViewModel board = (BoardViewModel)App.Current.Resources["boardViewModel"];
            List<Move> moves = new();
            List<Move> attackMap = new();
            
            foreach (var item in dir)
            {
                if (startSquare + item >= 0 && startSquare + item < 64)
                {
                    if (board.TheGrid[startSquare + item].piece == null)
                    {
                        moves.Add(new Move(startSquare, startSquare + item));
                        attackMap.Add(new Move(startSquare, startSquare + item));
                    }
                    else
                    {
                        if (board.TheGrid[startSquare + item].piece.IsWhite != board.IsWhitesTurn)
                        {
                            moves.Add(new Move(startSquare, startSquare + item));
                        }
                        attackMap.Add(new Move(startSquare, startSquare + item));

                    }
                }
            }
        }
    }
}

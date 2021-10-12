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
        //All possible move indexes if the knight is not near any edges
        private readonly int[] dirAll = new int[] { -17, -15, -10, -6, 10, 17, 15, 6 };
        //All possible move indexes if the knight on Rank 0
        private readonly int[] dirRank0 = new int[] { -6, -15, 10, 17 };
        //All possible move indexes if the knight on Rank 1
        private readonly int[] dirRank1 = new int[] { -17, -15, -6, 10, 17, 15 };
        //All possible move indexes if the knight on Rank 7
        private readonly int[] dirRank7 = new int[] { 17, -15, -10, -17, 15, 6 };
        //All possible move indexes if the knight on Rank 8
        private readonly int[] dirRank8 = new int[] { -17, -10, 15, 6 };


        public List<Move> GenerateKnightMove(int startSquare)
        {
            BoardViewModel board = (BoardViewModel)App.Current.Resources["boardViewModel"];
            List<Move> moves = new();

            moves = (startSquare % 8) switch
            {
                7 => KnightMoveRankChecker(dirRank8, startSquare),
                6 => KnightMoveRankChecker(dirRank7, startSquare),
                0 => KnightMoveRankChecker(dirRank0, startSquare),
                1 => KnightMoveRankChecker(dirRank1, startSquare),
                _ => KnightMoveRankChecker(dirAll, startSquare),
            };
            return moves;
        }

        private static List<Move> KnightMoveRankChecker(int[] dir, int startSquare)
        {
            BoardViewModel board = (BoardViewModel)App.Current.Resources["boardViewModel"];
            List<Move> moves = new();
            //This is a map of all the squares that the piece attacks
            
            //Loop through possible move indexes
            foreach (var item in dir)
            {
                //Checking that the given index is within the limits of the board
                if (startSquare + item >= 0 && startSquare + item < 64)
                {
                    //If there isn't a piece on the given index
                    if (board.TheGrid[startSquare + item].piece == null)
                    {
                        moves.Add(new Move(startSquare, startSquare + item));
                        board.AttackMap.Add(new Move(startSquare, startSquare + item));
                    }
                    //If there is a piece on said index
                    else
                    {
                        //Check if the piece is not friendly
                        if (board.TheGrid[startSquare + item].piece.IsWhite != board.IsWhitesTurn)
                        {
                            moves.Add(new Move(startSquare, startSquare + item));
                        }
                        //We want to mark the square as attacked regardless
                        board.AttackMap.Add(new Move(startSquare, startSquare + item));

                    }
                }
            }

            return moves;
        }
    }
}

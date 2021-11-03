using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Model.Piece;
using ChessEngine.ViewModel;
namespace ChessEngine.Model.Piece
{
    public class King
    {
        //Possible index moves the king can do if it's on the edges of the board or anywhere else
        private readonly int[] dirRank8 = new int[] { -8, 8, 1, 9, -7 };
        private readonly int[] dirRank0 = new int[] { -8, 8, -1, -9, 7 };
        private readonly int[] dirAll = new int[] { -8, 8, 1, -1, 7, -7, 9, -9 };
        readonly BoardViewModel board = (BoardViewModel)App.Current.Resources["boardViewModel"];



        public List<Move> GenerateKingMove(int startSquare, Piece king)
        {
            List<Move> moves = new();
            moves = (startSquare % 8) switch
            {
                7 => moves = GenerateKingMoveRankChecker(dirRank0, startSquare, king),
                0 => moves = GenerateKingMoveRankChecker(dirRank8, startSquare, king),
                _ => moves = GenerateKingMoveRankChecker(dirAll, startSquare, king),
            };
            return moves;
        }

        private List<Move> GenerateKingMoveRankChecker(int[] dir, int startSquare, Piece king)
        {
            List<Move> moves = new();

            //Loop through possible index moves
            foreach (var item in dir)
            {
                //Check that it's within the board limits
                if (startSquare + item >= 0 && startSquare + item < 64)
                {
                    //If there isn't a piece on the target square
                    if (board.TheGrid[startSquare + item].piece == null)
                    {
                        moves.Add(new Move(startSquare, startSquare + item));
                    }
                    else
                    {
                        //Check if the piece that's on the target square isn't friendly
                        if (board.TheGrid[startSquare + item].piece.IsWhite != board.BitBoard.WhiteToMove)
                        {
                            moves.Add(new Move(startSquare, startSquare + item));
                        }
                        //We want to mark the square as attacked regardless
                        board.AttackMap.Add(new Move(startSquare, startSquare + item));
                    }
                }
            }
            //Check for possible castles
            moves.AddRange(CastleMove(king, startSquare));


            return moves;
        }

        private List<Move> CastleMove(Piece king, int startSquare)
        {
            List<Move> moves = new();
            if (!king.HasMoved && king.Name == "King")
            {
                //Castle king side
                //Since we can only castle if neither the rook nor king has moved
                //we can assume the rook will still be at it's starting position
                Piece kingSideRook = board.TheGrid[startSquare + 3].piece;

                //Check if the piece that was the square really was a rook
                if (kingSideRook != null && kingSideRook.Name == "Rook")
                {
                    //Check that there isn't any pieces blocking the move
                    if (board.TheGrid[startSquare + 1].piece == null && board.TheGrid[startSquare + 2].piece == null)
                    {
                        //Check if the given rook has moved
                        if (!kingSideRook.HasMoved)
                        {
                            moves.Add(new Move(startSquare, startSquare + 2, startSquare + 1, startSquare + 3));
                        }
                    }
                }
                //Castle queen side
                //Same logic ^
                if (startSquare - 4 >= 0)
                {
                    Piece queenSideRook = board.TheGrid[startSquare - 4].piece;
                    if (queenSideRook != null && queenSideRook.Name == "Rook")
                    {
                        if (board.TheGrid[startSquare - 1].piece == null && board.TheGrid[startSquare - 2].piece == null && board.TheGrid[startSquare - 3].piece == null)
                        {
                            if (!queenSideRook.HasMoved)
                            {
                                moves.Add(new Move(startSquare, startSquare - 2, startSquare - 1, startSquare - 4));
                            }
                        }
                    }
                }
                
            }
            return moves;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Model.Piece;
using ChessEngine.ViewModel;

namespace ChessEngine.Model.Piece
{
    public class Pawn
    {
        int[] dirs;
        int[] pushDir; 
        int[] enPassentCheckList;

        public Pawn(bool isWhite)
        {
            dirs = (isWhite) ? new[] { -9, -7 } : new[] { 9, 7 };
            pushDir = (isWhite) ? new[] { -8, -16 } : new[] { 8, 16 };
            enPassentCheckList = new int[] { -1, 1 };
        }

        public List<Move> GeneratePossibleMoves(Piece pawn, int startSquare)
        {
            BoardViewModel board = (BoardViewModel)App.Current.Resources["boardViewModel"];
            List<Move> moves = new();

            //Check if the pawn has moved if not add double push as a possible move
            if (startSquare + pushDir[1] < 64 && !pawn.HasMoved && board.TheGrid[startSquare + pushDir[1]].piece == null)
            {
                moves.Add(new Move(startSquare, startSquare + pushDir[1], true));
            }

            //Check that the pawn is not on the very first or last rank
            //If not, add a normal single push as a possible move
            if (startSquare > 7 && startSquare < 56)
            {
                //Check if the next square is occupied 
                if (board.TheGrid[startSquare + pushDir[0]].piece == null)
                {
                    moves.Add(new Move(startSquare, startSquare + pushDir[0]));
                }
            }
            //Loop through possible En passent moves (if there is a piece in either side of the pawn)
            foreach (var item in enPassentCheckList)
            {
                //Check if there is a piece on either side of the pawn
                if (startSquare + item >= 0 && startSquare + item > 64 && board.TheGrid[startSquare + item].piece != null)
                {
                    //Check if the piece is friendly or hostile
                    if (board.TheGrid[startSquare + item].piece.IsWhite != board.IsWhitesTurn)
                    {
                        //Check if the hostile pawn has made a double pawn push
                        //ERROR: this method does not check if the last move it made was a double pawn push
                        if (board.TheGrid[startSquare + item].piece.HasDoublePushed)
                        {
                            moves.Add(new Move(startSquare, (startSquare + item) + pushDir[0], startSquare + item));
                        }
                    }
                }
            }



            //This codes purpose is to check for possible captures the given pawn can make
            //No captures possible if it already is on the first/last rank i.e promote (not implemented)
            if (startSquare > 7)
            {
                //Checks if the pawns position is on the edges of the board
                //Otherwise the possible captures would overflow to another file
                if (startSquare % 8 == 0)
                {
                    //Check if there is a piece and it's not friendly
                    if (board.TheGrid[startSquare + dirs[1]].piece != null && board.TheGrid[startSquare + dirs[1]].piece.IsWhite != pawn.IsWhite)
                    {
                        moves.Add(new Move(startSquare, startSquare + dirs[1]));
                    }
                }
                else if (startSquare % 8 == 7)
                {
                    //Check if there is a piece and it's not friendly
                    if (board.TheGrid[startSquare + dirs[0]].piece != null && board.TheGrid[startSquare + dirs[0]].piece.IsWhite != pawn.IsWhite)
                    {
                        moves.Add(new Move(startSquare, startSquare + dirs[0]));
                    }
                }
                else
                {
                    //The pawn is not on either of the edges
                    foreach (var item in dirs)
                    {
                        if (startSquare + item < 64 && startSquare + item > 0 &&  board.TheGrid[startSquare + item].piece != null && board.TheGrid[startSquare + item].piece.IsWhite != pawn.IsWhite)
                        {
                            moves.Add(new Move(startSquare, startSquare + item));
                            board.AttackMap.Add(new Move(startSquare, startSquare + item));
                        }
                        board.AttackMap.Add(new Move(startSquare, startSquare + item));
                    }
                }
            }
            return moves;
        }

    }
}

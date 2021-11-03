using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Model;
using ChessEngine.ViewModel;

namespace ChessEngine.Model.Piece
{
    public class SlidePiece
    {
        private int[][] NumSquaresToEdge = new int[64][];
        private static readonly int[] DirectionOffsets = { 8, -8, -1, 1, 7, -7, 9, -9 };
        private BoardViewModel board = (BoardViewModel)App.Current.Resources["boardViewModel"];

        //This class generates moves for all slideing pieces(Queen, rook, bishop)
        public SlidePiece()
        {
            //Data used to determine where the edge of the board is
            PrecomputedMoveData();
        }

        public List<Move> GenerateSlidingMoves(int startSquare, Piece piece)
        {
            int startDirIndex = (piece.Name == "Bishop") ? 4 : 0;
            int endDirIndex = (piece.Name == "Rook") ? 4 : 8;
            List<Move> moves = new();


            for (int directionIndex = startDirIndex; directionIndex < endDirIndex; directionIndex++)
            {
                for (int n = 0; n < NumSquaresToEdge[startSquare][directionIndex]; n++)
                {
                    int targetSquare = startSquare + DirectionOffsets[directionIndex] * (n + 1);
                    Piece pieceOnTargetSquare = board.TheGrid[targetSquare].piece;

                    //Blocked my friendly piece
                    if (pieceOnTargetSquare != null)
                    {
                        if (pieceOnTargetSquare.IsWhite == board.BitBoard.WhiteToMove)
                        {
                            board.AttackMap.Add(new Move(startSquare, targetSquare));
                            break;
                        }
                    }
                    board.AttackMap.Add(new Move(startSquare, targetSquare));
                    moves.Add(new Move(startSquare, targetSquare));


                    //Can't move further in this direction after capturing opponents piece
                    if (pieceOnTargetSquare != null)
                    {
                        if (pieceOnTargetSquare.IsWhite != board.BitBoard.WhiteToMove)
                        {
                            board.AttackMap.Add(new Move(startSquare, targetSquare));
                            break;
                        }
                    }

                }
            }
            return moves;
        }


        private void PrecomputedMoveData()
        {
            for (int file = 0; file < 8; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    int numNorth = 7 - rank;
                    int numSouth = rank;
                    int numWest = file;
                    int numEast = 7 - file;

                    int squareIndex = rank * 8 + file;

                    NumSquaresToEdge[squareIndex] = new int[8];
                    NumSquaresToEdge[squareIndex][0] = numNorth;
                    NumSquaresToEdge[squareIndex][1] = numSouth;
                    NumSquaresToEdge[squareIndex][2] = numWest;
                    NumSquaresToEdge[squareIndex][3] = numEast;
                    NumSquaresToEdge[squareIndex][4] = System.Math.Min(numNorth, numWest);
                    NumSquaresToEdge[squareIndex][5] = System.Math.Min(numSouth, numEast);
                    NumSquaresToEdge[squareIndex][6] = System.Math.Min(numNorth, numEast);
                    NumSquaresToEdge[squareIndex][7] = System.Math.Min(numSouth, numWest);


                }
            }
        }

    }
}

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
        public static readonly int[] DirectionOffsets = { 8, -8, -1, 1, 7, -7, 9, -9 };
        public int[][] NumSquaresToEdge = new int[64][];
        private BoardViewModel boardViewModel = (BoardViewModel)App.Current.Resources["boardViewModel"];
        private bool friendlyColor = true;


        public void PrecomputedMoveData()
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

        List<Move> moves;





        public List<Move> GenerateMoves()
        {
            moves = new List<Move>();
            for (int startSquare = 0; startSquare < 64; startSquare++)
            {
                Piece.Piece piece = boardViewModel.TheGrid[startSquare].piece;
                if (boardViewModel.TheGrid[startSquare].piece != null)
                {
                    if (piece.IsWhite == boardViewModel.IsWhitesTurn)
                    {
                        if (piece.Name == "Rook" || piece.Name == "Bishop" || piece.Name == "Queen")
                        {
                            GenerateSlidingMoves(startSquare, piece);
                        }
                    }
                }
                         
            }
            return moves;
        }


        public List<Move> GenerateMoveForPiece(Piece.Piece piece, int startingPosition)
        {
            moves = new List<Move>();
            if (piece != null)
            {
                if (piece.IsWhite == boardViewModel.IsWhitesTurn)
                {
                    if (piece.Name == "Rook" || piece.Name == "Bishop" || piece.Name == "Queen")
                    {
                        GenerateSlidingMoves(startingPosition, piece);                       
                    }
                    else if (piece.Name == "Pawn")
                    {
                        GeneratePawnMove(startingPosition, piece);
                    }
                }
            }
            return moves;
        }

        private void GeneratePawnMove(int startSquare, Piece.Piece piece)
        {
            if (piece.IsWhite)
            {
                if (!piece.HasMoved)
                {
                    moves.Add(new Move(startSquare, startSquare - 16));
                }
                moves.Add(new Move(startSquare, startSquare - 8));              
            }
            else
            {
                int moveTo = piece.HasMoved ? startSquare + 8 : startSquare + 16;
                moves.Add(new Move(startSquare, moveTo));
            }
        }


        private void GenerateSlidingMoves(int startSquare, Piece.Piece piece)
        {
            int startDirIndex = (piece.Name == "Bishop") ? 4 : 0;
            int endDirIndex = (piece.Name == "Rook") ? 4 : 8;
            for (int directionIndex = startDirIndex; directionIndex < endDirIndex; directionIndex++)
            {
                for (int n = 0; n < NumSquaresToEdge[startSquare][directionIndex]; n++)
                {
                    int targetSquare = startSquare + DirectionOffsets[directionIndex] * (n + 1);
                    Piece.Piece pieceOnTargetSquare = boardViewModel.TheGrid[targetSquare].piece;

                    //Blocked my friendly piece
                    if (pieceOnTargetSquare != null)
                    {
                        if (pieceOnTargetSquare.IsWhite == friendlyColor)
                        {
                            break;
                        }
                    }                   
                    moves.Add(new Move(startSquare, targetSquare));

                    if (pieceOnTargetSquare != null)
                    {
                        if (pieceOnTargetSquare.IsWhite == false)
                        {
                            break;
                        }
                    }
                    
                }
            }
        }
    }
}

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
        public List<Move> AttackMap = new();

        public void MakeMove(int startSquare, int targetSquare, Piece.Piece selectedPiece)
        {
            BoardViewModel temp = (BoardViewModel)App.Current.Resources["boardViewModel"];
            selectedPiece.HasMoved = true;
            temp.TheGrid[targetSquare].piece = selectedPiece;
            temp.TheGrid[startSquare].piece = null;
            temp.Debuger.RecordMove(startSquare, targetSquare, selectedPiece);            
        }

        public void SwitchTurn()
        {
            BoardViewModel temp = (BoardViewModel)App.Current.Resources["boardViewModel"];
            temp.IsWhitesTurn = !temp.IsWhitesTurn;
        }


        public void RemovePieceAtIndex(int index)
        {
            BoardViewModel temp = (BoardViewModel)App.Current.Resources["boardViewModel"];
            temp.TheGrid[index].piece = null;
        }
        
        
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



        public List<Move> GenerateAttackMap()
        {
            moves = new List<Move>();
            boardViewModel = (BoardViewModel)App.Current.Resources["boardViewModel"];
            for (int startSquare = 0; startSquare < 64; startSquare++)
            {
                Piece.Piece piece = boardViewModel.TheGrid[startSquare].piece;
                if (boardViewModel.TheGrid[startSquare].piece != null)
                {
                    if (piece.IsWhite != boardViewModel.IsWhitesTurn)
                    {
                        if (piece.Name == "Rook" || piece.Name == "Bishop" || piece.Name == "Queen")
                        {
                            //GenerateSlidingMoves(startSquare, piece);
                        }
                        if (piece.Name == "Pawn")
                        {
                           GeneratePawnMove(startSquare, piece);
                        }
                        if (piece.Name == "King")
                        {
                            GenerateKingMove(startSquare, piece);
                        }
                        if (piece.Name == "Knight")
                        {
                            GenerateKnightMove(startSquare, piece);
                        }
                    }
                }
                         
            }
            return AttackMap;
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
                    else if(piece.Name == "Knight")
                    {
                        GenerateKnightMove(startingPosition, piece);
                    }
                    else if (piece.Name == "King")
                    {
                        GenerateKingMove(startingPosition, piece);
                    }
                }
            }
            return moves;
        }

        private void GenerateKingMove(int startSquare, Piece.Piece piece)
        {
            int[] dirRank8 = new int[] { -8, 8, 1, 9, -7 };
            int[] dirRank0 = new int[] { -8, 8, -1, -9, 7 };
            int[] dirAll = new int[] { -8, 8, 1, -1, 7, -7, 9, -9 };
            switch (startSquare % 8)
            {
                case 7:
                    GenerateKingMoveRankChecker(dirRank0, startSquare, piece);
                    break;
                case 0:
                    GenerateKingMoveRankChecker(dirRank8, startSquare, piece);
                    break;

                default:
                    GenerateKingMoveRankChecker(dirAll, startSquare, piece);
                    break;
            }
        }

        private void GenerateKingMoveRankChecker(int[] dir, int startSquare, Piece.Piece king)
        {
            foreach (var item in dir)
            {              
                if (startSquare + item >= 0 && startSquare + item < 64)
                {
                    if (boardViewModel.TheGrid[startSquare + item].piece == null)
                    {
                        moves.Add(new Move(startSquare, startSquare + item));
                    }
                    else
                    {
                        if (boardViewModel.TheGrid[startSquare + item].piece.IsWhite != boardViewModel.IsWhitesTurn)
                        {
                            moves.Add(new Move(startSquare, startSquare + item));
                        }
                        AttackMap.Add(new Move(startSquare, startSquare + item));
                    }
                    AttackMap.Add(new Move(startSquare, startSquare + item));
                }
            }
            CastleMove(king, startSquare);
        }


        private void GenerateKnightMove(int startSquare, Piece.Piece piece)
        {
            int[] dirAll = new int[] { -17, -15, -10, -6, 10, 17, 15, 6 };
            int[] dirRank0 = new int[] { -6, -15, 10, 17 };
            int[] dirRank1 = new int[] { -17, -15, -6, 10, 17, 15 };
            int[] dirRank7 = new int[] { 17, -15, -10,  -17, 15, 6};
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
        private void KnightMoveRankChecker(int[] dir, int startSquare)
        {
            foreach (var item in dir)
            {              
                if (startSquare + item >= 0 && startSquare + item < 64)
                {
                    if (boardViewModel.TheGrid[startSquare + item].piece == null)
                    {
                        moves.Add(new Move(startSquare, startSquare + item));
                        AttackMap.Add(new Move(startSquare, startSquare + item));
                    }
                    else
                    {
                        if (boardViewModel.TheGrid[startSquare + item].piece.IsWhite != boardViewModel.IsWhitesTurn)
                        {
                            moves.Add(new Move(startSquare, startSquare + item));
                            AttackMap.Add(new Move(startSquare, startSquare + item));

                        }
                    }
                }
            }
        }
        private void CastleMove(Piece.Piece king, int startSquare)
        {
            if (!king.HasMoved && king.Name == "King")
            {
                //Castle king side
                Piece.Piece kingSideRook = boardViewModel.TheGrid[startSquare + 3].piece;
                if (kingSideRook != null &&  kingSideRook.Name == "Rook")
                {
                    if (boardViewModel.TheGrid[startSquare + 1].piece == null && boardViewModel.TheGrid[startSquare + 2].piece == null)
                    {
                        if (!kingSideRook.HasMoved)
                        {
                            moves.Add(new Move(startSquare, startSquare + 2, startSquare + 1, startSquare + 3));
                        }
                    }          
                }
                //Castle queen side
                Piece.Piece queenSideRook = boardViewModel.TheGrid[startSquare - 4].piece;
                if (queenSideRook != null && queenSideRook.Name == "Rook")
                {
                    if (boardViewModel.TheGrid[startSquare - 1].piece == null && boardViewModel.TheGrid[startSquare - 2].piece == null && boardViewModel.TheGrid[startSquare - 3].piece == null)
                    {
                        if (!queenSideRook.HasMoved)
                        {
                            moves.Add(new Move(startSquare, startSquare - 2, startSquare - 1, startSquare - 4));
                        }
                    }
                }
            }
        }

        private void GeneratePawnMove(int startSquare, Piece.Piece piece)
        {
            var dirs = (piece.IsWhite) ? new[] { -9, -7 } : new[] { 9, 7 };
            var pushDir = (piece.IsWhite) ? new[] { -8, -16 } : new[] { 8, 16 };
            int[] enPassentCheckList = new int[] { -1, 1 };
            if (!piece.HasMoved && boardViewModel.TheGrid[startSquare + pushDir[1]].piece == null)
            {
                 moves.Add(new Move(startSquare, startSquare + pushDir[1]));
                piece.HasDoublePushed = true;
            }
            if (startSquare > 7)
            {
                if (boardViewModel.TheGrid[startSquare + pushDir[0]].piece == null)
                {
                    moves.Add(new Move(startSquare, startSquare + pushDir[0]));
                }
            }
            foreach (var item in enPassentCheckList)
            {
                if (boardViewModel.TheGrid[startSquare + item].piece != null)
                {
                    if (boardViewModel.TheGrid[startSquare + item].piece.IsWhite != boardViewModel.IsWhitesTurn)
                    {
                        if (boardViewModel.TheGrid[startSquare + item].piece.HasDoublePushed)
                        {
                            moves.Add(new Move(startSquare, (startSquare + item) + pushDir[0], startSquare + item));
                        }
                    }
                }
            }
                       
            foreach (var item in dirs)
            {
                if (startSquare > 7)
                {
                    if (boardViewModel.TheGrid[startSquare + item].piece != null && boardViewModel.TheGrid[startSquare + item].piece.IsWhite != piece.IsWhite)
                    {
                        moves.Add(new Move(startSquare, startSquare + item));
                    }
                    AttackMap.Add(new Move( startSquare, startSquare + item));
                }
                                  
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
                        if (pieceOnTargetSquare.IsWhite == boardViewModel.IsWhitesTurn)
                        {
                            break;
                        }
                    }
                    moves.Add(new Move(startSquare, targetSquare));
                    AttackMap.Add(new Move(startSquare, targetSquare));
                    

                    if (pieceOnTargetSquare != null)
                    {
                        if (pieceOnTargetSquare.IsWhite == boardViewModel.IsWhitesTurn)
                        {
                            break;
                        }
                    }
                    
                }
            }
        }
    }
}

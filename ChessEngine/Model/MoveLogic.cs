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
        public BoardViewModel temp = (BoardViewModel)App.Current.Resources["boardViewModel"];
        public Check check = new();
        public Stack<PreviousMove> recentMoves = new();


        //public Dictionary<int, Piece.Piece> recentCaptures = new();

        public void SetViewModel()
        {
            temp = (BoardViewModel)App.Current.Resources["boardViewModel"];
        }


        public List<Move> GenerateMoves()
        {
            //Generates all moves possible for whoevers turn it is
            boardViewModel = (BoardViewModel)App.Current.Resources["boardViewModel"];
            List<Move> moves = new();
            
            //Loop through each square
            for (int startSquare = 0; startSquare < 64; startSquare++)
            {
                //if there is a piece on the square
                if (boardViewModel.TheGrid[startSquare].piece != null)
                {                   
                    //Check if it matches color of whoevers turn it is
                    if (boardViewModel.TheGrid[startSquare].piece.IsWhite == boardViewModel.IsWhitesTurn)
                    {
                        moves.AddRange(GenerateMovesBoilerPlate(boardViewModel.TheGrid[startSquare].piece, startSquare));
                    }
                }

            }
            


            return moves;
        }

        public void MakeMove(Move move)
        {
            Piece.Piece selectedPiece = temp.TheGrid[move.StartSquare].piece;
            selectedPiece.HasMoved = true;           
            if (move.IsDoublePush)            
                selectedPiece.HasDoublePushed = true;
            
            if (temp.TheGrid[move.TargetSquare].piece != null)
            {
                recentMoves.Push(new PreviousMove(move, temp.TheGrid[move.TargetSquare].piece));
            }
            else
            {
                recentMoves.Push(new PreviousMove(move));
            }

            temp.TheGrid[move.TargetSquare].piece = selectedPiece;
            temp.TheGrid[move.StartSquare].piece = null;
            temp.Debuger.RecordMove(move.StartSquare, move.TargetSquare, selectedPiece);          
        }


        public bool MakePseudoMove(Move move)
        {
            Piece.Piece selectedPiece = temp.TheGrid[move.StartSquare].piece;
            selectedPiece.HasMoved = true;
            if (temp.TheGrid[move.TargetSquare].piece != null)
            {
                recentMoves.Push(new PreviousMove(move, temp.TheGrid[move.TargetSquare].piece));
                temp.TheGrid[move.TargetSquare].piece = selectedPiece;
                temp.TheGrid[move.StartSquare].piece = null;
                //temp.Debuger.RecordMove(move.StartSquare, move.TargetSquare, selectedPiece);
                return true;
            }
            else
            {
                recentMoves.Push(new PreviousMove(move));
                temp.TheGrid[move.TargetSquare].piece = selectedPiece;
                temp.TheGrid[move.StartSquare].piece = null;
                //temp.Debuger.RecordMove(move.StartSquare, move.TargetSquare, selectedPiece);
                return false;
            }
            
        }
        public void PlacePiece(Move move)
        {
            if (move.isCastleMove)
            {
                boardViewModel.MoveLogic.MakeMove(new Move(move.StartSquare, move.TargetSquare));
                boardViewModel.MoveLogic.MakeMove(new Move(move.CastleStart, move.CastleTarget));
            }
            else if (move.isEnPassent)
            {
                RemovePieceAtIndex(move.EnPassentIndex);
                boardViewModel.MoveLogic.MakeMove(move);
            }
            else
            {
                boardViewModel.MoveLogic.MakeMove(move);
            }
        }




        public void UnmakeMove()
        {
            if (recentMoves.Count != 0)
            {
                PreviousMove previousMove = recentMoves.Pop();
                if (previousMove.CapturedPiece != null)
                {

                }

                
                boardViewModel.TheGrid[previousMove.Move.StartSquare].piece = boardViewModel.TheGrid[previousMove.Move.TargetSquare].piece;
                boardViewModel.TheGrid[previousMove.Move.TargetSquare].piece = previousMove.CapturedPiece;
                
            }
            
        }

        public static void SwitchTurn()
        {
            BoardViewModel temp = (BoardViewModel)App.Current.Resources["boardViewModel"];
            temp.IsWhitesTurn = !temp.IsWhitesTurn;
        }


        public static void RemovePieceAtIndex(int index)
        {
            BoardViewModel temp = (BoardViewModel)App.Current.Resources["boardViewModel"];
            temp.TheGrid[index].piece = null;
        }
        
        
     

        

       
        public List<Move> GenerateMoveForPiece(Piece.Piece piece, int startingPosition)
        {
            boardViewModel = (BoardViewModel)App.Current.Resources["boardViewModel"];
            List<Move> moves = new List<Move>();            
            if (piece != null)
            {                              
                    if (piece.IsWhite == boardViewModel.IsWhitesTurn)
                    {
                        moves = GenerateMovesBoilerPlate(piece, startingPosition);
                    }
            }
            return moves;
        }


        private List<Move> GenerateMovesBoilerPlate(Piece.Piece piece, int startingPosition)
        {
            List<Move> moves = new List<Move>();
            if (piece.Name == "Rook" || piece.Name == "Queen" || piece.Name == "Bishop")
            {
                SlidePiece slidePieces = new();
                moves = slidePieces.GenerateSlidingMoves(startingPosition, piece);
            }
            if (piece.Name == "Pawn")
            {
                Pawn pawnMoves = new(boardViewModel.IsWhitesTurn);
                moves = pawnMoves.GeneratePossibleMoves(piece, startingPosition);
            }
            if (piece.Name == "King")
            {
                King king = new();
                moves = king.GenerateKingMove(startingPosition, piece);
            }
            if (piece.Name == "Knight")
            {
                Knight knight = new();
                moves = knight.GenerateKnightMove(startingPosition);
            }
            return moves;
        }


    }
}

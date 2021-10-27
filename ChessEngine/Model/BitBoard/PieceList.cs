using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Model.BitBoard
{
    public class PieceList
    {
        public int[] occupiedSquares;
        int[] map;
        int numPieces;

        public PieceList(int maxPieceCount = 16)
        {
            occupiedSquares = new int[maxPieceCount];
            map = new int[64];
            numPieces = 0;
        }

        public int Count
        {
            get
            {
                return numPieces;
            }
        }

        public void AddPieceAtSquare(int square)
        {
            occupiedSquares[numPieces] = square;
            map[square] = numPieces;
            numPieces++;
        }

        public void RemovePieceAtSquare(int square)
        {
            int pieceIndex = map[square]; // get the index of this element in the occupiedSquares array
            occupiedSquares[pieceIndex] = occupiedSquares[numPieces - 1]; // move last element in array to the place of the removed element
            map[occupiedSquares[pieceIndex]] = pieceIndex; // update map to point to the moved element's new location in the array
            numPieces--;
        }

        public void MovePiece(int startSquare, int targetSquare)
        {
            int pieceIndex = map[startSquare]; // get the index of this element in the occupiedSquares array
            occupiedSquares[pieceIndex] = targetSquare;
            map[targetSquare] = pieceIndex;
        }

        public int this[int index] => occupiedSquares[index];
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Model;
using ChessEngine.Model.Piece;

namespace ChessEngine.ViewModel
{
    public class BoardViewModel
    {
        //Size of board
        public int Size { get; set; }
        //2d array of type cell
        public Cell[,] theGrid { get; set; }

        public BoardViewModel()
        {
            Size = 8;
            theGrid = new Cell[Size, Size];
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    theGrid[i, j] = new Cell(i, j);
                }
            }
        }

        public void MarkNextLegalMove(Cell currentCell, string chessPiece)
        {
            //Clear privious legel moves
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    theGrid[i, j].LegealNextMove = false;
                    theGrid[i, j].CurrentlyOccupied = false;
                }
            }

            //Find all legal moves and mark the cells as legal
            switch (chessPiece)
            {
                case "Knight":
                    theGrid[currentCell.RowNumber + 2, currentCell.ColumnNumber + 1].LegealNextMove = true;
                    theGrid[currentCell.RowNumber + 2, currentCell.ColumnNumber - 1].LegealNextMove = true;
                    theGrid[currentCell.RowNumber - 2, currentCell.ColumnNumber + 1].LegealNextMove = true;
                    theGrid[currentCell.RowNumber - 2, currentCell.ColumnNumber - 1].LegealNextMove = true;
                    theGrid[currentCell.RowNumber + 2, currentCell.ColumnNumber + 2].LegealNextMove = true;
                    theGrid[currentCell.RowNumber + 2, currentCell.ColumnNumber - 2].LegealNextMove = true;
                    theGrid[currentCell.RowNumber - 2, currentCell.ColumnNumber + 2].LegealNextMove = true;
                    theGrid[currentCell.RowNumber - 2, currentCell.ColumnNumber - 2].LegealNextMove = true;

                    break;
                case "King":
                    break;
                case "Rook":
                    break;
                case "Bishop":
                    break;
                case "Queen":
                    break;
                default:
                    break;
            }
        }
    }
}

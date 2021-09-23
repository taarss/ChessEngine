using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ChessEngine.Model;
using ChessEngine.Model.Piece;

namespace ChessEngine.ViewModel
{
    public class BoardViewModel
    {
        private bool isWhitesTurn = true;
        private bool movePieceEnabled = false;
        private Piece movePiece = new Piece("Pawn", true);
        private ObservableCollection<Cell> theGrid = new ObservableCollection<Cell>();


        public Piece MovePiece { get => movePiece; set => movePiece = value; }
        public bool MovePieceEnabled { get => movePieceEnabled; set => movePieceEnabled = value; }
        public ObservableCollection<Cell> TheGrid { get => theGrid; set => theGrid = value; }
        public bool IsWhitesTurn { get => isWhitesTurn; set => isWhitesTurn = value; }

        public BoardViewModel()
        {
            for (int i = 0; i < 64; i++)
            {
                int[] coordinates = IndexToCoordinate(i);
                theGrid.Add(new Cell(coordinates[0], coordinates[1]));
            }
            LoadDefaultPosition();

        }

        private void LoadDefaultPosition()
        {
            FENLoader("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");
        }

        private void FENLoader(string FenString)
        {
            var pieceTypeFromSymbol = new Dictionary<char, Piece>
            {
                ['k'] = new Piece("King", false),
                ['p'] = new Piece("Pawn", false),
                ['n'] = new Piece("Knight", false),
                ['b'] = new Piece("Bishop", false),
                ['q'] = new Piece("Queen", false),
                ['r'] = new Piece("Rook", false),
                ['K'] = new Piece("King", true),
                ['P'] = new Piece("Pawn", true),
                ['N'] = new Piece("Knight", true),
                ['B'] = new Piece("Bishop", true),
                ['Q'] = new Piece("Queen", true),
                ['R'] = new Piece("Rook", true),
            };
            char[] fenChars = FenString.ToCharArray();
            int index = 0;
            foreach (var item in fenChars)
            {
                if (Char.IsDigit(item))
                {
                    index +=  int.Parse(item.ToString());
                }
                else if (item.ToString() == "/")
                {

                }
                else
                {
                    Piece result = pieceTypeFromSymbol[item];
                    int[] coordinate = IndexToCoordinate(index);
                    Cell cell = new Cell(coordinate[0], coordinate[1]);
                    cell.piece = result;
                    TheGrid[index] = cell;
                    index++;
                }
            }
        }

        private int[] IndexToCoordinate(int index)
        {
            int[] result = new int[2];
            int y = (int)MathF.Ceiling(index / 8);
            int x = index % 8;
            result[0] = x;
            result[1] = y;
            return result;
        }

        public void MarkNextLegalMove(Cell currentCell, string chessPiece)
        {          
            //Find all legal moves and mark the cells as legal
            switch (chessPiece)
            {
                case "Knight":                    
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

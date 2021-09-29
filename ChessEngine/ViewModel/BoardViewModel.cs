using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using ChessEngine.Model;
using ChessEngine.Model.Piece;

namespace ChessEngine.ViewModel
{
    public class BoardViewModel
    {
        private bool isWhitesTurn = true;
        private bool movePieceEnabled = false;
        private List<Move> moves;
        private Piece movePiece = new Piece("Pawn", true);
        private ObservableCollection<Cell> theGrid = new ObservableCollection<Cell>();
        private MoveLogic moveLogic = new();
        private Coordinate followPieceCoordinates = new Coordinate(100, 100);
        private Debuger debuger = new();


        public Piece MovePiece { get => movePiece; set => movePiece = value; }
        public bool MovePieceEnabled { get => movePieceEnabled; set => movePieceEnabled = value; }
        public ObservableCollection<Cell> TheGrid { get => theGrid; set => theGrid = value; }
        public bool IsWhitesTurn { get => isWhitesTurn; set => isWhitesTurn = value; }
        public List<Move> Moves { get => moves; set => moves = value; }
        public Coordinate FollowPieceCoordinates { get => followPieceCoordinates; set => followPieceCoordinates = value; }
        public Debuger Debuger { get => debuger; set => debuger = value; }
        public MoveLogic MoveLogic { get => moveLogic; set => moveLogic = value; }

        public BoardViewModel()
        {
            for (int i = 0; i < 64; i++)
            {
                int[] coordinates = IndexToCoordinate(i);
                theGrid.Add(new Cell(coordinates[0], coordinates[1]));
            }
            LoadDefaultPosition();
            MoveLogic.PrecomputedMoveData();
        }

        private void LoadDefaultPosition()
        {
            //Normal start position
            FENLoader("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");

            //Knight debug fen string
            //FENLoader("1n4n1/8/8/8/8/8/8/1N4N1");

            //Sliding pieces debug
            //FENLoader("r1bq1b1r/8/8/8/8/8/8/R1BQ1B1R");

            //Test 1
            //FENLoader("r6r/1b2k1bq/8/8/7B/8/8/R3K2R");


        }

        private void FENLoader(string FenString)
        {
            var pieceTypeFromSymbol = new Dictionary<char, string>
            {
                ['k'] = "King",
                ['p'] = "Pawn",
                ['n'] = "Knight",
                ['b'] = "Bishop",
                ['q'] = "Queen",
                ['r'] = "Rook",               
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
                    Piece result = new Piece(pieceTypeFromSymbol[Char.ToLower(item)], char.IsUpper(item));
                    int[] coordinate = IndexToCoordinate(index);
                    Cell cell = new Cell(coordinate[0], coordinate[1]);
                    cell.piece = result;
                    TheGrid[index] = cell;
                    index++;
                }
            }
        }

        public int[] IndexToCoordinate(int index)
        {
            int[] result = new int[2];
            int y = (int)MathF.Ceiling(index / 8);
            int x = index % 8;
            result[0] = x;
            result[1] = y;
            return result;
        }

        public bool isMoveLegal(Piece piece)
        {
            throw new NotImplementedException();
        }
    }
}

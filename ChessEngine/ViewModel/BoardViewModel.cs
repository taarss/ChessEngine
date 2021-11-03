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
using ChessEngine.Model.AI;
using ChessEngine.Model.BitBoard;
using System.ComponentModel;

namespace ChessEngine.ViewModel
{
    public class BoardViewModel : INotifyPropertyChanged
    {

        private bool playerColor = true;
        private bool movePieceEnabled = false;
        private List<Move> moves;
        public List<Move> AttackMap = new();
        private Piece movePiece = new Piece("Pawn", true);
        private ObservableCollection<Cell> theGrid = new();
        private MoveLogic moveLogic = new();
        private Debuger debuger = new();
        private Coordinate followPieceCoordinates = new Coordinate(100, 100);
        private BitBoard bitBoard = new();
        public Stack<BitMove> oldMoves = new();






        public BoardViewModel()
        {
            for (int i = 0; i < 64; i++)
            {
                int[] coordinates = IndexToCoordinate(i);
                theGrid.Add(new Cell(coordinates[0], coordinates[1]));
            }
           // LoadDefaultPosition();
            BitBoard.LoadStartPosition();
            UpdateGUI();

        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public void UpdateGUI()
        {
            Piece[] pieces = BoardRepresentation.TranslateBitBoardToUI(BitBoard);
            for (int i = 0; i < pieces.Length; i++)
            {
                theGrid[i].piece = pieces[i];
            }
        }

        private void LoadDefaultPosition()
        {
            //Test 2 start position
            //FENLoader("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R");
            //Normal start position
            FENLoader("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");
            //Knight debug fen string
            //FENLoader("1n4n1/8/8/8/8/8/8/1N4N1");



            //Debug pieces debug
            //FENLoader("8/p3k3/8/6P1/8/8/8/8");


            //Sliding pieces debug
            //FENLoader("2b11b2/pppppppp/8/8/8/8/8/R1BQ1B1R");

            //Test 1
            //FENLoader("7r/1b2k1bq/8/8/7B/8/8/R3K2R");


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

        public static int[] IndexToCoordinate(int index)
        {
            int[] result = new int[2];
            int y = (int)MathF.Ceiling(index / 8);
            int x = index % 8;
            result[0] = x;
            result[1] = y;
            return result;
        }

        public List<int> GetAllTypePieces(int piece)
        {
            List<int> result = new();

            for (int i = 0; i < bitBoard.Square.Length; i++)
            {
                if (bitBoard.Square[i] != 0)
                {
                    if (bitBoard.Square[i] == piece)
                    {
                        result.Add(bitBoard.Square[i]);
                    }
                }
            }    
            return result;
        }


        public Piece MovePiece { get => movePiece; set => movePiece = value; }
        public bool MovePieceEnabled { get => movePieceEnabled; set => movePieceEnabled = value; }
        public ObservableCollection<Cell> TheGrid { get => theGrid; set => theGrid = value; }
        public List<Move> Moves { get => moves; set => moves = value; }
        public Coordinate FollowPieceCoordinates { get => followPieceCoordinates; set => followPieceCoordinates = value; }
        public Debuger Debuger { get => debuger; set => debuger = value; }
        public MoveLogic MoveLogic { get => moveLogic; set => moveLogic = value; }
        public bool PlayerColor { get => playerColor; set => playerColor = value; }
        public BitBoard BitBoard { get => bitBoard; set
            {
                bitBoard = value;
                RaisePropertyChanged("BitBoard");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Model
{
    public class Debuger
    {
        ObservableCollection<String> moveRecordings = new();
        ObservableCollection<String> possibleMoves = new();

        public ObservableCollection<string> MoveRecordings { get => moveRecordings; set => moveRecordings = value; }
        public ObservableCollection<string> PossibleMoves { get => possibleMoves; set => possibleMoves = value; }

        public void RecordMove(int startSquare, int targetSquare, Piece.Piece piece)
        {
            moveRecordings.Add(piece.Name + " " + startSquare.ToString() + " => " + targetSquare.ToString());
        }
    }
}

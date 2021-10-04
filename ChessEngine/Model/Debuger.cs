using ChessEngine.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public void StartMoveGenerationTest(int depth)
        {
            Stopwatch timer = new();
            timer.Start();
            int result = MoveGenerationTest(depth);
            timer.Stop();
            moveRecordings.Add(result.ToString() + " " + timer.ElapsedMilliseconds);
            
        }

        public int MoveGenerationTest(int depth)
        {
            BoardViewModel boardViewModel = (BoardViewModel)App.Current.Resources["boardViewModel"]; 
            if (depth == 0)
            {
                return 1;
            }
            List<Move> moves = boardViewModel.MoveLogic.GenerateMoves();
            int numPositions = 0;
            foreach (var move in moves)
            {
                boardViewModel.MoveLogic.MakeMove(move);
                numPositions += MoveGenerationTest(depth - 1);
                boardViewModel.MoveLogic.UnmakeMove(move);
            }
            return numPositions;
        }
    }
}

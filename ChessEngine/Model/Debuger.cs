using ChessEngine.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ChessEngine.Model
{
    public class Debuger : INotifyPropertyChanged
    {
        ObservableCollection<String> moveRecordings = new();
        ObservableCollection<String> possibleMoves = new();
        private int numPositionsBinding = 0;
        



        public ObservableCollection<string> MoveRecordings { get => moveRecordings; set => moveRecordings = value; }
        public ObservableCollection<string> PossibleMoves { get => possibleMoves; set => possibleMoves = value; }
        
        public int NumPositionsBinding { get => numPositionsBinding; set {
                numPositionsBinding = value;
                RaisePropertyChanged("numPositionsBinding");
            } 
        }

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
            test(depth);

            /*
            if (depth == 0)
            {
                return 1;
            }
            List<Move> moves = boardViewModel.MoveLogic.GenerateMoves();
            int numPositions = 0;
            foreach (var move in moves)
            {
                boardViewModel.MoveLogic.MakePseudoMove(move);
                numPositions += MoveGenerationTest(depth - 1);
                boardViewModel.MoveLogic.UnmakeMove(move);
            }
            
             return numPositions;
             
            

            */
            return NumPositionsBinding;
        }

        public async void test(int depth)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer("C:/Users/chri45n5/source/repos/ChessEngine/ChessEngine/assets/sounds/chess.wav");
            BoardViewModel boardViewModel = (BoardViewModel)App.Current.Resources["boardViewModel"];
            List<Move> moves = boardViewModel.MoveLogic.GenerateMoves();
            foreach (var move in moves)
            {
                boardViewModel.MoveLogic.MakePseudoMove(move);
                numPositionsBinding++;
                await Task.Delay(10);
                MoveLogic.SwitchTurn();
                moves = boardViewModel.MoveLogic.GenerateMoves();
                foreach (var item in moves)
                {
                    boardViewModel.MoveLogic.MakePseudoMove(item);
                    numPositionsBinding++;
                    await Task.Delay(10);
                    boardViewModel.MoveLogic.UnmakeMove(item);
                    await Task.Delay(10);
                }
                boardViewModel.MoveLogic.UnmakeMove(move);
                MoveLogic.SwitchTurn();
                await Task.Delay(10);


            }
        }
                
            /*
            foreach (var move in moves)
            {
                boardViewModel.MoveLogic.MakeMove(move);
                player.Play();
                numPositionsBinding++;
                await Task.Delay(100);
                boardViewModel.MoveLogic.UnmakeMove(move);
                await Task.Delay(100);
            }

            
           if (depth != 0)
           {
               BoardViewModel boardViewModel = (BoardViewModel)App.Current.Resources["boardViewModel"];
               List<Move> moves = boardViewModel.MoveLogic.GenerateMoves();            
               foreach (var move in moves)
               {
                   boardViewModel.MoveLogic.MakeMove(move);
                   await Task.Delay(100);
                   test(depth - 1);
                   boardViewModel.MoveLogic.UnmakeMove(move);
                   await Task.Delay(100);
               }
           }*/
        

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

    }
}

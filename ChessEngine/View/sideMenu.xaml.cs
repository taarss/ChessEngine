using ChessEngine.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChessEngine.Model.AI;
using ChessEngine.Model.BitBoard;
using ChessEngine.Model.BitBoard.BinaryMoveGen;

namespace ChessEngine.View
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        private int[] expectedPly = { 1, 20, 400, 8902, 197281, 4865609 };
        private BoardViewModel boardViewModel = (BoardViewModel)App.Current.Resources["boardViewModel"];
        public UserControl1()
        {
            InitializeComponent();
            background.Fill = new SolidColorBrush(Color.FromRgb(51, 51, 51));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwatch = new();
            for (int i = 0; i < 1; i++)
            {
                stopwatch.Start();
                int result = boardViewModel.Debuger.MoveGenerationTest(3);
                stopwatch.Stop();
                TextBlock text = new();
                StackPanel canvas = new();
                TextBlock block = new();
                canvas.Orientation = Orientation.Horizontal;
                if (result != expectedPly[i])
                {
                    block.Text = "✕";
                    block.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    block.Text = "✔";
                    block.Foreground = new SolidColorBrush(Colors.Green);
                }
                block.Width = 20;
                text.Text = "Depth: " + i + " ply    " + "Result: " + result + "  Time: " + stopwatch.ElapsedMilliseconds + " milliseconds";
                text.FontSize = 15;
                text.Foreground = new SolidColorBrush(Colors.White);
                
                canvas.Children.Add(block);
                canvas.Children.Add(text);
                canvas.Height = 50;
                testResults.Children.Add(canvas);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            testResults.Children.Clear();
            //BitBoard tempBoard = new(boardViewModel.bitBoard);
            BitBoard tempBoard = boardViewModel.bitBoard;
            AI ai = new(tempBoard);
            ai.StartSearch();
            boardViewModel.bitBoard.MakeMove(ai.GetSearchResult());
            boardViewModel.UpdateGUI();
        }
    }
}

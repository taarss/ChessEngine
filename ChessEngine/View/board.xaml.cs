﻿using System;
using System.Collections.Generic;
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
using ChessEngine.Model;
using ChessEngine.Model.Piece;
using ChessEngine.ViewModel;

namespace ChessEngine.View
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Board : UserControl
    {
        private Piece selectedPiece;
        private int oldIndex;
        private BoardViewModel boardViewModel = (BoardViewModel)App.Current.Resources["boardViewModel"];
        private List<Move> moves;


        public Board()
        {
            InitializeComponent();
            InitiateGrid();
        }
        private void InitiateGrid()
        {           
            bool isWhite = false;
            myGrid.Width = 480;
            myGrid.Height = 480;
            myGrid.HorizontalAlignment = HorizontalAlignment.Left;
            myGrid.VerticalAlignment = VerticalAlignment.Top;
            //Creates rows and collumns based of the given number
            for (int i = 0; i < 8; i++)
            {
                ColumnDefinition colDef = new ColumnDefinition();
                RowDefinition rowDef = new RowDefinition();
                for (int j = 0; j < 8; j++)
                {
                    Rectangle rectangle = new Rectangle();
                    rectangle.Name = "name" +(i * j).ToString();
                    if (isWhite)                 
                        rectangle.Fill = new SolidColorBrush(System.Windows.Media.Colors.Bisque);
                    else
                        rectangle.Fill = new SolidColorBrush(System.Windows.Media.Colors.Brown);

                    isWhite = !isWhite;
                    Grid.SetRow(rectangle, j);
                    Grid.SetColumn(rectangle, i);
                    myGrid.Children.Add(rectangle);
                }
                isWhite = !isWhite;

                myGrid.RowDefinitions.Add(rowDef);
                myGrid.ColumnDefinitions.Add(colDef);

            }
        }

        private void selectPiece(object sender, MouseEventArgs e)
        {
            moves = new List<Move>();
            Point position = Mouse.GetPosition(myCanvas);
            oldIndex = CalculateIndexFromPosition(position);
            selectedPiece = boardViewModel.TheGrid[oldIndex].piece;
            MoveLogic moveLogic = new();
            moveLogic.PrecomputedMoveData();
            moves = moveLogic.GenerateMoveForPiece(boardViewModel.TheGrid[oldIndex].piece, oldIndex);
            foreach (var item in moves)
            {
                myGrid.Children.
                Rectangle rectangle = (Rectangle)FindName("name"+item.TargetSquare.ToString());
                rectangle.Fill = new SolidColorBrush(System.Windows.Media.Colors.Yellow);
            }
        }

        private void placePiece(object sender, MouseButtonEventArgs e)
        {
            if (selectedPiece != null)
            {
                Point position = Mouse.GetPosition(myCanvas);
                int index = CalculateIndexFromPosition(position);
                foreach (var item in moves)
                {
                    if (item.TargetSquare == index)
                    {
                        selectedPiece.HasMoved = true;
                        boardViewModel.TheGrid[index].piece = selectedPiece;
                        boardViewModel.TheGrid[oldIndex].piece = null;
                        selectedPiece = null;
                        System.Media.SoundPlayer player = new System.Media.SoundPlayer("C:/Users/chris/Source/Repos/ChessEngine/ChessEngine/assets/sounds/chess.wav");
                        player.Play();
                        break;
                    }
                }
                
            }                              
        }

        private int CalculateIndexFromPosition(Point point)
        {
            int pX = (int)Math.Floor(point.X / 60.0);
            int pY = (int)Math.Floor(point.Y / 60.0);
            int index = 8 * pX + pY;
            return index;
        }
    }
}

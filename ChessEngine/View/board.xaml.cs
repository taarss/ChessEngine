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
        private bool isDragging = false;

        public Board()
        {
            InitializeComponent();
            InitiateGrid();
        }
        private void InitiateGrid()
        {           
            bool isWhite = false;
            myGridOverlay.Width = 480;
            myGridOverlay.Height = 480;
            myGrid.Width = 480;
            myGrid.Height = 480;
            myGrid.HorizontalAlignment = HorizontalAlignment.Left;
            myGrid.VerticalAlignment = VerticalAlignment.Top;
            myGridOverlay.HorizontalAlignment = HorizontalAlignment.Left;
            myGridOverlay.VerticalAlignment = VerticalAlignment.Top;
            //Creates rows and collumns based of the given number
            for (int i = 0; i < 8; i++)
            {
                ColumnDefinition colDef = new ColumnDefinition();
                RowDefinition rowDef = new RowDefinition();
                ColumnDefinition overlayColDef = new ColumnDefinition();
                RowDefinition overlayRowDef = new RowDefinition();
                for (int j = 0; j < 8; j++)
                {
                    Rectangle rectangle = new Rectangle();
                    Rectangle overlayRectangle = new Rectangle();
                    if (isWhite)                 
                        rectangle.Fill = new SolidColorBrush(System.Windows.Media.Colors.Bisque);
                    else
                        rectangle.Fill = new SolidColorBrush(System.Windows.Media.Colors.Peru);

                    isWhite = !isWhite;
                    Grid.SetRow(rectangle, j);
                    Grid.SetColumn(rectangle, i);
                    Grid.SetRow(overlayRectangle, j);
                    Grid.SetColumn(overlayRectangle, i);
                    myGrid.Children.Add(rectangle);
                    myGridOverlay.Children.Add(overlayRectangle);
                    
                }
                isWhite = !isWhite;

                myGrid.RowDefinitions.Add(rowDef);
                myGrid.ColumnDefinitions.Add(colDef);
                myGridOverlay.RowDefinitions.Add(overlayRowDef);
                myGridOverlay.ColumnDefinitions.Add(overlayColDef);

            }
        }

        private void selectPiece(object sender, MouseEventArgs e)
        {
            moves = new List<Move>();
            Point position = Mouse.GetPosition(myCanvas);
            oldIndex = CalculateIndexFromPosition(position);
            selectedPiece = boardViewModel.TheGrid[oldIndex].piece;
            boardViewModel.TheGrid[oldIndex].piece = null;
            MoveLogic moveLogic = new();
            moveLogic.PrecomputedMoveData();
            moves = moveLogic.GenerateMoveForPiece(selectedPiece, oldIndex);
            MarkLegalMoves(moves);
            isDragging = true;
            followPiece.Visibility = Visibility.Visible;
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
                        boardViewModel.TheGrid[index].piece = selectedPiece;
                        boardViewModel.TheGrid[index].piece.HasMoved = true;
                        boardViewModel.TheGrid[oldIndex].piece = null;
                        boardViewModel.Debuger.RecordMove(item.StartSquare, item.TargetSquare, selectedPiece);
                        selectedPiece = null;
                        UnmarkLegalMoves();
                        System.Media.SoundPlayer player = new System.Media.SoundPlayer("C:/Users/chris/Source/Repos/ChessEngine/ChessEngine/assets/sounds/chess.wav");
                        player.Play();
                        isDragging = false;
                        followPiece.Visibility = Visibility.Collapsed;
                        break;
                    }

                }
                boardViewModel.TheGrid[oldIndex].piece = selectedPiece;
            }
            isDragging = false;
            followPiece.Visibility = Visibility.Collapsed;
        }

        private int CalculateIndexFromPosition(Point point)
        {
            int pX = (int)Math.Floor(point.X / 60.0);
            int pY = (int)Math.Floor(point.Y / 60.0);
            int index = 8 * pX + pY;
            return index;
        }

        private void MarkLegalMoves(List<Move> moves)
        {
            foreach (var item in moves)
            {
                int[] move = boardViewModel.IndexToCoordinate(item.TargetSquare);
                Rectangle rectangle = myGridOverlay.Children.Cast<Rectangle>().First(e => Grid.GetRow(e) == move[0] && Grid.GetColumn(e) == move[1]);

                rectangle.Fill = new SolidColorBrush(System.Windows.Media.Colors.Yellow);
                rectangle.Opacity = 0.3;
            }
        }

        private void UnmarkLegalMoves()
        {
            foreach (Rectangle item in myGridOverlay.Children)
            {
                item.Fill = Brushes.Transparent;
            }
        }

        private void Rectangle_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isDragging)
            {
                var uriSource = new Uri(selectedPiece.Icon);
                followPiece.Source = new BitmapImage(uriSource);
                Point position = Mouse.GetPosition(myCanvas);
                int pX = (int)Math.Round(position.X - 30);
                int pY = (int)Math.Round(position.Y - 30);
                boardViewModel.FollowPieceCoordinates.X = pX;
                boardViewModel.FollowPieceCoordinates.Y = pY;
            }
        }
    }
}

using System;
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

namespace ChessEngine.View
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Board : UserControl
    {
        public Board()
        {
            InitializeComponent();
            InitiateGrid();
        }
        private void InitiateGrid()
        {
            

            bool isWhite = false;
            myGrid.Width = 500;
            myGrid.Height = 500;
            myGrid.HorizontalAlignment = HorizontalAlignment.Left;
            myGrid.VerticalAlignment = VerticalAlignment.Top;
            myGrid.ShowGridLines = true;
            //Creates rows and collumns based of the given number
            for (int i = 0; i < 8; i++)
            {
                ColumnDefinition colDef = new ColumnDefinition();
                RowDefinition rowDef = new RowDefinition();
                for (int j = 0; j < 8; j++)
                {
                    Rectangle rectangle = new Rectangle();
                    if (isWhite)                 
                        rectangle.Fill = new SolidColorBrush(System.Windows.Media.Colors.White);
                    else
                        rectangle.Fill = new SolidColorBrush(System.Windows.Media.Colors.Black);

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
    }
}

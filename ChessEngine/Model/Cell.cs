using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Model.Piece;

namespace ChessEngine.Model
{
    public class Cell : INotifyPropertyChanged
    {
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public bool CurrentlyOccupied { get; set; }
        public bool LegealNextMove { get; set; }
        public Piece.Piece piece { get => Piece; set {
                Piece = value;
                RaisePropertyChanged("piece");
            } 
        }

        private Piece.Piece Piece;


        public Cell(int x, int y)
        {
            RowNumber = x * 60;
            ColumnNumber = y * 60;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}

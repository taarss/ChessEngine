using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessEngine.Model.Piece;
using ChessEngine.Model;
using System.ComponentModel;

namespace ChessEngine.Model
{
    public class BoardContainer<Cell> : INotifyPropertyChanged
    {
        private Cell[] array = new Cell[64];

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public Cell this[int i]
        {
            get { return this.Array[i]; }
            set {
                this.Array[i] = value;
                RaisePropertyChanged("TheGrid.Array");

            }
        }

        public void AddPiece(Cell cell, int index)
        {
            array[index] = cell;
            RaisePropertyChanged("TheGrid.Array");
        }

        public Cell[] Array { get => array; set {
                this.array = value;
                RaisePropertyChanged("TheGrid.Array");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}

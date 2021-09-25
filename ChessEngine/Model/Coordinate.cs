using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Model
{
    public class Coordinate : INotifyPropertyChanged
    {
        private int y;
        private int x;

        public Coordinate(int x, int y)
        {
            this.Y = y;
            this.X = x;
        }

        public int Y
        {
            get => y; set
            {
                y = value;
                RaisePropertyChanged("Y");
                
            }
        }
        public int X
        {
            get => x; set
            {
                 x = value;
                RaisePropertyChanged("X");              
            }
        }

        private void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

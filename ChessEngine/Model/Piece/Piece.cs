using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ChessEngine.Model.Piece
{
    public class Piece
    {
        private string name;
        private int value;
        private bool isWhite;
        private bool hasMoved = false;
        private string icon = "C:/Users/chris/Source/Repos/ChessEngine/ChessEngine/assets/pieces/";
        public Piece(string name, bool isWhite)
        {
            this.name = name;
            this.isWhite = isWhite;
            IconFinder();
        }

        public string Name { get => name; set => name = value; }
        public int Value { get => value; set => this.value = value; }
        public bool IsWhite { get => isWhite; set => isWhite = value; }
        public string Icon { get => icon; set => icon = value; }
        public bool HasMoved { get => hasMoved; set => hasMoved = value; }

        private void IconFinder()
        {
            if (isWhite)
            {
                Icon = icon + "white" + name + ".png";
            }
            else
            {
                Icon = icon + "black" + name + ".png";
            }
        }
    }
}

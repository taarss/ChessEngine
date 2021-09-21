using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Model.Piece
{
    public class Piece
    {
        private string name;
        private int value;
        private bool isWhite;
        private string icon;

        public Piece(string name, bool isWhite)
        {
            this.name = name;
            this.isWhite = isWhite;
        }

        public string Name { get => name; set => name = value; }
        public int Value { get => value; set => this.value = value; }
        public bool IsWhite { get => isWhite; set => isWhite = value; }
    }
}

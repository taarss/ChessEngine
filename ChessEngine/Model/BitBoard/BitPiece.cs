using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Model.BitBoard
{
    public static class BitPiece
    {
        public const int None = 0;
        public const int King = 1;
        public const int Pawn = 2;
        public const int Knight = 3;
        public const int Bishop = 4;
        public const int Rook = 5;
        public const int Queen = 6;

        public const int White = 8;
        public const int Black = 16;

        const int typeMask = 0b00111;
        const int blackMask = 0b10000;
        const int whiteMask = 0b01000;
        const int colourMask = whiteMask | blackMask;
        /*
        color   type
        00   -   000

        none    none
        00   -  000

        black   king
        10   -  001     
        black   pawn
        10   -  010
        black   knight
        10   -  011
        black   bishop
        10   -  100
        black   rook
        10   -  101
        black   queen
        10   -  111


        white   king
        01   -  001     
        white   pawn
        01   -  010
        white   knight
        01   -  011
        white   bishop
        01   -  100
        white   rook
        01   -  101
        white   queen
        01   -  111            
         */
        public static bool IsColour(int piece, int colour)
        {
            return (piece & colourMask) == colour;
        }

        public static int Colour(int piece)
        {
            return piece & colourMask;
        }

        public static int PieceType(int piece)
        {
            return piece & typeMask;
        }
        public static bool IsRookOrQueen(int piece)
        {
            return (piece & 0b110) == 0b110;
        }

        public static bool IsBishopOrQueen(int piece)
        {
            return (piece & 0b101) == 0b101;
        }

        public static bool IsSlidingPiece(int piece)
        {
            return (piece & 0b100) != 0;
        }
    }
}

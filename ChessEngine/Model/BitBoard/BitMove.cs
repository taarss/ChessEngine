using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Model.BitBoard
{
    public readonly struct BitMove
    {
		/* 
		To preserve memory during search, moves are stored as 16 bit numbers.
		The format is as follows:
		bit 0-5: from square (0 to 63)
		bit 6-11: to square (0 to 63)
		bit 12-15: flag
		*/
		public readonly struct Flag
		{
			public const int None = 0;
			public const int EnPassantCapture = 1;
			public const int Castling = 2;
			public const int PromoteToQueen = 3;
			public const int PromoteToKnight = 4;
			public const int PromoteToRook = 5;
			public const int PromoteToBishop = 6;
			public const int PawnTwoForward = 7;
		}

		const ushort startSquareMask = 0b0000000000111111;
		const ushort targetSquareMask = 0b0000111111000000;
		const ushort flagMask = 0b1111000000000000;

		readonly ushort moveValue;

		public BitMove(ushort moveValue)
        {
			this.moveValue = moveValue;
        }
	}
}

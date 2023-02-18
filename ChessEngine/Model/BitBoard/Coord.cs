using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Model.BitBoard
{
	public struct Coord : IComparable<Coord>
	{
		public readonly int fileIndex;
		public readonly int rankIndex;

		public Coord(int fileIndex, int rankIndex)
		{
			this.fileIndex = fileIndex;
			this.rankIndex = rankIndex;
		}

		public bool IsLightSquare()
		{
			return (fileIndex + rankIndex) % 2 != 0;
		}

		public int CompareTo(Coord other)
		{
			return (fileIndex == other.fileIndex && rankIndex == other.rankIndex) ? 0 : 1;
		}
	}
}

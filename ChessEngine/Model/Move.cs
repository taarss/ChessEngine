using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessEngine.Model
{
    public class Move
    {
        public int StartSquare;
        public int TargetSquare;
        public int CastleTarget;
        public int CastleStart;
        public int EnPassentIndex;
        public bool isCastleMove = false;
        public bool isEnPassent = false;
        public bool IsDoublePush = false;

        public Move(int startSquare, int targetSquare)
        {
            StartSquare = startSquare;
            TargetSquare = targetSquare;
        }
        public Move(int startSquare, int targetSquare, bool isDoublePush)
        {
            StartSquare = startSquare;
            TargetSquare = targetSquare;
            IsDoublePush = isDoublePush;
        }
        public Move(int startSquare, int targetSquare, int enPassentIndex)
        {
            StartSquare = startSquare;
            TargetSquare = targetSquare;
            EnPassentIndex = enPassentIndex;
            isEnPassent = true;
        }
        public Move(int startSquare, int targetSquare, int castleTarget, int castleStart)
        {
            StartSquare = startSquare;
            TargetSquare = targetSquare;
            CastleTarget = castleTarget;
            CastleStart = castleStart;
            isCastleMove = true;
        }
    }
}

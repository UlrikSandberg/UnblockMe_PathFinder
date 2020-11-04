using System;
namespace UnblockMe
{
    public class PossibleMoves
    {
        public int PositiveMoves { get; }
        public int NegativeMoves { get; }

        public PossibleMoves(int positiveMoves, int negativeMoves)
        {
            PositiveMoves = positiveMoves;
            NegativeMoves = negativeMoves;
        }
    }
}

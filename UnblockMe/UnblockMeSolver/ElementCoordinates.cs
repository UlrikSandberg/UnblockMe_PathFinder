using System;
namespace UnblockMe
{
    public class ElementCoordinates
    {
        public int StartRow { get; }
        public int StartCol { get; }

        public ElementCoordinates(int startRow, int startCol)
        {
            StartRow = startRow;
            StartCol = startCol;
        }
    }
}

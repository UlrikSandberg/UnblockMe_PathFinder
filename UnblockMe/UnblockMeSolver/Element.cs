using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace UnblockMe
{
    public class Element
    {
        public Orientation Orientation { get; }
        public Color Color { get; set; }
        public ElementType Type { get; }
        public ElementCoordinates Coordinates { get; set; }
        public int Size { get; set; }
        public Guid Id { get; set; }

        public int BoardId { get; set; }

        public Element(ElementType elementType, int startRow, int startCol, Orientation orientation = Orientation.Horizontal)
        {
            Coordinates = new ElementCoordinates(startRow, startCol);
            Type = elementType;
            Orientation = orientation;

            Id = Guid.NewGuid();

            Color = GenerateColor();
        }

        public Element(Element element)
        {
            Coordinates = element.Coordinates;
            Type = element.Type;
            Orientation = element.Orientation;
            Id = element.Id;

            BoardId = element.BoardId;
            Color = element.Color;
        }

        public List<ElementCoordinates> GetCoordinateList()
        {
            var list = new List<ElementCoordinates>();

            Size = Type.Equals(ElementType.Big) ? 3 : 2;

            if(Orientation.Equals(Orientation.Horizontal))
            {
                for(int i = Coordinates.StartCol; i < Coordinates.StartCol + Size; i++)
                {
                    list.Add(new ElementCoordinates(Coordinates.StartRow, i));
                }
            }
            else
            {
                for(int i = Coordinates.StartRow; i < Coordinates.StartRow + Size; i++)
                {
                    list.Add(new ElementCoordinates(i, Coordinates.StartCol));
                }
            }

            return list;
        }

        public Color GenerateColor()
        {
            var rn = new Random();

            var rlist = new int[5];

            var gList = new int[5];

            var bList = new int[5];

            for (int i = 0; i < 5; i++)
            {
                var next = rn.Next(0, 255);
                Console.WriteLine($"Generating reds: {next}");
                rlist[i] = next;
            }

            for (int i = 0; i < 5; i++)
            {
                var next = rn.Next(0, 255);
                Console.WriteLine($"Generating greens: {next}");
                gList[i] = next;
            }

            for (int i = 0; i < 5; i++)
            {
                var next = rn.Next(0, 255);
                Console.WriteLine($"Generating blues: {next}");
                bList[i] = next;
            }

            var r = rn.Next(0, 4);
            var g = rn.Next(0, 4);
            var b = rn.Next(0, 4);

            return Color.FromRgb(rlist[r], gList[g], bList[b]);
        }
    }

    public enum Orientation
    {
        Horizontal,
        Vertical
    }

    public enum ElementType
    {
        Small,
        Big,
        WinnerBlock
    }
}

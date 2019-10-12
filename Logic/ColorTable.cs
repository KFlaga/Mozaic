using System;
using System.Collections.Generic;
using System.Drawing;

namespace MozaicLand
{
    public struct BlockColor
    {
        public Color Color { get; set; }
        public double Cost { get; set; }
    }

    public class ColorTable
    {
        public const int NoColor = -1;

        public Dictionary<int, BlockColor> Colors { get; private set; }

        public Color Color(int i)
        {
            return Colors[i].Color;
        }

        public double Cost(int i)
        {
            return Colors[i].Cost;
        }

        public ColorTable()
        {
            Colors = new Dictionary<int, BlockColor>();
        }

        public ColorTable(Dictionary<int, BlockColor> colors)
        {
            Colors = colors;
        }

        public ColorTable(string contents)
        {
            throw new NotImplementedException("Loading from text is not implemented yet");
        }
    }
}

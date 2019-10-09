using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace MozaicLand
{
    public struct BlockColor
    {
        Color Color { get; set; }
        double Cost { get; set; }
    }

    public class ColorTable
    {
        public Dictionary<int, BlockColor> Colors { get; private set; }

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

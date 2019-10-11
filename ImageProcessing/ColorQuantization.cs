using Emgu.CV;
using Emgu.CV.Structure;
using MozaicLand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing
{
    public static class ColorQuantization
    {
        // Finds color in ColorTable which is closest to given color (i.e. total difference between each of BGR values is smallest).
        public static Bgr BestMatch(Bgr initColor, ColorTable colorTable)
        {
            BlockColor closest = colorTable.Colors.Values.MinElement((block) =>
            {
                return Math.Abs(initColor.Blue - block.Color.B) +
                       Math.Abs(initColor.Green - block.Color.G) +
                       Math.Abs(initColor.Red - block.Color.R);
            });
            return new Bgr(closest.Color);
        }

        // Substitutes every pixel in given image with best matching color from ColorTable.
        public static Image<Bgr, byte> AssignBestMatchColors(Image<Bgr, byte> fragmented, ColorTable colorTable)
        {
            Image<Bgr, byte> result = new Image<Bgr, byte>(fragmented.Cols, fragmented.Rows);
            fragmented.ForEach((pixel, color) =>
            {
                result[pixel.Y, pixel.X] = BestMatch(color, colorTable);
            });
            return result;
        }
    }
}

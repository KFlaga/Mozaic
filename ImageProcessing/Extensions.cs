using Emgu.CV;
using System;
using System.Drawing;

namespace ImageProcessing
{
    public static class Extensions
    {
        public static void ForEach<TColor, TDepth>(this Image<TColor, TDepth> image, Action<Point> func)
            where TColor : struct, IColor
            where TDepth : new()
        {
            for (int r = 0; r < image.Rows; ++r)
            {
                for (int c = 0; c < image.Cols; ++c)
                {
                    func(new Point(c, r));
                }
            }
        }

        public static void ForEach<TColor, TDepth>(this Image<TColor, TDepth> image, Action<Point, TColor> func)
            where TColor : struct, IColor
            where TDepth : new()
        {
            for (int r = 0; r < image.Rows; ++r)
            {
                for (int c = 0; c < image.Cols; ++c)
                {
                    func(new Point(c, r), image[r, c]);
                }
            }
        }
    }
}

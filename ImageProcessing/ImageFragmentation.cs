using MozaicLand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace ImageProcessing
{
    public class FragmentationResult
    {
        public Image<Bgr, byte> Blocks { get; set; }
        public double MarginTop { get; set; }
        public double MarginLeft { get; set; }
    }

    public static class Fragmenter
    {
        // Divides area of real mozaic into blocks, separated by frame walls.
        // If whole number of blocks doesn't fit into given area its margins are cropped.
        // Finally, given image is resized so that its every pixel corresponds to a color block.
        // Color of each block is interpolated from its neighbourhood according to given interpolation method.
        // 
        // If image cannot be fragmented (e.g. when even one block won't fit real mozaic), null is returned.
        public static FragmentationResult FragmentImage(
            Image<Bgr, byte> image,
            SizeF realSize,
            double blockSize, double frameSize, // size of frame between blocks
            Emgu.CV.CvEnum.Inter interpolationMethod = Emgu.CV.CvEnum.Inter.Cubic
            )
        {
            // Compute how many blocks will fit into image, taking into account spacing between them
            // Number of frame sides is equal to number of blocks + 1, thats why frameSize is substracted from real size
            double blockWithFrame = blockSize + frameSize;
            int rows = (int)((realSize.Height - frameSize) / blockWithFrame);
            int cols = (int)((realSize.Width - frameSize) / blockWithFrame);

            if(rows == 0 || cols == 0)
            {
                // Not even one block would fit
                return null;
            }

            // Actual size of mozaic
            double actualHeight = rows * blockSize + (rows + 1) * frameSize;
            double actualWidth = cols * blockSize + (cols + 1) * frameSize;

            // Amount of image which needs to be cut from each side
            // It will also define top-left of mozaic on image
            double marginTop = (realSize.Height - frameSize - rows * blockWithFrame) / 2;
            double marginLeft = (realSize.Width - frameSize - cols * blockWithFrame) / 2;

            // Relative values (in range [0-1])
            double topRelative = marginTop / realSize.Height;
            double leftRelative = marginLeft / realSize.Width;
            double heightRelative = actualHeight / realSize.Height;
            double widthRelative = actualWidth / realSize.Width;

            int topPixel = (int)(topRelative * image.Rows);
            int leftPixel = (int)(leftRelative * image.Cols);
            int heightPixel = (int)(heightRelative * image.Rows);
            int widthPixel = (int)(widthRelative * image.Cols);

            // Crop image accroding to computed pixel sizes
            var cropped = image.GetSubRect(new Rectangle(
                leftPixel, topPixel,
                widthPixel, heightPixel
            ));

            // Simplest way to pick color for each block will be to up-/downscale image so that each pixel will correspond to one block
            var resized = image.Resize(cols, rows, interpolationMethod);
            return new FragmentationResult()
            {
                Blocks = resized,
                MarginTop = marginTop,
                MarginLeft = marginLeft
            };
        }

        // Draws a frame with given color between each block in fragmented image.
        // Each block in scaled in resulting image by 'pixelsPerBlock' and is separated by frame of size 'pixelsPerFrame'.
        public static Image<Bgr, byte> DrawWithFrames(Image<Bgr, byte> fragmented, int pixelsPerBlock, int pixelsPerFrame, Color frameColor)
        {
            int resultRows = fragmented.Rows * pixelsPerBlock + (fragmented.Rows + 1) * pixelsPerFrame;
            int resultCols = fragmented.Cols * pixelsPerBlock + (fragmented.Cols + 1) * pixelsPerFrame;

            Image<Bgr, byte> result = new Image<Bgr, byte>(resultCols, resultRows, new Bgr(frameColor));
            fragmented.ForEach((pixel, blockColor) =>
            {
                // Pixel from source image will be enlarged - rectangular ROI (Region Of Interest) on result image may be used for that
                // First we compute top-left of such rectangle using known formula
                // Then we set ROI of desired size and fill it with source pixel color
                // If ROI is set on image function 'SetValue()' will only fill assigned ROI rather than whole image
                int top = pixel.Y * pixelsPerBlock + (pixel.Y + 1) * pixelsPerFrame;
                int left = pixel.X * pixelsPerBlock + (pixel.X + 1) * pixelsPerFrame;
                result.ROI = new Rectangle(left, top, pixelsPerBlock, pixelsPerBlock);
                result.SetValue(blockColor);
            });
            result.ROI = Rectangle.Empty;
            return result;
        }
    }
}

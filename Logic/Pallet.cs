using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MozaicLand
{
    public class Pallet
    {
        public int[,] BlocksColors { get; private set; } // BlockColor is index in ColorTable
        public float BlockSize { get; set; }
        public float FrameSize { get; set; }
        
        public SizeF Size => new SizeF(BlockTopLeft(BlocksColors.Rows(), BlocksColors.Cols()));
        
        public Pallet(int rows, int cols, float blockSize, float frameSize = 0) :
            this(new int[rows, cols], blockSize, frameSize)
        {
        }

        public Pallet(int[,] blocks, float blockSize, float frameSize = 0)
        {
            BlocksColors = blocks;
            BlockSize = blockSize;
            FrameSize = frameSize;
        }

        public static PointF BlockTopLeft(int row, int col, float blockSize, float frameSize)
        {
            return new PointF(
                col * blockSize + (col + 1) * frameSize,
                row * blockSize + (row + 1) * frameSize
            );
        }

        public PointF BlockTopLeft(int row, int col)
        {
            return Pallet.BlockTopLeft(row, col, BlockSize, FrameSize);
        }

        public int CountBlocksWithColor(int colorIndex)
        {
            return BlocksColors.ToEnumerable().Count((block) => block == colorIndex);
        }

        public Dictionary<int, int> CountColors()
        {
            Dictionary<int, int> colors = new Dictionary<int, int>();
            foreach(int color in BlocksColors)
            {
                if(colors.ContainsKey(color))
                {
                    colors[color] += 1;
                }
                else
                {
                    colors.Add(color, 1);
                }
            }

            return colors;
        }
    }
}

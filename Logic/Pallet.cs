using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MozaicLand
{
    // Pallet consists of grid of color blocks with a thin rigid frame keeping them in place.
    // May represent both target pallet on mozaic and pallet which is currently being filled by robot by assigning -1 to some of blocks. 
    public class Pallet
    {
        public int[,] BlocksColors { get; private set; } // BlockColor is index in ColorTable, negative value means cell is empty
        public float BlockSize { get; set; }
        public float FrameSize { get; set; }
        
        // Size is equal to top-left corner of hypothetical block attached to bot-right corner of pallet
        public SizeF Size => new SizeF(BlockTopLeft(BlocksColors.Rows(), BlocksColors.Cols()));
        
        public Pallet(int[,] blocks, float blockSize, float frameSize = 0)
        {
            BlocksColors = blocks;
            BlockSize = blockSize;
            FrameSize = frameSize;
        }

        // TODO: change rows, cols to Size
        public Pallet(int rows, int cols, float blockSize, float frameSize = 0) :
            this(new int[rows, cols], blockSize, frameSize)
        {
            BlocksColors.Fill(ColorTable.NoColor);
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

        public PointF BlockCenter(int row, int col)
        {
            return BlockTopLeft(row, col) + new SizeF(BlockSize / 2, BlockSize / 2);
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

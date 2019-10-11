using System.Collections.Generic;
using System.Linq;

namespace MozaicLand
{
    public class Pallet
    {
        public int[,] BlocksColors { get; private set; } // BlockColor is index in ColorTable
        public double BlockSize { get; private set; }

        public double Height => BlocksColors.GetLength(0) * BlockSize;
        public double Width => BlocksColors.GetLength(1) * BlockSize;

        public Pallet(int rows, int cols, double blockSize)
        {
            BlocksColors = new int[rows, cols];
            BlockSize = blockSize;
        }

        public Pallet(int[,] blocks, double blockSize)
        {
            BlocksColors = blocks;
            BlockSize = blockSize;
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

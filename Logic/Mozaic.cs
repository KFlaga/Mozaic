using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MozaicLand
{
    // Mozaic is a matrix of thightly placed Pallets, representing whole picture.
    // May represent both target state and partially assembled state by assigning null to some of pallets.
    public class Mozaic
    {
        public Pallet[,] Palletes { get; private set; }

        // Assumes all Palletes have same size
        public SizeF PalletSize => Palletes.Length > 0 ? Palletes[0, 0].Size : new SizeF();
        public SizeF Size => new SizeF(PalletSize.Width * Palletes.Cols(), PalletSize.Height * Palletes.Rows());

        public Mozaic(int rows, int cols, int palletRows, int palletCols, float blockSize)
        {
            Palletes = new Pallet[rows, cols];
            for(int r = 0; r < rows; ++r)
            {
                for (int c = 0; c < cols; ++c)
                {
                    Palletes[r, c] = new Pallet(palletRows, palletCols, blockSize);
                }
            }
        }

        public Mozaic(Pallet[,] pallets)
        {
            Palletes = pallets;
        }

        public int PositionToPalletIndex(int row, int col)
        {
            return col + row * Palletes.Cols();
        }

        public int CountBlocksWithColor(int colorIndex)
        {
            return Palletes.ToEnumerable().Aggregate(0, (acc, pallet) => acc + pallet.CountBlocksWithColor(colorIndex));
        }

        public Dictionary<int, int> CountColors()
        {
            Dictionary<int, int> colors = new Dictionary<int, int>();

            foreach (Pallet pallet in Palletes)
            {
                foreach (int color in pallet.BlocksColors)
                {
                    if (colors.ContainsKey(color))
                    {
                        colors[color] += 1;
                    }
                    else
                    {
                        colors.Add(color, 1);
                    }
                }
            }

            return colors;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MozaicLand
{
    public class ColorCartridgeSlot
    {
        public ColorCartridge Cartridge { get; set; }
        public PointF TopLeft { get; set; }
        public SizeF Size { get; set; }

        public PointF CatchHole
        {
            get
            {
                return new PointF(TopLeft.X + Size.Width / 2, TopLeft.Y + Size.Height / 2);
            }
        }
    }

    public class PalletSlot
    {
        public Pallet Pallet { get; set; }
        public PointF TopLeft { get; set; }
        public SizeF Size { get; set; }
    }
 
    // Represents robot with fixed position which may move its effector to different positions and catch/put blocks.
    public class Robot
    {
        PointF center;
        public PointF Center
        {
            get { return center; }
            set
            {
                center = value;
                Effector = value;
            }
        }
        public float MovementSpeed { get; set; } // Units/second
        public TimeSpan CatchBlockTime { get; set; }
        public TimeSpan PutBlockTime { get; set; }

        public PointF Effector { get; private set; }
        public int CurrentBlock { get; private set; }
        
        // Moves effector to given position. Returns time robot needs to reach target.
        // Assumes constant speed and linear movement.
        public TimeSpan MoveTo(PointF target)
        {
            float distance = Effector.DistanceTo(target);
            Effector = target;
            return TimeSpan.FromSeconds(distance / MovementSpeed);
        }

        public TimeSpan CatchBlock(int color)
        {
            if(CurrentBlock != -1)
            {
                throw new InvalidOperationException("Attempt to catch block with non-empty effector");
            }
            CurrentBlock = color;
            return CatchBlockTime;
        }

        public TimeSpan PutBlock()
        {
            if (CurrentBlock != -1)
            {
                throw new InvalidOperationException("Attempt to put block with empty effector");
            }
            CurrentBlock = -1;
            return PutBlockTime;
        }
    }

    // Working space consists of 8 pallets forming a rectangle with robot in a middle, surrounded by color cartridges.
    // Cartridges are placed in qually space slots, so that in one row/column can be fit: cartridge, 3 x pallete, cartridge.
    // Cartridges which are placed on left/right side are turned by 90deg.
    public class WorkingSpace
    {
        public PalletSlot[,] Pallets { get; private set; } = new PalletSlot[3, 3]; // Pallets[1, 1] is always null
        public ColorCartridgeSlot[,] ColorCartridges { get; private set; } // Only first and last row and column is used, except corners:
                                                                           // [0, k], [k, 0], [last, k] and [k, last] are ok for k != 0 and k != last
        public Robot Robot { get; private set; }
        public SizeF Size { get; private set; }

        // TODO: consider adding some space between elements
        public WorkingSpace(Robot robot, SizeF palletSize, SizeF cartridgeSize)
        {
            float topSideWidth = 3 * palletSize.Width;
            float totalWidth = topSideWidth + 2 * cartridgeSize.Height;

            float leftSideHeight = 3 * palletSize.Height;
            float totalHeight = leftSideHeight + 2 * cartridgeSize.Height; // Left side cartridge is turned by 90deg, so we use Height again

            Size = new SizeF(totalWidth, totalHeight);

            PointF center = new PointF(Size.Width / 2, Size.Height / 2);
            Robot = robot;
            Robot.Center = center;

            InitPalletes(palletSize, cartridgeSize);
            InitCartridges(cartridgeSize, topSideWidth, leftSideHeight);
        }

        private void InitPalletes(SizeF palletSize, SizeF cartridgeSize)
        {
            PointF palletesTopLeft = new PointF(cartridgeSize.Height, cartridgeSize.Height);
            Pallets.Fill((r, c) =>
            {
                if (r == 1 && c == 1) { return null; }
                return new PalletSlot()
                {
                    Pallet = null,
                    Size = palletSize,
                    TopLeft = palletesTopLeft.Add(new PointF(c * palletSize.Width, r * palletSize.Height))
                };
            });
        }

        private void InitCartridges(SizeF cartridgeSize, float topSideWidth, float leftSideHeight)
        {
            int cartridgesInRow = (int)(topSideWidth / cartridgeSize.Width);
            int cartridgesInCol = (int)(leftSideHeight / cartridgeSize.Width);
            if (cartridgesInRow == 0 && cartridgesInCol == 0)
            {
                throw new ArgumentException("Color cartridge is bigger than 3 palletes");
            }

            ColorCartridges = new ColorCartridgeSlot[cartridgesInRow + 2, cartridgesInCol + 2];
            
            float spacingInRow = cartridgesInRow > 1 ? (topSideWidth - cartridgesInRow * cartridgeSize.Width) / cartridgesInRow : 0;

            float rowLeft = cartridgeSize.Height;
            float topRowTop = 0;
            float botRowTop = cartridgeSize.Height + leftSideHeight;
            
            for (int i = 0; i < cartridgesInRow; ++i)
            {
                float left = rowLeft + i * (cartridgeSize.Width + spacingInRow);
                ColorCartridges[0, 1 + i] = new ColorCartridgeSlot()
                {
                    Cartridge = null,
                    Size = cartridgeSize,
                    TopLeft = new PointF(left, topRowTop)
                };
                ColorCartridges[ColorCartridges.Rows() - 1, 1 + i] = new ColorCartridgeSlot()
                {
                    Cartridge = null,
                    Size = cartridgeSize,
                    TopLeft = new PointF(left, topRowTop)
                };
            }

            float spacingInCol = cartridgesInCol > 1 ? (leftSideHeight - cartridgesInCol * cartridgeSize.Width) / cartridgesInCol : 0;

            float colTop = cartridgeSize.Height;
            float leftColLeft = 0;
            float rightColLeft = cartridgeSize.Height + topSideWidth;
            SizeF turnedSize = new SizeF(cartridgeSize.Height, cartridgeSize.Width);

            for (int i = 0; i < cartridgesInCol; ++i)
            {
                float top = colTop + i * (cartridgeSize.Width + spacingInCol);
                ColorCartridges[1 + i, 0] = new ColorCartridgeSlot()
                {
                    Cartridge = null,
                    Size = turnedSize,
                    TopLeft = new PointF(leftColLeft, top)
                };
                ColorCartridges[1 + i, ColorCartridges.Cols() - 1] = new ColorCartridgeSlot()
                {
                    Cartridge = null,
                    Size = turnedSize,
                    TopLeft = new PointF(rightColLeft, top)
                };
            }
        }
    }
}

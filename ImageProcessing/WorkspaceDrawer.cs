using Emgu.CV;
using Emgu.CV.Structure;
using MozaicLand;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageProcessing
{
    public class WorkspaceDrawer
    {
        public Image<Bgr, byte> Drawing { get; private set; }

        public SizeF WorkspaceSize { get; set; }
        public float BlockSize { get; set; }
        public float FrameSize { get; set; }
        public float PalletesSpacing { get; set; }

        public ColorTable ColorTable { get; set; }

        public Color FrameColor { get; set; } = Color.Gray;
        
        public WorkspaceDrawer(int rows, int cols, Color bg)
        {
            Drawing = new Image<Bgr, byte>(rows, cols, new Bgr(bg));
        }

        public void DrawPallet(Pallet pallet, PointF topLeft)
        {
            DrawRect(new Bgr(FrameColor), topLeft, pallet.Size);
            for(int r = 0; r < pallet.BlocksColors.Rows(); ++r)
            {
                for (int c = 0; c < pallet.BlocksColors.Cols(); ++c)
                {
                    PointF p = new PointF(
                        pallet.BlockTopLeft(r, c).X + topLeft.X,
                        pallet.BlockTopLeft(r, c).Y + topLeft.Y
                    );

                    DrawRect(new Bgr(ColorTable.Color(pallet.BlocksColors[r, c])), p, new SizeF(BlockSize, BlockSize));
                }
            }
        }

        public void DrawColorCardridges(List<ColorCartridgeSlot> cartridges)
        {
            foreach(var c in cartridges)
            {
                if(c.Cartridge != null)
                {
                    DrawColorCardridge(
                        c.Cartridge.CurrentCount == 0 ? -1 : c.Cartridge.ColorIndex,
                        c.TopLeft,
                        c.Size
                    );
                }
            }
        }

        public void DrawColorCardridge(int colorIndex, PointF topLeft, SizeF size)
        {
            if (size.Height > 16 && size.Width > 16)
            {
                // Draw frame of cartridge
                DrawRect(new Bgr(Color.Black), topLeft, size);
                size = new SizeF(size.Width - 8, size.Height - 8);
                topLeft = new PointF(topLeft.X + 4, topLeft.Y + 4);
            }

            if (colorIndex == -1)
            {
                // Empty cardridge
                DrawCross(new Bgr(Color.White), topLeft, size);
            }
            else
            {
                Color color = ColorTable.Color(colorIndex);
                DrawRect(new Bgr(color), topLeft, size);
            }
        }

        public void DrawRobot(PointF topLeft, SizeF size, double armAngle = 0.0, double effectorAngle = 0.0)
        {
            PointF center = new PointF(topLeft.X + size.Width / 2, topLeft.Y + size.Height / 2);

            // Base
            RotatedRect rect = new RotatedRect(ToPixel(center), ToPixel(size), 0);
            CvInvoke.Ellipse(Drawing, rect, new Bgr(Color.Black).MCvScalar, 1);

            rect = new RotatedRect(ToPixel(center), ToPixel(new SizeF(size.Width / 2, size.Height / 2)), 0);
            CvInvoke.Ellipse(Drawing, rect, new Bgr(Color.DarkOrange).MCvScalar, 4);

            // Arm
            PointF effectorCenter = new PointF(
                center.X - 0.8f * size.Width * (float)Math.Sin(armAngle),
                center.Y - 0.8f * size.Width * (float)Math.Cos(armAngle)
            );
            CvInvoke.Line(Drawing, ToPixel(center), ToPixel(effectorCenter), new Bgr(Color.Black).MCvScalar, 2);

            // Effector
            float effectorHalfLength = BlockSize * 0.7f;
            float fingerHalfLength = BlockSize * 0.4f;

            PointF leftFingerMiddle = new PointF(
                effectorCenter.X - effectorHalfLength * (float)Math.Cos(effectorAngle),
                effectorCenter.Y + effectorHalfLength * (float)Math.Sin(effectorAngle)
            );
            PointF rightFingerMiddle = new PointF(
                effectorCenter.X + effectorHalfLength * (float)Math.Cos(effectorAngle),
                effectorCenter.Y - effectorHalfLength * (float)Math.Sin(effectorAngle)
            );

            CvInvoke.Line(Drawing, ToPixel(leftFingerMiddle), ToPixel(rightFingerMiddle), new Bgr(Color.Green).MCvScalar, 2);
        }

        private void DrawRect(Bgr color, PointF topLeft, SizeF size)
        {
            Drawing.ROI = new Rectangle(ToPixel(topLeft), ToPixel(size));
            Drawing.SetValue(color);
            Drawing.ROI = Rectangle.Empty;
        }

        private void DrawCross(Bgr color, PointF topLeft, SizeF size)
        {
            CvInvoke.Line(Drawing,
                ToPixel(topLeft),
                ToPixel(new PointF(topLeft.X + size.Width, topLeft.Y + size.Height)),
                color.MCvScalar, 2
           );
            CvInvoke.Line(Drawing,
                ToPixel(new PointF(topLeft.X + size.Width, topLeft.Y)),
                ToPixel(new PointF(topLeft.X, topLeft.Y + size.Height)),
                color.MCvScalar, 2
            );
        }

        private Point ToPixel(PointF p)
        {
            return new Point(
                (int)(Drawing.Cols * p.X / WorkspaceSize.Width),
                (int)(Drawing.Rows * p.Y / WorkspaceSize.Height)
            );
        }

        private Size ToPixel(SizeF s)
        {
            return new Size(
                (int)(Drawing.Cols * s.Width / WorkspaceSize.Width),
                (int)(Drawing.Rows * s.Height / WorkspaceSize.Height)
            );
        }
    }
}

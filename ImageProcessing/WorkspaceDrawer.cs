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

        public ColorTable ColorTable { get; set; }

        public Color FrameColor { get; set; } = Color.Gray;
        
        public WorkspaceDrawer(int rows, int cols, Color bg)
        {
            Drawing = new Image<Bgr, byte>(rows, cols, new Bgr(bg));
        }

        public void Draw(WorkingSpace workingSpace, PointF topLeft)
        {
            for(int i = 0; i < 8; ++i)
            {
                var slot = workingSpace.GetPalletSlot(i);
                if(slot.Pallet != null)
                {
                    Draw(slot.Pallet, topLeft.Add(slot.TopLeft));
                }
            }

            for (int i = 0; i < workingSpace.CartridgesSlotsCount; ++i)
            {
                var slot = workingSpace.GetCartridgeSlot(i);
                if (slot.Cartridge != null)
                {
                    Draw(slot.Cartridge, topLeft.Add(slot.TopLeft), slot.Size);
                }
            }

            Draw(workingSpace.Robot, topLeft, workingSpace.Pallets[0, 0].Size);
        }

        public void Draw(Pallet pallet, PointF topLeft)
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

                    int colorIndex = pallet.BlocksColors[r, c];
                    if(colorIndex < 0)
                    {
                        DrawCross(new Bgr(Color.Black), p, new SizeF(pallet.BlockSize, pallet.BlockSize));
                    }
                    else
                    {
                        DrawRect(new Bgr(ColorTable.Color(colorIndex)), p, new SizeF(pallet.BlockSize, pallet.BlockSize));
                    }
                }
            }
        }

        public void Draw(List<ColorCartridgeSlot> cartridges)
        {
            foreach(var c in cartridges)
            {
                if(c.Cartridge != null)
                {
                    Draw(c.Cartridge, c.TopLeft, c.Size);
                }
            }
        }

        public void Draw(ColorCartridge c, PointF topLeft, SizeF size)
        {
            int colorIndex = c.CurrentCount == 0 ? -1 : c.ColorIndex;
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

        public void Draw(Robot robot, PointF offset, SizeF size)
        {
            PointF center = robot.Center.Add(offset);
            float armAngle = 0;
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
            //float effectorHalfLength = size.Width * 0.07f;
            //float fingerHalfLength = size.Width * 0.04f;

            //PointF leftFingerMiddle = new PointF(
            //    effectorCenter.X - effectorHalfLength * (float)Math.Cos(effectorAngle),
            //    effectorCenter.Y + effectorHalfLength * (float)Math.Sin(effectorAngle)
            //);
            //PointF rightFingerMiddle = new PointF(
            //    effectorCenter.X + effectorHalfLength * (float)Math.Cos(effectorAngle),
            //    effectorCenter.Y - effectorHalfLength * (float)Math.Sin(effectorAngle)
            //);

            //CvInvoke.Line(Drawing, ToPixel(leftFingerMiddle), ToPixel(rightFingerMiddle), new Bgr(Color.Green).MCvScalar, 2);
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

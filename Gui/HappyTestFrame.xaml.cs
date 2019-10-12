using Emgu.CV;
using Emgu.CV.Structure;
using ImageProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MozaicLand
{
    public partial class HappyTestFrame : UserControl
    {
        public HappyTestFrame()
        {
            InitializeComponent();

            interpolationMethodInput.ItemsSource = Enum.GetValues(typeof(Emgu.CV.CvEnum.Inter)).Cast<Emgu.CV.CvEnum.Inter>();
            interpolationMethodInput.SelectedIndex = 0;
        }

        struct Sizes
        {
            public static readonly Sizes InvalidSize = new Sizes();

            public double realHeight;
            public double realWidth;
            public double blockSize;
            public double frameSize;

            public bool IsInvalid()
            {
                return realHeight == 0.0;
            }
        }

        Image<Bgr, byte> loadedImage;
        Image<Bgr, byte> fragmentedImage;
        Image<Bgr, byte> quantizedImage;
        Image<Bgr, byte> framedImage;

        ColorTable testTable = new ColorTable(new Dictionary<int, BlockColor>()
        {
            { 0, new BlockColor{ Color = Color.FromArgb(0, 0, 0) } },
            { 1, new BlockColor{ Color = Color.FromArgb(255, 0, 0) } },
            { 2, new BlockColor{ Color = Color.FromArgb(0, 255, 0) } },
            { 3, new BlockColor{ Color = Color.FromArgb(0, 0, 255) } },
            { 4, new BlockColor{ Color = Color.FromArgb(255, 255, 0) } },
            { 5, new BlockColor{ Color = Color.FromArgb(0, 255, 255) } },
            { 6, new BlockColor{ Color = Color.FromArgb(255, 0, 255) } },
            { 7, new BlockColor{ Color = Color.FromArgb(255, 255, 255) } }
        });

        private void LoadImage(object sender, RoutedEventArgs e)
        {
            loadedImage = ImageLoader.FromFile();
            if (loadedImage != null)
            {
                imageViewer.Source = ImageLoader.ImageSourceForBitmap(loadedImage.Bitmap);
                fragmentedImage = null;
                framedImage = null;
                quantizedImage = null;
            }
        }

        private void FragmentImage(object sender, RoutedEventArgs e)
        {
            if (loadedImage == null)
            {
                MessageBox.Show("Image needs to be loaded first");
                return;
            }

            if (FragmentImage(loadedImage) && AddFrames(fragmentedImage))
            {
                UpdateImage(framedImage);
            }
        }

        private void QuantizeImage(object sender, RoutedEventArgs e)
        {
            if (loadedImage == null)
            {
                MessageBox.Show("Image needs to be loaded first");
                return;
            }

            if (fragmentedImage != null)
            {
                // As quantization is very slow for now, execute it on fragmented image, not framed one as it is smaller
                // and add frames later
                if (QuantizeImage(fragmentedImage) && AddFrames(quantizedImage))
                {
                    UpdateImage(framedImage);
                }
            }
            else
            {
                // Image wasn't fragmented, so quantize raw one
                QuantizeImage(loadedImage);
                UpdateImage(quantizedImage);
            }
        }

        private Sizes ReadSizes()
        {
            if (!double.TryParse(blockSizeInput.Text, out double blockSize))
            {
                MessageBox.Show("Invalid 'Block size' - expected a number");
                return Sizes.InvalidSize;
            }
            if (!double.TryParse(frameSizeInput.Text, out double frameSize))
            {
                MessageBox.Show("Invalid 'Frame size' - expected a number");
                return Sizes.InvalidSize;
            }
            if (!double.TryParse(realHeightInput.Text, out double realHeight))
            {
                MessageBox.Show("Invalid 'Real height' - expected a number");
                return Sizes.InvalidSize;
            }
            if (!double.TryParse(realWidthInput.Text, out double realWidth))
            {
                MessageBox.Show("Invalid 'Real width' - expected a number");
                return Sizes.InvalidSize;
            }
            return new Sizes()
            {
                realHeight = realHeight,
                realWidth = realWidth,
                blockSize = blockSize,
                frameSize = frameSize
            };
        }

        private bool FragmentImage(Image<Bgr, byte> i)
        {
            Sizes sizes = ReadSizes();
            if (sizes.IsInvalid())
            {
                return false;
            }

            Emgu.CV.CvEnum.Inter interpolationMethod = (Emgu.CV.CvEnum.Inter)interpolationMethodInput.SelectedItem;

            FragmentationResult result = Fragmenter.FragmentImage(
                i,
                new SizeF((float)sizes.realWidth, (float)sizes.realHeight),
                sizes.blockSize, sizes.frameSize,
                interpolationMethod
            );
            fragmentedImage = result.Blocks;
            return true;
        }

        private bool AddFrames(Image<Bgr, byte> i)
        {
            Sizes sizes = ReadSizes();
            if (sizes.IsInvalid())
            {
                return false;
            }

            int pixelsPerBlock = 4;
            int pixelsPerFrame = (int)Math.Ceiling(pixelsPerBlock * sizes.frameSize / sizes.blockSize);
            framedImage = Fragmenter.DrawWithFrames(i, pixelsPerBlock, pixelsPerFrame, Color.White);
            return true;
        }

        private bool QuantizeImage(Image<Bgr, byte> i)
        {
            quantizedImage = ColorQuantization.AssignBestMatchColors(i, testTable);
            return true;
        }

        private void UpdateImage(Image<Bgr, byte> i)
        {
            imageViewer.Source = ImageLoader.ImageSourceForBitmap(i.Bitmap);
        }

        private Random random = new Random();
        private int NextColor()
        {
            return random.Next(0, 8);
        }

        private Pallet RandPallet(int rows, int cols, float bs, float fs)
        {
            Pallet pallet = new Pallet(rows, cols, bs, fs);
            for (int r = 0; r < rows; ++r)
            {
                for (int c = 0; c < cols; ++c)
                {
                    pallet.BlocksColors[r, c] = NextColor();
                }
            }
            return pallet;
        }

        private void DrawWorkspace(object sender, RoutedEventArgs e)
        {
            WorkspaceDrawer drawer = new WorkspaceDrawer(400, 400, Color.White)
            {
                WorkspaceSize = new SizeF(1000, 1000),
                BlockSize = 20,
                FrameSize = 5,
                PalletesSpacing = 20,
                FrameColor = Color.Gray,
                ColorTable = testTable
            };

            // Pallet size = 130 x 130

            drawer.DrawPallet(RandPallet(5, 5, 20, 5), new PointF(100, 100));
            drawer.DrawPallet(RandPallet(5, 5, 20, 5), new PointF(250, 100));
            drawer.DrawPallet(RandPallet(5, 5, 20, 5), new PointF(400, 100));
            drawer.DrawPallet(RandPallet(5, 5, 20, 5), new PointF(400, 250));
            drawer.DrawPallet(RandPallet(5, 5, 20, 5), new PointF(400, 400));
            drawer.DrawPallet(RandPallet(5, 5, 20, 5), new PointF(250, 400));
            drawer.DrawPallet(RandPallet(5, 5, 20, 5), new PointF(100, 400));
            drawer.DrawPallet(RandPallet(5, 5, 20, 5), new PointF(100, 250));

            float deg45 = (float)(Math.PI / 4);
            drawer.DrawRobot(new PointF(265, 265), new SizeF(100, 100), -deg45, -deg45);

            SizeF cartridgeSize = new SizeF(70, 50);
            List<ColorCartridgeSlot> cartridges = new List<ColorCartridgeSlot>()
            {
                new ColorCartridgeSlot()
                {
                    TopLeft = new PointF(100, 40),
                    Size = cartridgeSize,
                    Cartridge = new ColorCartridge(NextColor(), 10)
                },
                new ColorCartridgeSlot()
                {
                    TopLeft = new PointF(180, 40),
                    Size = cartridgeSize,
                    Cartridge = new ColorCartridge(NextColor(), 0)
                },
                new ColorCartridgeSlot()
                {
                    TopLeft = new PointF(260, 40),
                    Size = cartridgeSize,
                    Cartridge = new ColorCartridge(NextColor(), 10)
                },
                new ColorCartridgeSlot()
                {
                    TopLeft = new PointF(340, 40),
                    Size = cartridgeSize,
                    Cartridge = new ColorCartridge(NextColor(), 10)
                },
                new ColorCartridgeSlot()
                {
                    TopLeft = new PointF(540, 100),
                    Size = new SizeF(cartridgeSize.Height, cartridgeSize.Width),
                    Cartridge = new ColorCartridge(NextColor(), 10)
                },
                new ColorCartridgeSlot()
                {
                    TopLeft = new PointF(540, 180),
                    Size = new SizeF(cartridgeSize.Height, cartridgeSize.Width),
                    Cartridge = new ColorCartridge(NextColor(), 10)
                },
            };

            drawer.DrawColorCardridges(cartridges);

            UpdateImage(drawer.Drawing);
        }
    }
}

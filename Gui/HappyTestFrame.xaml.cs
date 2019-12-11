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
                return  realHeight == 0.0;
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

        AssemblingSequence testSequence;

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
                FrameColor = Color.Gray,
                ColorTable = testTable
            };

            SizeF palletSize = new SizeF(250, 125);
            SizeF cartridgeSize = new SizeF(70, 50);

            WorkingSpace workingSpace = new WorkingSpace(new Robot(), palletSize, cartridgeSize, 10.0f);

            for(int i = 0; i < 8; ++i)
            {
                workingSpace.GetPalletSlot(i).Pallet = RandPallet(5, 5, 20, 5);
            }

            for(int i = 0; i < workingSpace.CartridgesSlotsCount; ++i)
            {
                workingSpace.GetCartridgeSlot(i).Cartridge = new ColorCartridge(NextColor(), 10);
            }

            workingSpace.GetCartridgeSlot(3).Cartridge = new ColorCartridge(NextColor(), 0);

            drawer.Draw(workingSpace, new PointF(200, 200));

            UpdateImage(drawer.Drawing);
        }

        private IAssemblingAction GetNextBlock(Robot robot, WorkingSpace workingSpace)
        {
            // First non-empty cartridge
            for(int i = 0; i < workingSpace.CartridgesSlotsCount; ++i)
            {
                var slot = workingSpace.GetCartridgeSlot(i);
                if (slot.Cartridge != null && slot.Cartridge.CurrentCount > 0)
                {
                    return new GetBlockAction(robot, slot);
                }
            }

            return null;
        }
        
        private IAssemblingAction PutNextBlock(Robot robot, WorkingSpace workingSpace)
        {
            // First non-empty block
            for (int i = 0; i < 8; ++i)
            {
                var slot = workingSpace.GetPalletSlot(i);
                if (slot.Pallet != null)
                {
                    for(int r = 0; r < slot.Pallet.BlocksColors.Rows(); ++r)
                    {
                        for (int c = 0; c < slot.Pallet.BlocksColors.Cols(); ++c)
                        {
                            if(slot.Pallet.BlocksColors[r, c] < 0)
                            {
                                return new PutBlockAction(robot, slot, new System.Drawing.Point(c, r));
                            }
                        }
                    }
                }
            }

            return null;
        }

        private void AssemblyMozaic(object sender, RoutedEventArgs e)
        {
            SizeF palletSize = new SizeF(125, 125);
            SizeF cartridgeSize = new SizeF(70, 50);

            Robot robot = new Robot()
            {
                CatchBlockTime = TimeSpan.FromSeconds(1),
                PutBlockTime = TimeSpan.FromSeconds(1.5),
                MovementSpeed = 5.0f
            };

            WorkingSpace workingSpace = new WorkingSpace(robot, palletSize, cartridgeSize, 10.0f);

            for (int i = 0; i < 8; ++i)
            {
                workingSpace.GetPalletSlot(i).Pallet = new Pallet(5, 5, 20, 5);
            }

            for (int i = 0; i < workingSpace.CartridgesSlotsCount; ++i)
            {
                workingSpace.GetCartridgeSlot(i).Cartridge = new ColorCartridge(NextColor(), 10);
            }

            testSequence = new AssemblingSequence()
            {
                WorkingSpace = workingSpace,
                Actions = new List<IAssemblingAction>()
            };


            WorkspaceDrawer drawer = new WorkspaceDrawer(400, 400, Color.White)
            {
                WorkspaceSize = new SizeF(1000, 1000),
                FrameColor = Color.Gray,
                ColorTable = testTable
            };

            drawer.Draw(testSequence.WorkingSpace, new PointF(200, 200));

            UpdateImage(drawer.Drawing);
        }

        private void NextStep(object sender, RoutedEventArgs e)
        {
            IAssemblingAction action = GetNextBlock(testSequence.WorkingSpace.Robot, testSequence.WorkingSpace);
            if(action != null)
            {
                testSequence.Actions.Add(action);
                testSequence.ExecuteNext();
            }
            action = PutNextBlock(testSequence.WorkingSpace.Robot, testSequence.WorkingSpace);
            if (action != null)
            {
                testSequence.Actions.Add(action);
                testSequence.ExecuteNext();
            }

            timeText.Text = testSequence.ExecutionTime.TotalSeconds.ToString();

            WorkspaceDrawer drawer = new WorkspaceDrawer(400, 400, Color.White)
            {
                WorkspaceSize = new SizeF(1000, 1000),
                FrameColor = Color.Gray,
                ColorTable = testTable
            };

            drawer.Draw(testSequence.WorkingSpace, new PointF(200, 200));

            UpdateImage(drawer.Drawing);
        }
    }
}

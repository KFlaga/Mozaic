using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ImageProcessing;
using MozaicLand;
using System.Drawing;

namespace Gui
{
    public partial class MainWindow : Window
    {
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

        System.Windows.Controls.Image image = new System.Windows.Controls.Image();

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

        public MainWindow()
        {
            InitializeComponent();

            interpolationMethodInput.ItemsSource = Enum.GetValues(typeof(Emgu.CV.CvEnum.Inter)).Cast<Emgu.CV.CvEnum.Inter>();
            interpolationMethodInput.SelectedIndex = 0;

            imageArea.Child = image;
        }

        private void LoadImage(object sender, RoutedEventArgs e)
        {
            loadedImage = ImageLoader.FromFile();
            if (loadedImage != null)
            {
                image.Source = ImageLoader.ImageSourceForBitmap(loadedImage.Bitmap);
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

            if(FragmentImage(loadedImage) && AddFrames(fragmentedImage))
            {
                UpdateImage(framedImage);
            }
        }

        private void QuantizeImage(object sender, RoutedEventArgs e)
        {
            if(loadedImage == null)
            {
                MessageBox.Show("Image needs to be loaded first");
                return;
            }
            
            if(fragmentedImage != null)
            {
                // As quantization is very slow for now, execute it on fragmented image, not framed one as it is smaller
                // and add frames later
                if(QuantizeImage(fragmentedImage) && AddFrames(quantizedImage))
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
            int pixelsPerFrame = (int)Math.Ceiling(sizes.frameSize / sizes.blockSize);
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
            image.Source = ImageLoader.ImageSourceForBitmap(i.Bitmap);
        }
    }
}

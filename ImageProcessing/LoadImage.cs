using Emgu.CV.Structure;
using MozaicLand;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageProcessing
{
    public class CvTest
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public static ImageSource ImageSourceForBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                ImageSource newSource = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(handle);
                return newSource;
            }
            catch (Exception)
            {
                DeleteObject(handle);
                return null;
            }
        }

        public static void Cvtest()
        {
            // Load image from file
            // Resulting image fromat is probably 8bit BGR channels
            Emgu.CV.Mat mat1 = Emgu.CV.CvInvoke.Imread("image.png", Emgu.CV.CvEnum.ImreadModes.Color);

            // Create image manually
            Emgu.CV.Image<Bgr, byte> image = new Emgu.CV.Image<Bgr, byte>(100, 100);
            image.Data[0, 0, 0] = 128;

            // Mat may be needed for some open cv algorithms
            Emgu.CV.Mat mat2 = image.Mat;
            
            // Convert to format which can be shown inside WPF Image control
            ImageSource wpfImage = ImageSourceForBitmap(image.Bitmap);
        }

        public static Emgu.CV.Mat LoadImageFromFile()
        {
            Emgu.CV.Mat loadedImage = null;
            FileOp.LoadFromFile((s, path) =>
            {
                loadedImage = Emgu.CV.CvInvoke.Imread(path, Emgu.CV.CvEnum.ImreadModes.Color);
            });
            return loadedImage;
        }
    }
}

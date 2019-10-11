using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MozaicLand;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using ImageProcessing;

namespace Tests
{
    [TestClass]
    public class ColorQuantizationTests
    {
        ColorTable colorTable;
        Image<Bgr, byte> image;

        [TestInitialize]
        public void Init()
        {
            colorTable = new ColorTable(new Dictionary<int, BlockColor>()
            {
                { 0, new BlockColor{ Color = Color.FromArgb(0, 0, 0) } },
                { 1, new BlockColor{ Color = Color.FromArgb(100, 100, 100) } }
            });

            image = new Image<Bgr, byte>(new byte[2, 2, 3]
            {
                { { 0, 0, 0 }, { 0, 100, 0 } },
                { { 100, 100, 0 }, { 50, 50, 50 } },
            });
        }

        [TestMethod]
        public void BestMatch()
        {
            Assert.AreEqual(new Bgr(0, 0, 0), ColorQuantization.BestMatch(new Bgr(0, 0, 0), colorTable));
            Assert.AreEqual(new Bgr(0, 0, 0), ColorQuantization.BestMatch(new Bgr(0, 100, 0), colorTable));
            Assert.AreEqual(new Bgr(0, 0, 0), ColorQuantization.BestMatch(new Bgr(101, 0, 50), colorTable));
            Assert.AreEqual(new Bgr(0, 0, 0), ColorQuantization.BestMatch(new Bgr(50, 50, 50), colorTable));

            Assert.AreEqual(new Bgr(100, 100, 100), ColorQuantization.BestMatch(new Bgr(100, 100, 100), colorTable));
            Assert.AreEqual(new Bgr(100, 100, 100), ColorQuantization.BestMatch(new Bgr(100, 1, 50), colorTable));
        }

        [TestMethod]
        public void AssignBestMatchColors()
        {
            Image<Bgr, byte> result = ColorQuantization.AssignBestMatchColors(image, colorTable);
            Assert.AreEqual(new Bgr(0, 0, 0), result[0, 0]);
            Assert.AreEqual(new Bgr(0, 0, 0), result[0, 1]);
            Assert.AreEqual(new Bgr(100, 100, 100), result[1, 0]);
            Assert.AreEqual(new Bgr(0, 0, 0), result[1, 1]);
        }
    }
}

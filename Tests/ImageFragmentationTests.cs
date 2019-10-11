using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MozaicLand;
using ImageProcessing;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace Tests
{
    [TestClass]
    public class ImageFragmentationTests
    {
        Image<Bgr, byte> testImage;

        [TestInitialize]
        public void Init()
        {
            testImage = new Image<Bgr, byte>(new byte[4, 4, 3]
            {
                {   { 0, 0, 0 }, { 0, 255, 0 }, { 0, 255, 0 }, { 0, 0, 0 } },
                { { 255, 0, 0 }, { 0, 255, 0 }, { 0, 255, 0 }, { 0, 0, 255 } },
                { { 255, 0, 0 }, { 0, 255, 0 }, { 0, 255, 0 }, { 0, 0, 255 } },
                {   { 0, 0, 0 }, { 0, 255, 0 }, { 0, 255, 0 }, { 0, 0, 0 } },
            });
        }

        [TestMethod]
        public void FragmentImage_upscale()
        {
            FragmentationResult results = Fragmenter.FragmentImage(
                testImage,
                new SizeF(200, 100),
                12, 2
            );
            
            Assert.AreEqual(7, results.Blocks.Rows);
            Assert.AreEqual(14, results.Blocks.Cols);
            Assert.AreEqual(0, results.MarginTop);
            Assert.AreEqual(1, results.MarginLeft);

            // Color values are not tested, as they are mainly dependend on 3-party code
        }

        [TestMethod]
        public void FragmentImage_downscale()
        {
            FragmentationResult results = Fragmenter.FragmentImage(
                testImage,
                new SizeF(200, 100),
                50, 6
            );

            Assert.AreEqual(1, results.Blocks.Rows);
            Assert.AreEqual(3, results.Blocks.Cols);
            Assert.AreEqual(19, results.MarginTop);
            Assert.AreEqual(13, results.MarginLeft);
        }

        [TestMethod]
        public void FragmentImage_blockSizeGreatedThanRealSize()
        {
            FragmentationResult results = Fragmenter.FragmentImage(
                testImage,
                new SizeF(200, 100),
                300, 50
            );

            Assert.IsNull(results);
        }

        [TestMethod]
        public void DrawWithFrames()
        {
            Color frameColor = Color.FromArgb(1, 1, 1);
            Image<Bgr, byte> result = Fragmenter.DrawWithFrames(testImage, 2, 1, frameColor);

            Assert.AreEqual(13, result.Rows);
            Assert.AreEqual(13, result.Cols);

            Assert.AreEqual(new Bgr(frameColor), result[0, 0]);
            Assert.AreEqual(new Bgr(frameColor), result[12, 12]);
            Assert.AreEqual(new Bgr(frameColor), result[3, 2]);

            Assert.AreEqual(new Bgr(255, 0, 0), result[7, 1]);
            Assert.AreEqual(new Bgr(0, 0, 0), result[11, 11]);
            Assert.AreEqual(new Bgr(0, 255, 0), result[7, 7]);
        }
    }
}

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MozaicLand;

namespace Tests
{
    [TestClass]
    public class MozaicTests
    {
        Mozaic mozaic;

        [TestInitialize]
        public void Init()
        {
            Units.mm size = (Units.mm)20.0;

            mozaic = new Mozaic(new Pallet[2, 2]
            {
                {
                    new Pallet(new int[2,2] { { 1, 2 }, { 3, 4 } }, size),
                    new Pallet(new int[2,2] { { 2, 3 }, { 4, 5 } }, size),
                },
                {
                    new Pallet(new int[1,2] { { 5, 5 } }, size),
                    new Pallet(new int[1,2] { { 1, 2 } }, size),
                }
            });
        }

        [TestMethod]
        public void CountBlocksWithColor()
        {
            Assert.AreEqual(3, mozaic.CountBlocksWithColor(2));
            Assert.AreEqual(2, mozaic.CountBlocksWithColor(3));
            Assert.AreEqual(0, mozaic.CountBlocksWithColor(6));
        }

        [TestMethod]
        public void CountColors()
        {
            Dictionary<int, int> colors = mozaic.CountColors();

            Assert.IsTrue(colors.ContainsKey(5));
            Assert.AreEqual(3, colors[5]);
            Assert.IsTrue(colors.ContainsKey(1));
            Assert.AreEqual(2, colors[1]);
            Assert.IsFalse(colors.ContainsKey(0));
        }
    }
}

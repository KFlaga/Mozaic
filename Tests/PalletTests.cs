using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MozaicLand;

namespace Tests
{
    [TestClass]
    public class PalletTests
    {
        [TestMethod]
        public void CountBlocksWithColor()
        {
            Pallet pallet = new Pallet(new int[2, 2] { { 2, 3 }, { 3, 4 } }, (Units.mm)20.0);

            Assert.AreEqual(1, pallet.CountBlocksWithColor(2));
            Assert.AreEqual(2, pallet.CountBlocksWithColor(3));
            Assert.AreEqual(0, pallet.CountBlocksWithColor(1));
        }

        [TestMethod]
        public void CountColors()
        {
            Pallet pallet = new Pallet(new int[2, 2] { { 2, 3 }, { 3, 4 } }, (Units.mm)20.0);

            Dictionary<int, int> colors = pallet.CountColors();

            Assert.IsTrue(colors.ContainsKey(2));
            Assert.AreEqual(1, colors[2]);
            Assert.IsTrue(colors.ContainsKey(3));
            Assert.AreEqual(2, colors[3]);
            Assert.IsFalse(colors.ContainsKey(1));
        }
    }
}

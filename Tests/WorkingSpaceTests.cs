using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MozaicLand;

namespace Tests
{
    [TestClass]
    public class WorkingSpaceTests
    {
        SizeF palletSize = new SizeF(100, 110);
        SizeF cartridgeSize = new SizeF(22, 15);
        float palletesSeparation = 5.0f;
        WorkingSpace workingSpace;

        [TestInitialize]
        public void Init()
        {
            workingSpace = new WorkingSpace(new Robot(), palletSize, cartridgeSize, palletesSeparation);
        }

        [TestMethod]
        public void Initialization()
        {
            Assert.AreEqual(350.0f, workingSpace.Size.Width);
            Assert.AreEqual(380.0f, workingSpace.Size.Height);

            Assert.AreEqual(175.0f, workingSpace.Robot.Center.X);
            Assert.AreEqual(190.0f, workingSpace.Robot.Center.Y);

            Assert.AreEqual(3, workingSpace.Pallets.Rows());
            Assert.AreEqual(3, workingSpace.Pallets.Cols());

            Assert.AreEqual(100.0f, workingSpace.Pallets[0, 0].Size.Width);
            Assert.AreEqual(110.0f, workingSpace.Pallets[0, 0].Size.Height);
            Assert.AreEqual(20.0f, workingSpace.Pallets[0, 0].TopLeft.X);
            Assert.AreEqual(20.0f, workingSpace.Pallets[0, 0].TopLeft.Y);

            Assert.AreEqual(100.0f, workingSpace.Pallets[2, 2].Size.Width);
            Assert.AreEqual(110.0f, workingSpace.Pallets[2, 2].Size.Height);
            Assert.AreEqual(230.0f, workingSpace.Pallets[2, 2].TopLeft.X);
            Assert.AreEqual(250.0f, workingSpace.Pallets[2, 2].TopLeft.Y);

            Assert.AreEqual(17, workingSpace.CartridgesSlots.Rows());
            Assert.AreEqual(16, workingSpace.CartridgesSlots.Cols());

            Assert.IsNull(workingSpace.CartridgesSlots[0, 0]);
            Assert.IsNull(workingSpace.CartridgesSlots[1, 1]);
            Assert.IsNull(workingSpace.CartridgesSlots[0, 15]);
            Assert.IsNull(workingSpace.CartridgesSlots[16, 15]);
            
            Assert.AreEqual(22.0f, workingSpace.CartridgesSlots[0, 1].Size.Width);
            Assert.AreEqual(15.0f, workingSpace.CartridgesSlots[0, 1].Size.Height);
            Assert.AreEqual(20.0f, workingSpace.CartridgesSlots[0, 1].TopLeft.X);
            Assert.AreEqual(0.0f, workingSpace.CartridgesSlots[0, 1].TopLeft.Y);

            Assert.AreEqual(22.0f, workingSpace.CartridgesSlots[0, 2].Size.Width);
            Assert.AreEqual(15.0f, workingSpace.CartridgesSlots[0, 2].Size.Height);
            Assert.AreEqual(42.1538467f, workingSpace.CartridgesSlots[0, 2].TopLeft.X);
            Assert.AreEqual(0.0f, workingSpace.CartridgesSlots[0, 2].TopLeft.Y);

            Assert.AreEqual(22.0f, workingSpace.CartridgesSlots[16, 14].Size.Width);
            Assert.AreEqual(15.0f, workingSpace.CartridgesSlots[16, 14].Size.Height);
            Assert.AreEqual(308.0f, workingSpace.CartridgesSlots[16, 14].TopLeft.X);
            Assert.AreEqual(365.0f, workingSpace.CartridgesSlots[16, 14].TopLeft.Y);

            Assert.AreEqual(15.0f, workingSpace.CartridgesSlots[2, 0].Size.Width);
            Assert.AreEqual(22.0f, workingSpace.CartridgesSlots[2, 0].Size.Height);
            Assert.AreEqual(0.0f, workingSpace.CartridgesSlots[2, 0].TopLeft.X);
            Assert.AreEqual(42.7142868f, workingSpace.CartridgesSlots[2, 0].TopLeft.Y);

            Assert.AreEqual(15.0f, workingSpace.CartridgesSlots[2, 15].Size.Width);
            Assert.AreEqual(22.0f, workingSpace.CartridgesSlots[2, 15].Size.Height);
            Assert.AreEqual(335.0f, workingSpace.CartridgesSlots[2, 15].TopLeft.X);
            Assert.AreEqual(42.7142868f, workingSpace.CartridgesSlots[2, 15].TopLeft.Y);
        }
        
        [TestMethod]
        public void GetColorCartridge()
        {
            Assert.AreEqual(58, workingSpace.CartridgesSlotsCount);

            Assert.AreEqual(20.0f, workingSpace.GetCartridgeSlot(0).TopLeft.X);
            Assert.AreEqual(0.0f, workingSpace.GetCartridgeSlot(0).TopLeft.Y);
            Assert.AreEqual(308.0f, workingSpace.GetCartridgeSlot(13).TopLeft.X);
            Assert.AreEqual(0.0f, workingSpace.GetCartridgeSlot(13).TopLeft.Y);
            Assert.AreEqual(335.0f, workingSpace.GetCartridgeSlot(14).TopLeft.X);
            Assert.AreEqual(20.0f, workingSpace.GetCartridgeSlot(14).TopLeft.Y);
            Assert.AreEqual(308.0f, workingSpace.GetCartridgeSlot(29).TopLeft.X);
            Assert.AreEqual(365.0f, workingSpace.GetCartridgeSlot(29).TopLeft.Y);
            Assert.AreEqual(0.0f, workingSpace.GetCartridgeSlot(43).TopLeft.X);
            Assert.AreEqual(338.0f, workingSpace.GetCartridgeSlot(43).TopLeft.Y);
            Assert.AreEqual(0.0f, workingSpace.GetCartridgeSlot(57).TopLeft.X);
            Assert.AreEqual(20.0f, workingSpace.GetCartridgeSlot(57).TopLeft.Y);

            Assert.ThrowsException<ArgumentException>(() => workingSpace.GetCartridgeSlot(58));
            Assert.ThrowsException<ArgumentException>(() => workingSpace.GetCartridgeSlot(-1));
        }
    }
}

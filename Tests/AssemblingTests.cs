using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MozaicLand;

namespace Tests
{
    [TestClass]
    public class AssemblingTests
    {
        float blockSize = 5;
        SizeF palletSize = new SizeF(100, 100);
        SizeF cartridgeSize = new SizeF(20, 20);
        WorkingSpace workingSpace;
        Robot robot;
        float speed = 10;

        [TestInitialize]
        public void Init()
        {
            robot = new Robot()
            {
                CatchBlockTime = TimeSpan.FromSeconds(1),
                PutBlockTime = TimeSpan.FromSeconds(2),
                MovementSpeed = speed
            };

            workingSpace = new WorkingSpace(robot, palletSize, cartridgeSize, 5.0f);
            for(int i = 0; i < 8; ++i)
            {
                workingSpace.GetPalletSlot(i).Pallet = new Pallet(20, 20, blockSize, 0);
            }

            for(int i = 0; i < workingSpace.CartridgesSlotsCount; ++i)
            {
                workingSpace.GetCartridgeSlot(i).Cartridge = new ColorCartridge(i, 10);
            }

            workingSpace.GetCartridgeSlot(5).Cartridge = null;
            workingSpace.GetCartridgeSlot(10).Cartridge = new ColorCartridge(-1, 0);
            workingSpace.GetCartridgeSlot(15).Cartridge = new ColorCartridge(0, 0);
        }

        [TestMethod]
        public void GetBlock()
        {
            robot.MoveTo(robot.Center);

            GetBlockAction action = new GetBlockAction(robot, workingSpace.GetCartridgeSlot(0));

            TimeSpan endTime = action.Execute(TimeSpan.FromSeconds(1));

            Assert.AreEqual(workingSpace.GetCartridgeSlot(0).CatchHole, workingSpace.Robot.Effector);

            float distance = 223.439026f;
            TimeSpan moveTime = TimeSpan.FromSeconds(distance / speed);

            Assert.AreEqual(moveTime + TimeSpan.FromSeconds(2), endTime);

            Assert.AreEqual(0, robot.CurrentBlock);
            Assert.AreEqual(9, workingSpace.GetCartridgeSlot(0).Cartridge.CurrentCount);
        }

        [TestMethod]
        public void PutBlock()
        {
            robot.MoveTo(robot.Center);
            robot.CatchBlock(4);

            PutBlockAction action = new PutBlockAction(robot, workingSpace.Pallets[0, 2], new Point(2, 2));

            TimeSpan endTime = action.Execute(TimeSpan.FromSeconds(1));

            Assert.AreEqual(new PointF(247.5f, 37.5f), workingSpace.Robot.Effector);

            float distance = 157.678467f;
            TimeSpan moveTime = TimeSpan.FromSeconds(distance / speed);

            Assert.AreEqual(moveTime + TimeSpan.FromSeconds(3), endTime);

            Assert.AreEqual(-1, robot.CurrentBlock);
            Assert.AreEqual(4, workingSpace.Pallets[0, 2].Pallet.BlocksColors[2, 2]);
        }

        [TestMethod]
        public void Sequence()
        {
            robot.MoveTo(robot.Center);

            AssemblingSequence sequence = new AssemblingSequence()
            {
                WorkingSpace = workingSpace,
                ExecutionTime = TimeSpan.Zero,
                Actions = new List<IAssemblingAction>()
                {
                    new GetBlockAction(robot, workingSpace.GetCartridgeSlot(0)),
                    new PutBlockAction(robot, workingSpace.Pallets[0, 2], new Point(2, 2)),
                    new GetBlockAction(robot, workingSpace.GetCartridgeSlot(1)),
                    new PutBlockAction(robot, workingSpace.Pallets[1, 0], new Point(4, 4))
                }
            };

            sequence.ExecuteAll();
            
            Assert.AreEqual(-1, robot.CurrentBlock);
            Assert.AreEqual(0, workingSpace.Pallets[0, 2].Pallet.BlocksColors[2, 2]);
            Assert.AreEqual(1, workingSpace.Pallets[1, 0].Pallet.BlocksColors[4, 4]);
        }
    }
}

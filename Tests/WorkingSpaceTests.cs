using System.Collections.Generic;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MozaicLand;

namespace Tests
{
    [TestClass]
    public class WorkingSpaceTests
    {
        WorkingSpace workingSpace;

        [TestInitialize]
        public void Init()
        {
            // TODO
            workingSpace = new WorkingSpace(new Robot(), new SizeF(100, 100), new SizeF(10, 10));
        }

        [TestMethod]
        public void InitializationOfPalletes()
        {
            // TODO
        }

        [TestMethod]
        public void InitializationOfCartridges()
        {
            // TODO
        }
    }
}

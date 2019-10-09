using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MozaicLand;

namespace Tests
{
    [TestClass]
    public class ColorTableTests
    {
        [TestMethod]
        public void LoadFromText()
        {
            string input = "TODO";
            
            ColorTable colorTable = new ColorTable(input);
        }
    }
}

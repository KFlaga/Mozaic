using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MozaicLand;

namespace Tests
{
    [TestClass]
    public class MiscTests
    {
        [TestMethod]
        public void MinMaxElement()
        {
            List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>
            {
                new KeyValuePair<int, int>(2, 3),
                new KeyValuePair<int, int>(5, 7),
                new KeyValuePair<int, int>(4, 6),
                new KeyValuePair<int, int>(-5, 4)
            };

            KeyValuePair<int, int> min = list.MinElement((kv) => kv.Value);
            Assert.AreEqual(2, min.Key);
            Assert.AreEqual(3, min.Value);

            KeyValuePair<int, int> max = list.MaxElement((kv) => kv.Key);
            Assert.AreEqual(5, max.Key);
            Assert.AreEqual(7, max.Value);
        }
    }
}

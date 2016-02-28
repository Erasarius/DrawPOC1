using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

namespace DrawPOC1 {

    [TestFixture]
    public class QuickMedianTest {

        [Test]
        public void MedianTest() {
            List<float> vals1 = new List<float> {
                5.1F, -2.0F, 10.5F, 3.2F, -1.2F, -2.0F, 4.0F
            };

            Assert.AreEqual(QuickMedian.Median(vals1), 3.2F);

            List<float> vals2 = new List<float> {
                5.1F, -2.0F, 10.5F, 3.2F, -1.2F, -2.0F, 4.0F, 11.0F
            };

            Assert.AreEqual(QuickMedian.Median(vals1), 3.2F);
        }
    }
};

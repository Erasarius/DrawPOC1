using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

namespace DrawPOC1 {

    [TestFixture]
    public class ShortStrawTest {

        [Test]
        public void getBoundingBoxTest() {
            ShortStraw shortStraw = new ShortStraw();

            List<Vector3> pList = new List<Vector3> {
                new Vector3(-1.0F, 0.0F),
                new Vector3(1.0F, 0.0F),
                new Vector3(0.0F, 2.0F),
                new Vector3(0.0F, -2.0F)
            };

            Vector3[] box = shortStraw.getBoundingBox(pList);
            Assert.AreEqual(box[0].x, -1.0F);
            Assert.AreEqual(box[0].y, 2.0F);
            Assert.AreEqual(box[1].x, 1.0F);
            Assert.AreEqual(box[1].y, -2.0F);
        }

        [Test]
        public void getResampleSpacingTest() {
            ShortStraw shortStraw = new ShortStraw();

            List<Vector3> pList = new List<Vector3> {
                new Vector3(-1.0F, 0.0F),
                new Vector3(2.0F, 0.0F),
                new Vector3(0.0F, 2.0F),
                new Vector3(0.0F, -2.0F)
            };

            float spacing = shortStraw.getResamplingSpacing(pList);
            Assert.AreEqual(spacing, 5.0F / 40.0F);
        }

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

using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;


[TestFixture]
public class RecognizerTest{

    
    [Test]
    public void checkAngleTest() {
        Recognizer recognizer = new Recognizer();

        // horizontal line
        List<Vector3> userStroke = new List<Vector3> {
                new Vector3(-1.0F, 0.0F),
                new Vector3(1.0F, 0.0F)
        };

        List<Vector3> targetStroke = new List<Vector3> {
                new Vector3(-1.0F, 0.0F),
                new Vector3(1.0F, 0.0F)
        };

        Assert.AreEqual(0f, recognizer.checkAngle(userStroke, targetStroke));

        // vertical target vs horizontal user
        targetStroke = new List<Vector3> {
                new Vector3(0F, 1F),
                new Vector3(0F, -1F)
        };

        Assert.AreEqual(-1f, recognizer.checkAngle(userStroke, targetStroke));

        // backwards user
        targetStroke = new List<Vector3> {
                new Vector3(1F, 0F),
                new Vector3(-1F, 0F)
        };
        Assert.AreEqual(-1f, recognizer.checkAngle(userStroke, targetStroke));

        // angle over threshold
        targetStroke = new List<Vector3> {
                new Vector3(0F, 0F),
                new Vector3(1F, 1F)
        };
        Assert.AreEqual(-1f, recognizer.checkAngle(userStroke, targetStroke));

        // angle within threshold
        targetStroke = new List<Vector3> {
                new Vector3(0F, 0F),
                new Vector3(1F, .1F)
        };
        Assert.Greater(recognizer.checkAngle(userStroke, targetStroke), 0f);

        // angle within threshold negative
        targetStroke = new List<Vector3> {
                new Vector3(0F, 0F),
                new Vector3(1F, -.1F)
        };
        Assert.Greater(recognizer.checkAngle(userStroke, targetStroke), 0f);
    }

    [Test]
    public void getAngleTest() {
        Recognizer recognizer = new Recognizer();

        Vector3 p1 = new Vector3(0f, 1f);
        Vector3 p2 = new Vector3(0f, -1f);

        Assert.AreEqual(-90f, recognizer.getAngle(p1, p2));

        p1 = new Vector3(-1f, 0f);
        p2 = new Vector3(1f, 0f);

        Assert.AreEqual(0f, recognizer.getAngle(p1, p2));

        p1 = new Vector3(-1f, -1f);
        p2 = new Vector3(1f, 1f);

        Assert.AreEqual(45f, recognizer.getAngle(p1, p2));
        Assert.AreEqual(-135f, recognizer.getAngle(p2, p1));
    }

    [Test]
    public void checkCornersTest() {
        Recognizer recognizer = new Recognizer();

        // horizontal line
        List<Vector3> userStroke = new List<Vector3> {
                new Vector3(-1.0F, 0.0F),
                new Vector3(1.0F, 0.0F)
        };

        List<Vector3> targetStroke = new List<Vector3> {
                new Vector3(-1.0F, 0.0F),
                new Vector3(1.0F, 0.0F)
        };

        Assert.AreEqual(0, recognizer.checkCorners(userStroke, targetStroke));

        targetStroke = new List<Vector3> {
                new Vector3(-1.0F, 0.0F),
                new Vector3(1.0F, 0.0F),
                new Vector3(1.0f, 1.0f)
        };
        Assert.AreEqual(-1, recognizer.checkCorners(userStroke, targetStroke));
        Assert.AreEqual(0, recognizer.checkCorners(userStroke, targetStroke, true));

        userStroke = new List<Vector3> {
                new Vector3(-1.0F, 0.0F),
                new Vector3(1.0F, 0.0F),
                new Vector3(),
                new Vector3()
        };
        Assert.AreEqual(-1, recognizer.checkCorners(userStroke, targetStroke));
    }

    [Test]
    public void getBoundingBoxTest() {
        List<Vector3> pList = new List<Vector3> {
                new Vector3(-1.0F, 0.0F),
                new Vector3(1.0F, 0.0F),
                new Vector3(0.0F, 2.0F),
                new Vector3(0.0F, -2.0F)
            };

        Vector3[] box = Recognizer.getBoundingBox(pList);
        Assert.AreEqual(box[0].x, -1.0F);
        Assert.AreEqual(box[0].y, 2.0F);
        Assert.AreEqual(box[1].x, 1.0F);
        Assert.AreEqual(box[1].y, -2.0F);
    }

    [Test]
    public void getCenterTest() {
        List<Vector3> pList = new List<Vector3> {
                new Vector3(-1.0F, 0.0F),
                new Vector3(1.0F, 0.0F),
                new Vector3(0.0F, 2.0F),
                new Vector3(0.0F, -2.0F)
            };

        Vector3 center = Recognizer.getCenter(pList);

        Assert.AreEqual(0f, center.x);
        Assert.AreEqual(0f, center.y);

        pList = new List<Vector3> {
                new Vector3(0f,0f),
                new Vector3(2f, 2f),
            };

        center = Recognizer.getCenter(pList);

        Assert.AreEqual(1f, center.x);
        Assert.AreEqual(1f, center.y);

        pList = new List<Vector3> {
                new Vector3(0f,0f),
                new Vector3(-2f, -2f),
            };

        center = Recognizer.getCenter(pList);

        Assert.AreEqual(-1f, center.x);
        Assert.AreEqual(-1f, center.y);
    }

    [Test]
    public void checkDistanceTest() {
        Recognizer recognizer = new Recognizer();

        List<Vector3> userStroke = new List<Vector3> {
                new Vector3(0f,0f),
                new Vector3(2f, 2f),
            };
        List<Vector3> targetStroke = new List<Vector3> {
                new Vector3(0f,0f),
                new Vector3(2f, 2f),
            };

        Assert.AreEqual(0f, recognizer.checkDistance(userStroke, targetStroke));

        userStroke = new List<Vector3> {
                new Vector3(-.5f,0f),
                new Vector3(1.5f, 2f),
            };

        Assert.AreEqual(.5f, recognizer.checkDistance(userStroke, targetStroke));

        userStroke = new List<Vector3> {
                new Vector3(-2f,0f),
                new Vector3(0f, 2f),
            };

        Assert.AreEqual(-1f, recognizer.checkDistance(userStroke, targetStroke));
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Recognizer {
    private const float ANGLE_THRESHOLD = 30f;
    private const int CORNER_PENALTY = 50;
    private const int CORNER_THRESHOLD = 0;
    private const float DISTANCE_THRESHOLD = 0.85f; // need to adjust!

    public Recognizer() {

    }

    public StrokeScore getResults(List<Vector3> userStroke, List<Vector3> targetStroke, bool hasFlag = false) {
        StrokeScore strokeScore = new StrokeScore();

        strokeScore.Angle = checkAngle(userStroke, targetStroke);
        strokeScore.Corners = checkCorners(userStroke, targetStroke, hasFlag);
        strokeScore.Distance = checkDistance(userStroke, targetStroke);

        if (strokeScore.Angle < 0 || strokeScore.Corners < 0 || strokeScore.Distance < 0) {
            strokeScore.Pass = false;
        } else {
            strokeScore.Pass = true;
        }

        return strokeScore;
    }

    public float checkAngle(List<Vector3> userStroke, List<Vector3> targetStroke,
        float threshold = ANGLE_THRESHOLD) {

        if (userStroke.Count < 2 || targetStroke.Count < 2) {
            return 0f;
        }

        float userAngle = getAngle(userStroke[0], userStroke[1]);
        float targetAngle = getAngle(targetStroke[0], targetStroke[1]);

        float diff = Mathf.Abs(userAngle - targetAngle);
        if (diff <= threshold) {
            return diff;
        }

        return -1f;
    }

    public float getAngle(Vector3 p1, Vector3 p2) {
        float x = p2.x - p1.x;
        float y = p2.y - p1.y;

        return (Mathf.Atan2(y, x) * Mathf.Rad2Deg);
    }

    public int checkCorners(List<Vector3> userStroke, List<Vector3> targetStroke,
        bool hasFlag = false,
        int threshold = CORNER_THRESHOLD) {

        int diff = Mathf.Abs(userStroke.Count - targetStroke.Count);

        // account for possible lack of flag upstroke
        if (hasFlag && (targetStroke.Count > userStroke.Count)) {
            diff--;
        }

        if (diff <= threshold) {
            return diff * CORNER_PENALTY;
        }

        return -1;
    }

    public float checkDistance(List<Vector3> userStroke, List<Vector3> targetStroke,
        bool hasFlag = false,
        float threshold = DISTANCE_THRESHOLD) {

        Vector3 targetCenter = getCenter(targetStroke);
        Vector3 userCenter = getCenter(userStroke);

        float dist = Vector3.Distance(targetCenter, userCenter);
        if (dist <= threshold) {
            return dist;
        }
        return -1;
    }

    public static Vector3 getCenter(List<Vector3> points) {
        Vector3[] rect = getBoundingBox(points);

        float width = rect[1].x - rect[0].x;
        float height = rect[0].y - rect[1].y;

        Vector3 center = new Vector3();
        center.x = rect[0].x + (width / 2f);
        center.y = rect[1].y + (height / 2f);
        center.z = points[0].z;

        return center;
    }


    /// <summary>
    /// Returns the upper left and lower right corner points of a
    /// rectangle that would encompass all points in a list.
    /// </summary>
    /// <param name="pList"></param>
    /// <returns></returns>
    public static Vector3[] getBoundingBox(List<Vector3> pList) {
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        foreach (Vector3 p in pList) {
            if (p.x < minX) minX = p.x;
            if (p.x > maxX) maxX = p.x;
            if (p.y < minY) minY = p.y;
            if (p.y > maxY) maxY = p.y;
        }

        // Assumes that a greater Y is higher
        return new Vector3[]
            { new Vector3(minX, maxY), new Vector3(maxX, minY) };
    }

}

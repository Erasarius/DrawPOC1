using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace DrawPOC1 {

    public static class ShortStraw {
        private const float INTERSPACING_CONSTANT = 40.0F;
        private const int STRAW_WINDOW = 3;
        private const float MEDIAN_THRESHOLD = 0.95F;
        private const float LINE_THRESHOLD = 0.80F;

        public static List<Vector3> getCornerPoints(List<Vector3> pList) {
            List<Vector3> corners = new List<Vector3>();
            if (pList.Count == 0 || pList.Count < 2) {
                return corners;
            } else {
                float rSpacing = getResamplingSpacing(pList);
                List<Vector3> rPoints = resamplePoints(pList, rSpacing);
                List<int> idx = getCornerIdx(rPoints);
                foreach (int i in idx) {
                    corners.Add(rPoints[i]);
                }
            }
            return corners;
        }



        /// <summary>
        /// Determine the top left and bottom right points needed to
        /// create a bounding box surrounding all points in the list.
        /// </summary>
        /// <param name="pList">List of points.</param>
        /// <returns>An array of the top left and bottom right points of 
        /// bounding box.</returns>
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

        public static float getResamplingSpacing(List<Vector3> pList) {
            // Get the top left corner point and the bottom right corner point
            Vector3[] box = getBoundingBox(pList);

            // Get diagonal distance of bounding box
            float diagDist = Vector3.Distance(box[0], box[1]);

            // Compute the interSpacing distance
            return (diagDist / INTERSPACING_CONSTANT);
        }

        private static List<Vector3> resamplePoints(List<Vector3> pList, float rSpacing) {
            float distance = 0.0F;
            List<Vector3> rPoints = new List<Vector3>();

            rPoints.Add(pList[0]);

            for (int i = 1; i < pList.Count; i++) {
                Vector3 p1 = pList[i - 1];
                Vector3 p2 = pList[i];

                float d = Vector3.Distance(p1, p2);
                
                if ((distance + d) >= rSpacing) {
                    Vector3 pNew = new Vector3();
                    pNew.x = p1.x + (((rSpacing - distance) / d) * (p2.x - p1.x));
                    pNew.y = p1.y + (((rSpacing - distance) / d) * (p2.y - p1.y));
                    rPoints.Add(pNew);
                    pList.Insert(i, pNew);
                    distance = 0.0F;
                } else {
                    distance += d;
                }
                
            }

            return rPoints;
        }

        private static List<int> getCornerIdx(List<Vector3> rPoints) {
            List<int> corners = new List<int>();
            
            // first point is a corner
            corners.Add(0);

            List<float> straws = new List<float>();

            // store "straw" distances between spaced apart points
            for (int i = STRAW_WINDOW; i < (rPoints.Count - STRAW_WINDOW); i++) {
                Vector3 p1 = rPoints[i - STRAW_WINDOW];
                Vector3 p2 = rPoints[i + STRAW_WINDOW];
                straws.Add(Vector3.Distance(p1, p2));
            }

            // determine a threshold value in order to eliminate longer straws
            float t = QuickMedian.Median(straws) * MEDIAN_THRESHOLD;

            // collect the point indeces corresponding to the shortest straws
            // in each region
            for (int i = STRAW_WINDOW; i < (rPoints.Count - STRAW_WINDOW); i++) {
                // check if straw corresponding to current point is below 
                // median threshold
                if (straws[pointToStrawIdx(i)] < t) {
                    float min = float.MaxValue;
                    int minIdx = i;

                    // continue searching for index of point corresponding to the
                    // shortest straw in this region
                    while ((pointToStrawIdx(i)) < straws.Count 
                        && straws[pointToStrawIdx(i)] < t) {
                        if (straws[pointToStrawIdx(i)] < min) {
                            min = straws[pointToStrawIdx(i)];
                            minIdx = i;
                        }
                        i++;
                    }
                    // save shortest straw in region
                    corners.Add(minIdx);
                }
            }
            // add final point as a corner
            corners.Add(rPoints.Count - 1);
            corners = postProcessCorners(rPoints, corners, straws);

            return corners;
        }

        /// <summary>
        /// Post-process corner list with higher-level polyline rules.
        /// </summary>
        /// <param name="rPoints"></param>
        /// <param name="corners"></param>
        /// <param name="straws"></param>
        /// <returns>A set of corners post-processed with higher-level polyline rules</returns>
        private static List<int> postProcessCorners(List<Vector3> rPoints, List<int> corners, List<float> straws) {
            bool cont = false;

            while (!cont) {
                cont = true;
                for (int i = 1; i < corners.Count; i++) {
                    int c1 = corners[i - 1];
                    int c2 = corners[i];

                    if (!isLine(rPoints, c1, c2)) {
                        int newCorner = getHalfwayCorner(straws, c1, c2);

                        // avoid adding undefined points
                        if (newCorner > c1 && newCorner < c2) {
                            corners.Insert(i, newCorner);
                            cont = false;
                        }
                    }
                }
            }

            for (int i = 1; i < (corners.Count - 1); i++) {
                int c1 = corners[i - 1];
                int c2 = corners[i + 1];

                if (isLine(rPoints, c1, c2)) {
                    corners.RemoveAt(i);
                    i--;
                }
            }

            return corners;
        }


        /// <summary>
        /// Calculate the path distance between two points.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="a">Index of point a</param>
        /// <param name="b">Index of point b</param>
        /// <returns>The path (stroke segment) distance between the points 
        /// at a and b</returns>
        private static float getPathDistance(List<Vector3> points, int a, int b) {
            float d = 0.0F;

            for (int i = a; i < b; i++) {
                d += Vector3.Distance(points[i], points[i + 1]);
            }

            return d;
        }

        /// <summary>
        /// Determine a possible corner between points at a and b.
        /// </summary>
        /// <param name="straws">List of straw distances.</param>
        /// <param name="a">Point (not straw) index for a.</param>
        /// <param name="b">Point (not straw) index for b.</param>
        /// <returns>A possible corner between the points a and b.</returns>
        private static int getHalfwayCorner(List<float> straws, int a, int b) {
            int minIdx = 0;
            int quarter = (b - a) / 4;
            float minVal = float.MaxValue;

            for (int i = (a + quarter); i < (b - quarter); i++) {
                // TODO this is kind of hacky...
                if (pointToStrawIdx(i) < 0 || pointToStrawIdx(i) >= straws.Count)
                    continue;

                if (straws[pointToStrawIdx(i)] < minVal) {
                    minVal = straws[pointToStrawIdx(i)];
                    minIdx = i;
                }
            }

            return minIdx;
        }

        /// <summary>
        /// Determine if the path between two points forms a straight line.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="a">Index at a.</param>
        /// <param name="b">Index at b.</param>
        /// <returns>True if the stroke segment between a and b is a line.</returns>
        private static bool isLine(List<Vector3> points, int a, int b) {
            float dist = Vector3.Distance(points[a], points[b]);
            float pathDist = getPathDistance(points, a, b);

            return ((dist / pathDist) > LINE_THRESHOLD);
        }

        private static int pointToStrawIdx(int i) {
            return (i - STRAW_WINDOW);
        }
    }
};
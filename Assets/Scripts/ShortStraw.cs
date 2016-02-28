using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace DrawPOC1 {

    public class ShortStraw : MonoBehaviour {
        private const float INTERSPACING_CONSTANT = 40.0F;
        private const int STRAW_WINDOW = 3;
        private const float MEDIAN_THRESHOLD = 0.95F;
        private float LINE_THRESHOLD = 0.80F;

        public ShortStraw() {
        }

        public List<Vector3> getCornerPoints(List<Vector3> pList) {
            List<Vector3> corners = new List<Vector3>();
            if (pList.Count == 0 || pList.Count < 2) {
                return corners;
            } else {
                float rSpacing = getResamplingSpacing(pList);
                List<Vector3> rPoints = resamplePoints(pList, rSpacing);
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
        public Vector3[] getBoundingBox(List<Vector3> pList) {
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

        public float getResamplingSpacing(List<Vector3> pList) {
            // Get the top left corner point and the bottom right corner point
            Vector3[] box = getBoundingBox(pList);

            // Get diagonal distance of bounding box
            float diagDist = Vector3.Distance(box[0], box[1]);

            // Compute the interSpacing distance
            return (diagDist / INTERSPACING_CONSTANT);
        }

        private List<Vector3> resamplePoints(List<Vector3> pList, float rSpacing) {
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

        private List<int> getCornerIdx(List<Vector3> rPoints) {
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
                if (straws[i - STRAW_WINDOW] < t) {
                    float min = float.MaxValue;
                    int minIdx = i;

                    // continue searching for index of point corresponding to the
                    // shortest straw in this region
                    while ((i - STRAW_WINDOW) < straws.Count && straws[i - STRAW_WINDOW] < t) {
                        if (straws[i - STRAW_WINDOW] < min) {
                            min = straws[i - STRAW_WINDOW];
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

        private List<int> postProcessCorners(List<Vector3> rPoints, List<int> corners, List<float> straws) {
            throw new NotImplementedException();
        }
    }
};
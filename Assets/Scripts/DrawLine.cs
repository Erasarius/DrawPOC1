/* Original source from:
http://www.theappguruz.com/blog/draw-line-mouse-move-detect-line-collision-unity2d-unity3d
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DrawPOC1 {

    public class DrawLine : MonoBehaviour {
        private LineRenderer line;
        private bool isMousePressed;
        private List<Vector3> pointsList;
        private Vector3 mousePos;
        //List<Vector3> corners;
        List<GameObject> marks;

        // Structure for line points
        struct myLine {
            public Vector3 StartPoint;
            public Vector3 EndPoint;
        };
        //	-----------------------------------	
        void Awake() {
            // Create line renderer component and set its property
            float width = 0.05f;
            line = gameObject.AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Particles/Additive"));
            line.SetVertexCount(0);
            line.SetWidth(width, width);
            line.SetColors(Color.green, Color.green);
            line.useWorldSpace = true;
            isMousePressed = false;
            pointsList = new List<Vector3>();
            marks = new List<GameObject>();
            //		renderer.material.SetTextureOffset(
        }
        //	-----------------------------------	
        void Update() {
            // If mouse button down, remove old line and set its color to green
            if (Input.GetMouseButtonDown(0)) {
                isMousePressed = true;
                line.SetVertexCount(0);
                pointsList.RemoveRange(0, pointsList.Count);
                line.SetColors(Color.green, Color.green);
                clearMarks();
            } else if (Input.GetMouseButtonUp(0)) {
                isMousePressed = false;

                List<Vector3> corners = ShortStraw.getCornerPoints(pointsList);

                markCorners(corners);
            }
            // Drawing line when mouse is moving(presses)
            if (isMousePressed) {
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                if (!pointsList.Contains(mousePos)) {
                    pointsList.Add(mousePos);
                    line.SetVertexCount(pointsList.Count);
                    line.SetPosition(pointsList.Count - 1, (Vector3)pointsList[pointsList.Count - 1]);
                }
            }
        }

        private void clearMarks() {
            foreach (GameObject mark in marks) {
                Destroy(mark);
            }
        }

        private void markCorners(List<Vector3> corners) {
            float scale = 0.1F;
            for (int i = 0; i < corners.Count; i++) {
                Vector3 corner = corners[i]; 
                GameObject mark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                mark.transform.position = corner;
                mark.transform.localScale = new Vector3(scale, scale, scale);
                mark.GetComponent<Renderer>().material.SetColor("_SpecColor", Color.red);
                this.marks.Add(mark);
                Debug.Log(string.Format("{0},{1},{2}", i, corner.x, corner.y));
            }
        }
    }
};
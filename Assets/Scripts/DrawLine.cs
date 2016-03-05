/* Original source from:
http://www.theappguruz.com/blog/draw-line-mouse-move-detect-line-collision-unity2d-unity3d
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KanjiDraw {

    public class DrawLine : MonoBehaviour {
        [SerializeField] public KanjiBuilder kanjiBuilder;
        private LineRenderer line;
        private bool isMousePressed;
        private List<Vector3> pointsList;
        private Vector3 mousePos;
        private bool mouseOver = false;
        //private Event
        //List<Vector3> corners;

        private Color lineColor = Color.red;

        //	-----------------------------------	
        void Awake() {
            // Create line renderer component and set its property
            //float width = 0.05f;
            line = gameObject.GetComponent<LineRenderer>();
            //line.material = new Material(Shader.Find("Particles/Additive"));
            //line.material = new Material(Shader.Find("Sprites/NewSurfaceShader"));
            line.SetVertexCount(0);
            //line.SetWidth(width, width);
            //line.SetColors(lineColor, lineColor);
            line.useWorldSpace = true;
            //line.sortingLayerName = "Default";
            //line.sortingOrder = 0;
            
            isMousePressed = false;
            pointsList = new List<Vector3>();
            
            //		renderer.material.SetTextureOffset(
        }
        //	-----------------------------------	
        void Update() {

            /*
            // If mouse button down, remove old line and set its color to green
            if (Input.GetMouseButtonDown(0)) {
                isMousePressed = true;
                line.SetVertexCount(0);
                pointsList.RemoveRange(0, pointsList.Count);
                //line.SetColors(lineColor, lineColor);
                if (kanjiBuilder != null) {
                    kanjiBuilder.onStrokeBegin();
                }
            } else if (Input.GetMouseButtonUp(0)) {
                isMousePressed = false;

                List<Vector3> corners = ShortStraw.getCornerPoints(pointsList);

                if (kanjiBuilder != null) {
                    kanjiBuilder.onStrokeComplete(corners);
                }
            }
            // Drawing line when mouse is moving(presses)
            if (isMousePressed) {
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = -10;
                if (!pointsList.Contains(mousePos)) {
                    pointsList.Add(mousePos);
                    line.SetVertexCount(pointsList.Count);
                    line.SetPosition(pointsList.Count - 1, (Vector3)pointsList[pointsList.Count - 1]);
                }
            }
            */

        }

        public void onPointerEnter() {
            Debug.Log("Pointer enter");
        }

        public void onPointerExit() {
            Debug.Log("Pointer exit");
        }

        public void onDrag() {
            Debug.Log("Drag");
        }
    }
};
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace KanjiDraw {

    public class KanjiBuilder : MonoBehaviour {
        private List<Vector3> corners;
        private List<GameObject> marks;
        [SerializeField] public Sprite markSprite;

        public void onStrokeBegin() {
            clearMarks();
        }

        public void onStrokeComplete(List<Vector3> points) {

            this.corners = ShortStraw.getCornerPoints(points);
            markCorners(corners);
        }

        // Use this for initialization
        void Start() {
            marks = new List<GameObject>();
        }

        // Update is called once per frame
        void Update() {

        }

        private void clearMarks() {
            foreach (GameObject mark in marks) {
                Destroy(mark);
            }
        }

        private void markCorners(List<Vector3> corners) {
            /*
            float scale = 0.1F;
            for (int i = 0; i < corners.Count; i++) {
                Vector3 corner = corners[i];
                GameObject mark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                mark.transform.position = corner;
                mark.transform.localScale = new Vector3(scale, scale, scale);
                Renderer renderer = mark.GetComponent<Renderer>();
                renderer.material.SetColor("_SpecColor", Color.red);
                this.marks.Add(mark);
                Debug.Log(string.Format("{0},{1},{2}", i, corner.x, corner.y));
                
            }
            */
            for (int i = 0; i < corners.Count; i++) {
                GameObject newMark = new GameObject();
                SpriteRenderer renderer = newMark.AddComponent<SpriteRenderer>();
                renderer.sprite = markSprite;
                newMark.transform.position = corners[i];

                marks.Add(newMark);
            }
        }

    }
};
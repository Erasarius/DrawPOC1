using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using GameDataEditor;

namespace KanjiDraw {

    public class KanjiTest : MonoBehaviour {
        private List<Vector3> corners;
        //private List<List<Vector3>> charCorners;
        private List<Vector3>[] charCorners;
        private List<GameObject> marks;
        [SerializeField]
        public Sprite markSprite;
        private SpriteRepository spriteRepo;
        private GDECharacterData charData;
        private string charKey = GDEItemKeys.Character_Three;
        private int curStroke = 0;
        //private GameObject drawLine;
        [SerializeField]
        public GameObject drawLineObj;
        [SerializeField] public bool showPromptTotal = true;
        [SerializeField] public float promptAlpha = 0.5f;
        [SerializeField] public bool showMarks;
        private Vector3 center;

        // Use this for initialization
        void Start() {
            marks = new List<GameObject>();
            GDEDataManager.Init("gde_data");
            charData = new GDECharacterData(charKey);

            spriteRepo = new SpriteRepository("Sprites/Kanji");

            //setSprite(charData.spriteNames[curStroke]);

            charCorners = new List<Vector3>[charData.numStrokes];
            if (charData.strokes.Count != 0) {
                for (int i = 0; i < charData.strokes.Count; i++) {
                    charCorners[i] = charData.strokes[i];
                }
            }

            center = new Vector3(0f, 0f, 0f);

            if (showPromptTotal) {
                doPromptTotal();
            }
        }

        // Update is called once per frame
        void Update() {

        }

        void Awake() {
            Messenger.AddListener(GameEvent.STROKE_COMPLETE, onStrokeComplete);
            Messenger.AddListener(GameEvent.STROKE_BEGIN, onStrokeBegin);
        }

        void OnDestroy() {
            Messenger.RemoveListener(GameEvent.STROKE_COMPLETE, onStrokeComplete);
            Messenger.RemoveListener(GameEvent.STROKE_BEGIN, onStrokeBegin);
        }

        public void onStrokeBegin() {
            clearMarks();
        }

        public void onStrokeComplete() {
            DrawLine script = drawLineObj.GetComponent<DrawLine>();
            List<Vector3> points = script.getPointsList();
            this.corners = ShortStraw.getCornerPoints(points);

            if (showMarks) {
                markCorners(corners);
            }

            charCorners[curStroke] = corners; // FIXME
            Debug.Log("Stroke Complete!");
            foreach (Vector3 corner in corners) {
                Debug.Log(string.Format("    {0}, {1}, {2}", corner.x, corner.y, corner.z));
            }

            // show sprite stroke for the just completed stroke
            if (curStroke < charData.numStrokes) {
                setSprite(charData.spriteNames[curStroke]);
                script.clearLine();
            }
            curStroke++;
        }


        private void clearMarks() {
            if (marks == null) {
                return;
            }

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

                newMark.transform.position = new Vector3(corners[i].x, corners[i].y, SceneDepth.LINE_DEPTH);

                marks.Add(newMark);
            }
        }

        private void setSprite(string name) {
            Sprite sprite = spriteRepo.getSprite(name);
            GetComponent<SpriteRenderer>().sprite = sprite;
        }

        /// <summary>
        /// Inserts a new sprite into the background of the completed
        /// character to assist player.
        /// </summary>
        private void doPromptTotal() {
            GameObject promptTotalObj = new GameObject("Prompt Total");
            string spriteName = charData.spriteNames[charData.spriteNames.Count - 1];
            SpriteRenderer renderer = promptTotalObj.AddComponent<SpriteRenderer>();
            renderer.sprite = spriteRepo.getSprite(spriteName);
            renderer.color = new Color(1f, 1f, 1f, promptAlpha);

            promptTotalObj.transform.position = center;
        }
    }
};
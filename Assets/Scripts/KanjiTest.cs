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
        [SerializeField] public float strokeMarkDelay = 0.3f;
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

            StartCoroutine(displayStrokeHint(curStroke));
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
            script.clearLine();

            if (showMarks) {
                markCorners(corners);
            }

            charCorners[curStroke] = corners; // FIXME
            Debug.Log("Stroke Complete!");
            //foreach (Vector3 corner in corners) {
            //    Debug.Log(string.Format("    {0}, {1}, {2}", corner.x, corner.y, corner.z));
            //}

            Recognizer recognizer = new Recognizer();
            StrokeScore score = recognizer.getResults(corners, charData.strokes[curStroke]);

            Debug.Log(string.Format("    *** {0} *** angle: {1}, corners: {2}, dist: {3}", 
                score.Pass ? "Pass" : "Fail", score.Angle, score.Corners, score.Distance));

            // show sprite stroke for the just completed stroke
            if (score.Pass) {
                setStrokeSprite(charData.spriteNames[curStroke]);
                curStroke++;
            }

            if (curStroke >= charData.numStrokes) {
                // the character is complete, back out of scene
                Debug.Log("Character complete!");
                curStroke--;
            } else {
                StartCoroutine(displayStrokeHint(curStroke));
            }

        }

        private IEnumerator displayStrokeHint(int strokeNum) {
            List<Vector3> targetCorners = charData.strokes[strokeNum];

            yield return new WaitForSeconds(strokeMarkDelay);

            // display the completed next stroke
            GameObject strokeHint = new GameObject("Stroke Hint");
            SpriteRenderer strokeHintRenderer = strokeHint.AddComponent<SpriteRenderer>();
            strokeHintRenderer.transform.position = new Vector3(0f, 0f, -5f);

            string spriteName = charData.spriteNames[strokeNum];
            strokeHintRenderer.sprite = spriteRepo.getSprite(spriteName);
            strokeHintRenderer.color = Color.black;

            // wait a moment
            yield return new WaitForSeconds(strokeMarkDelay);

            GameObject mark = new GameObject();
            SpriteRenderer renderer = mark.AddComponent<SpriteRenderer>();
            renderer.sprite = markSprite;

            // display the corner points in order, pausing along the way
            // to indicate stroke direction
            for (int i = 0; i < targetCorners.Count; i++) {
                mark.transform.position = 
                    new Vector3(targetCorners[i].x, targetCorners[i].y, SceneDepth.LINE_DEPTH);

                yield return new WaitForSeconds(strokeMarkDelay);
            }

            // destory the mark, wait a moment, then destroy the stroke hint
            Destroy(mark);
            yield return new WaitForSeconds(strokeMarkDelay);
            Destroy(strokeHint);
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

        private void setStrokeSprite(string name) {
            Sprite sprite;
            if (name == null) {
                sprite = null;
            } else {
                sprite = spriteRepo.getSprite(name);
            }
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = Color.black;
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
            //renderer.color = Color.black;
            renderer.color = new Color(0f, 0f, 0f, promptAlpha);

            promptTotalObj.transform.position = center;
        }
    }
};
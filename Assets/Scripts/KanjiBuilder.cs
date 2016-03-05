using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using GameDataEditor;

namespace KanjiDraw {

    public class KanjiBuilder : MonoBehaviour {
        private List<Vector3> corners;
        [SerializeField] private List<List<Vector3>> charCorners;
        private List<GameObject> marks;
        [SerializeField] public Sprite markSprite;
        private SpriteRepository spriteRepo;
        private GDECharacterData charData;
        private string charKey = GDEItemKeys.Character_Three;
        private int curStroke = 0;

        // Use this for initialization
        void Start() {
            marks = new List<GameObject>();
            GDEDataManager.Init("gde_data");
            charData = new GDECharacterData(charKey);

            spriteRepo = new SpriteRepository("Sprites/Kanji");

            setSprite(charData.spriteNames[curStroke]);

            charCorners = new List<List<Vector3>>();
        }

        // Update is called once per frame
        void Update() {

        }

        void Awake() {
            Messenger.AddListener(GameEvent.NEXT_STROKE, onNextStroke);
            //Messenger.AddListener(GameEvent.ADD_CORNERS, onAddCorners);
            Messenger.AddListener(GameEvent.RESET_CHARACTER, onResetCharacter);
            Messenger.AddListener(GameEvent.SAVE_CHARACTER, onSaveCharacter);
            Messenger<bool>.AddListener(GameEvent.VIEW_CORNERS, onViewCorners);
        }

        public void onStrokeBegin() {
            clearMarks();
        }

        public void onStrokeComplete(List<Vector3> points) {

            this.corners = ShortStraw.getCornerPoints(points);
            markCorners(corners);
            //charCorners[curStroke] = corners; // FIXME
            Debug.Log("Stroke Complete!");
        }

        private void onResetCharacter() {
            charCorners.Clear();
            curStroke = 0;
            setSprite(charData.spriteNames[curStroke]);
        }

        void OnDestroy() {
            Messenger.RemoveListener(GameEvent.NEXT_STROKE, onNextStroke);
            //Messenger.RemoveListener(GameEvent.ADD_CORNERS, onAddCorners);
            Messenger.RemoveListener(GameEvent.RESET_CHARACTER, onResetCharacter);
            Messenger.RemoveListener(GameEvent.SAVE_CHARACTER, onSaveCharacter);
            Messenger<bool>.RemoveListener(GameEvent.VIEW_CORNERS, onViewCorners);
        }

        private void onSaveCharacter() {
            charData.strokes = charCorners;
            charData.Set_strokes();
            GDEDataManager.Save();
            GDEDataManager.SaveToDisk();
        }

        //private void onAddCorners() {
        //    charCorners[curStroke] = corners;
        //}

        private void onNextStroke() {
            curStroke = (curStroke + 1) % charData.numStrokes;
            setSprite(charData.spriteNames[curStroke]);
        }

        private void onViewCorners(bool viewCorners) {
            clearMarks();

            if (viewCorners) {
                foreach (List<Vector3> strokeCorners in charCorners) {
                    markCorners(strokeCorners);
                }
            }
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

        private void setSprite(string name) {
            Sprite sprite = spriteRepo.getSprite(name);
            GetComponent<SpriteRenderer>().sprite = sprite;
        }

    }
};
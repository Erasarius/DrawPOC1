using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KanjiDraw {
    public class SceneController : MonoBehaviour {
        //[SerializeField] private Sprite[] images;
        //[SerializeField] private GameObject background;
        private SpriteRepository spriteRepo;
        private int curStroke;
        private const int NUM_STROKES = 3;

        // Use this for initialization
        void Start() {
            //Vector3 position = background.transform.position;

            //for (int i = 0; i < images.Length; i++) {
            //    images[i].
            //}
            spriteRepo = new SpriteRepository("Sprites/Kanji");
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetMouseButtonDown(0)) {
                Sprite sprite = getSprite(curStroke);
                GetComponent<SpriteRenderer>().sprite = sprite;
                Debug.Log(string.Format("Sprite name: {0}", sprite.name));
                curStroke = (curStroke + 1) % NUM_STROKES;
            }
        }

        private Sprite getSprite(int stroke) {
            string name = string.Format("three_{0:00}", stroke);

            return spriteRepo.getSprite(name);
        }
    }
}
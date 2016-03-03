using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KanjiDraw {
    public class SpriteRepository {
        private Dictionary<string, Sprite> spriteDict;

        public SpriteRepository() {
            spriteDict = new Dictionary<string, Sprite>();
        }

        public SpriteRepository(string path) {
            buildRepository(path);
        }

        public void buildRepository(string path) {
            spriteDict = new Dictionary<string, Sprite>();
            Sprite[] sprites = Resources.LoadAll<Sprite>(path);

            foreach (Sprite sprite in sprites) {
                spriteDict.Add(sprite.name, sprite);
            }
        }

        public Sprite getSprite(string name) {
            Sprite sprite;

            if (spriteDict.TryGetValue(name, out sprite)) {
                return sprite;
            }

            throw new System.ArgumentException(string.Format("No sprite matching \"{0}\" exists in repository", name));
        }
    }
};
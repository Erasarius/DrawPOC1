using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneController : MonoBehaviour {
    [SerializeField] private Sprite[] images;
    //[SerializeField] private GameObject background;
    private int curStroke;

	// Use this for initialization
	void Start () {
        //Vector3 position = background.transform.position;

        //for (int i = 0; i < images.Length; i++) {
        //    images[i].
        //}

	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            GetComponent<SpriteRenderer>().sprite = images[curStroke];
            curStroke = (curStroke + 1) % images.Length;
        }
    }
}

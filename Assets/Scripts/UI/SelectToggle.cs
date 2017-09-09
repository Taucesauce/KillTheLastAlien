using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectToggle : MonoBehaviour {

    Image currentImage;
    [SerializeField]
    Player player;

    [SerializeField]
    Sprite[] toggleSprites;

    void Start() {
        currentImage = GetComponent<Image>();
    }
    // Update is called once per frame
    void Update () {
        if (player.isPlaying) {
            currentImage.sprite = toggleSprites[1];
        } else {
            currentImage.sprite = toggleSprites[0];
        }
	}
}

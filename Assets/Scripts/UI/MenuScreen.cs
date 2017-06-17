using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour {
    [SerializeField]
    Sprite[] menuScreens = new Sprite[5];

    Image currentImage;
    int screenIndex = 0;

	// Use this for initialization
	void Start () {
        currentImage = GetComponent<Image>();
	}

    void PrevScreen() {
        if (screenIndex < menuScreens.Length) {
            screenIndex--;
            currentImage.sprite = menuScreens[screenIndex];
        }
    }

    void NextScreen() {
        if(screenIndex < menuScreens.Length) {
            screenIndex++;
            currentImage.sprite = menuScreens[screenIndex];
        }
    }
}

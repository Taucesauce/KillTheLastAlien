using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleLock : MonoBehaviour {
    Image currentImage;

    [SerializeField]
    int player;

    [SerializeField]
    Sprite[] sprites = new Sprite[2];

    void Start() {
        currentImage = GetComponent<Image>();
    }
    void OnEnable() {
        EventManager.StartListeningTypeInt("PlayerLocked", UILock);
        EventManager.StartListeningTypeInt("PlayerUnlocked", UIUnlock);
    }

    void OnDisable() {
        EventManager.StopListeningTypeInt("PlayerLocked", UILock);
        EventManager.StopListeningTypeInt("PlayerUnlocked", UIUnlock);
    }

    void UILock(int player) {
        if(this.player == player)
            currentImage.sprite = sprites[1];
    }

    void UIUnlock(int player) {
        if (this.player == player)
            currentImage.sprite = sprites[0];
    }
    
}

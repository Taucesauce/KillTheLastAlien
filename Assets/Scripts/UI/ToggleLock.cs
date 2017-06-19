using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleLock : MonoBehaviour {
    Image currentImage;

    [SerializeField]
    int player;

    [SerializeField]
    Sprite[] sprites = new Sprite[4];

    void Start() {
        currentImage = GetComponent<Image>();
    }
    void OnEnable() {
        EventManager.StartListeningTypeInt("PlayerLocked", UILock);
        EventManager.StartListeningTypeInt("PlayerUnlocked", UIUnlock);
        EventManager.StartListeningTypeInt("UISuccess", UISuccess);
        EventManager.StartListeningTypeInt("UIFail", UIFail);
        EventManager.StartListening("RoundReset", UIUnlockAll);
    }

    void OnDisable() {
        EventManager.StopListeningTypeInt("PlayerLocked", UILock);
        EventManager.StopListeningTypeInt("PlayerUnlocked", UIUnlock);
        EventManager.StopListeningTypeInt("UISuccess", UISuccess);
        EventManager.StopListeningTypeInt("UIFail", UIFail);
    }

    void UIUnlock(int player) {
        if (this.player == player)
            currentImage.sprite = sprites[0];
    }

    void UILock(int player) {
        if(this.player == player)
            currentImage.sprite = sprites[1];
    }

    void UISuccess(int player) {
        if (this.player == player)
            currentImage.sprite = sprites[2];
    }

    void UIFail(int player) {
        if (this.player == player)
            currentImage.sprite = sprites[3];
    }

    void UIUnlockAll() {
        currentImage.sprite = sprites[0];
    }
}

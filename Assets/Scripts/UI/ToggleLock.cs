using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Originally a class to toggle the player's UI from empty to X on selection.
//As the game progressed, we decided there would be variety of UI states, so this class grew beyond just locking the UI.
public class ToggleLock : MonoBehaviour {
    //Current UI sprite
    Image currentImage;

    //Which player's UI this belongs to, assigned in inspector
    [SerializeField]
    int player;

    //Set of UI sprites specific to the player, assigned in inspector
    [SerializeField]
    Sprite[] sprites = new Sprite[4];

    //Get whatever sprite is being used.
    void Start() {
        currentImage = GetComponent<Image>();
    }

    //Setup UI changing events corresponding to actions taken in game.
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

    //Methods for changing sprites based on player actions.
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

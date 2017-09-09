using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartToggle : MonoBehaviour {
    Image currentImage;

    [SerializeField]
    Sprite[] toggleSprites;
	// Use this for initialization
	void Start () {
        currentImage = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
		if(PlayerManager.Instance.GetActivePlayerCount() >= 2) {
            currentImage.sprite = toggleSprites[1];
        } else {
            currentImage.sprite = toggleSprites[0];
        }
	}
}

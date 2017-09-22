using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundResize : MonoBehaviour {

	// Use this for initialization
	void Start () {
        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        if (rend == null) { return; }

        transform.localScale = Vector3.one;

        float spriteWidth = rend.sprite.bounds.size.x;
        float spriteHeight = rend.sprite.bounds.size.y;

        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        transform.localScale = new Vector2(worldScreenWidth / spriteWidth, worldScreenHeight / spriteHeight);
    }
}

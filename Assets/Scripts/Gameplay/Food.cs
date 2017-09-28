using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Food : MonoBehaviour {
    //Animation offset for sprites to not sync on start
    [SerializeField]
    private GameObject GameCanvas;

    //Gameplay Vars
    private Vector2 originalPos;
    public FoodColor color;
    private string colorName;
    public bool isSelected = false;
    bool isAttached = false;
    private int? playerID = null;

    void Start() {
        originalPos = GetComponent<Transform>().position;
        colorName = Enum.GetName(typeof(FoodColor), color);
    }

    void Update() {
        if (isAttached) {
            transform.position = transform.parent.position;
        }
    }

	void OnEnable() {
        EventManager.StartListeningTypeInt("ReachedFood", AssignPlayer);
        EventManager.StartListeningTypeInt("GrabMochi", AttachToPlayer);
        EventManager.StartListening("RoundReset", RoundReset);
        EventManager.StartListening("EndGameUI", EndGame);
    }

    void OnDisable() {
        EventManager.StopListeningTypeInt("ReachedFood", AssignPlayer);
        EventManager.StopListeningTypeInt("GrabMochi", AttachToPlayer);
        EventManager.StopListening("RoundReset", RoundReset);
    }

    void AssignPlayer(int id) {
        if (!isSelected) {
            playerID = id;
            isSelected = true;
            EventManager.TriggerIntEvent("Score", id);
        }
    }

    void AttachToPlayer(int id) {
        if(id == playerID) {
            isAttached = true;
            int adjustedID = (int)playerID + 1;
            transform.parent = GameObject.Find("Player" + adjustedID).transform;
            EventManager.TriggerEvent(playerID + "GrabbedMochi");
            EventManager.TriggerIntEvent("MochiGrabbed", (int)color);
        }
    }

    void RoundReset() {
        isSelected = false;
        isAttached = false;
        transform.parent = null;
        transform.position = originalPos;
        playerID = null;
    }

    void EndGame() {
        transform.parent = GameCanvas.transform;
    }
}

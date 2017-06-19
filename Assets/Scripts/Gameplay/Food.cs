using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Food : MonoBehaviour {
    //Animation offset for sprites to not sync on start
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float animationOffset;

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
        GetComponent<Animator>().Play(Enum.GetName(typeof(FoodColor), color) + "Idle", -1, animationOffset);
        colorName = Enum.GetName(typeof(FoodColor), color);
    }

    void Update() {
        if (isAttached) {
            transform.position = transform.parent.position;
        }
    }
	void OnEnable() {
        EventManager.StartListeningTypeInt(Enum.GetName(typeof(FoodColor),color), AssignPlayer);
        EventManager.StartListeningTypeInt("GrabMochi", AttachToPlayer);
        EventManager.StartListening("RoundReset", RoundReset);
        EventManager.StartListening("EndGameUI", EndGame);
    }

    void OnDisable() {
        EventManager.StopListeningTypeInt(Enum.GetName(typeof(FoodColor), color), AssignPlayer);
        EventManager.StopListeningTypeInt("GrabMochi", AttachToPlayer);
        EventManager.StopListening("RoundReset", RoundReset);
    }

    void AssignPlayer(int id) {
        if (!isSelected) {
            playerID = id;
            isSelected = true;
            if (GameManager.Instance.state == GameState.Decision) {
                EventManager.TriggerIntEvent("DecisionScore", id);
            } else if (GameManager.Instance.state == GameState.Scramble) {
                EventManager.TriggerIntEvent("ScrambleScore", id);
            }
        }
    }

    void AttachToPlayer(int id) {
        if(id == playerID) {
            isAttached = true;
            int adjustedID = (int)playerID + 1;
            transform.parent = GameObject.Find("Player" + adjustedID).transform;
            GetComponent<Animator>().Play(colorName + "Wow");
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
        GetComponent<Animator>().Play(Enum.GetName(typeof(FoodColor), color) + "Idle", -1, animationOffset);
    }

    void EndGame() {
        transform.parent = GameCanvas.transform;
    }
}

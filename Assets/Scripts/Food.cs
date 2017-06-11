using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Food : MonoBehaviour {

    public FoodColor color;
    public bool isSelected = false;
    bool isAttached = false;

    [SerializeField]
    private int? playerID = null;

    void Update() {
        if (isAttached) {
            transform.position = transform.parent.position;
        }
    }
	void OnEnable() {
        EventManager.StartListeningTypeInt(Enum.GetName(typeof(FoodColor),color), AssignPlayer);
        EventManager.StartListeningTypeInt("GrabMochi", AttachToPlayer);
    }

    void OnDisable() {
        EventManager.StopListeningTypeInt(Enum.GetName(typeof(FoodColor), color), AssignPlayer);
        EventManager.StopListeningTypeInt("GrabMochi", AttachToPlayer);
    }

    void AssignPlayer(int id) {
        if (!isSelected) {
            Debug.Log("Food Location: " + color);
            Debug.Log("Player ID: " + id);
            playerID = id;
            isSelected = true;
        }
    }

    void AttachToPlayer(int id) {
        if(id == playerID) {
            isAttached = true;
            int adjustedID = (int)playerID + 1;
            transform.parent = GameObject.Find("Player" + adjustedID).transform;
        }
    }
}

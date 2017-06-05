using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Food : MonoBehaviour {

    public FoodColor color;
    public bool isSelected = false;

    [SerializeField]
    private int? playerID = null;

	void OnEnable() {
        EventManager.StartListeningTypeInt(Enum.GetName(typeof(FoodColor),color), AssignPlayer);
    }

    void OnDisable() {
        EventManager.StopListeningTypeInt(Enum.GetName(typeof(FoodColor), color), AssignPlayer);
    }

    void AssignPlayer(int id) {
        if (!isSelected) {
            Debug.Log("Food Location: " + color);
            Debug.Log("Player ID: " + id);
            playerID = id;
            isSelected = true;
        }
    }
}

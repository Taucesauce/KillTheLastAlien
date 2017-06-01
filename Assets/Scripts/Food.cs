using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FoodLocation {
    Left,
    Middle,
    Right
}

public class Food : MonoBehaviour {

    public FoodLocation location;
    public bool isSelected = false;

    [SerializeField]
    private int? playerID = null;

	void OnEnable() {
        EventManager.StartListeningTypeInt(Enum.GetName(typeof(FoodLocation),location), AssignPlayer);
    }

    void OnDisable() {
        EventManager.StopListeningTypeInt(Enum.GetName(typeof(FoodLocation), location), AssignPlayer);
    }

    void AssignPlayer(int id) {
        //Eventually allow delegate to pass int so food can be assigned on event.
        Debug.Log("Food Location: " + location);
        Debug.Log("Player ID: " + id);
        playerID = id;
    }
}

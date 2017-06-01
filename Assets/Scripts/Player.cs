using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Rewired;
using System;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour {

    public int playerId; //The Rewired player id of this character 
    private Rewired.Player player;
    private CharacterController cc;

    //Gameplay Variables
    bool leftPressed = false;
    bool midPressed = false;
    bool rightPressed = false;
    FoodLocation selection;

    void Awake() {
        player = ReInput.players.GetPlayer(playerId); //use to assign map and access button states

        cc = GetComponent<CharacterController>(); //will need to control anim state and movement of hands
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GetInput();
        ProcessInput();
	}

    private void GetInput() {
        leftPressed = player.GetButtonDown("LeftSelect");
        midPressed = player.GetButtonDown("MiddleSelect");
        rightPressed = player.GetButtonDown("RightSelect");
    }

    private void ProcessInput() {
        if (leftPressed) {
            EventManager.TriggerIntEvent("Left", playerId);
            selection = FoodLocation.Left;
        }
        if (midPressed) {
            EventManager.TriggerIntEvent("Middle", playerId);
            selection = FoodLocation.Middle;
        }
        if (rightPressed) {
            EventManager.TriggerIntEvent("Right", playerId);
            selection = FoodLocation.Right;
        }
    }

    
}

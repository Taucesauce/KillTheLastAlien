﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Rewired;
using System;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour {

    public enum PlayerState {
        Idle, // In-between states during "scramble"
        Selecting, //Expecting Lock-In results
        Grabbing, //Animation State, locked input.
        Locked //Has food, during countdown, finished selecting in countdown phase, etc.
    }

    public int playerId; //The Rewired player id of this character 
    private Rewired.Player player;
    private CharacterController cc;
    public PlayerState state;

    IEnumerator IdleState() {
        Debug.Log("Entering Idle for Player " + playerId);
        while(state == PlayerState.Idle) {
            yield return 0;
        }
        Debug.Log("Exiting Idle for Player " + playerId);
        NextState();
    }

    IEnumerator SelectingState() {
        Debug.Log("Entering Selecting for Player " + playerId);
        while (state == PlayerState.Selecting) {
            yield return 0;
        }
        Debug.Log("Exiting Selecting for Player " + playerId);
        NextState();
    }

    IEnumerator GrabbingState() {
        Debug.Log("Entering Grabbing for Player " + playerId);
        while (state == PlayerState.Grabbing) {
            yield return 0;
        }
        Debug.Log("Exiting Grabbing for Player " + playerId);
        NextState();
    }

    IEnumerator LockedState() {
        Debug.Log("Entering Locked for Player " + playerId);
        EventManager.TriggerIntEvent("PlayerLocked", playerId);
        while (state == PlayerState.Locked) {
            yield return 0;
        }
        Debug.Log("Exiting Locked for Player " + playerId);
        EventManager.TriggerIntEvent("PlayerUnlocked", playerId);
        NextState();
    }

    private void NextState() {
        string methodName = state.ToString() + "State";
        System.Reflection.MethodInfo info = GetType().GetMethod(methodName,
                                                                System.Reflection.BindingFlags.NonPublic |
                                                                System.Reflection.BindingFlags.Instance);
        StartCoroutine((IEnumerator)info.Invoke(this, null));
    }

    //Gameplay Variables
    bool leftPressed = false;
    bool midPressed = false;
    bool rightPressed = false;
    FoodColor selection;

    void Awake() {
        player = ReInput.players.GetPlayer(playerId); //use to assign map and access button states

        cc = GetComponent<CharacterController>(); //will need to control anim state and movement of hands
        NextState();
    }

    void OnEnable() {
        EventManager.StartListeningTypeInt("GameStateChange", ChangeState);
    }

    void OnDisable() {
        EventManager.StopListeningTypeInt("GameStateChange", ChangeState);
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
        leftPressed = player.GetButtonDown("GreenSelect");
        midPressed = player.GetButtonDown("OrangeSelect");
        rightPressed = player.GetButtonDown("PinkSelect");
    }

    private void ProcessInput() {
        if (state == PlayerState.Selecting) {
            if (leftPressed) {
                EventManager.TriggerIntEvent("Green", playerId);
                selection = FoodColor.Green;
                state = PlayerState.Locked;
            }
            if (midPressed) {
                EventManager.TriggerIntEvent("Orange", playerId);
                selection = FoodColor.Orange;
                state = PlayerState.Locked;
            }
            if (rightPressed) {
                EventManager.TriggerIntEvent("Pink", playerId);
                selection = FoodColor.Pink;
                state = PlayerState.Locked;
            }
        }
    }

    //Called when Game changes state, adapts accordingly
    void ChangeState(int newGameState) {
        switch ((GameState)newGameState){
            case GameState.Menu:
                state = PlayerState.Idle;
                break;
            case GameState.Decision:
                state = PlayerState.Selecting;
                break;
            case GameState.Scramble:
                state = PlayerState.Grabbing;
                break;
            case GameState.EndRound:
                state = PlayerState.Idle;
                break;
            case GameState.EndGame:
                state = PlayerState.Idle;
                break;
        }
    }
}

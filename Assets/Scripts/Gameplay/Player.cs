using System.Collections;
using UnityEngine;
using Rewired;
using System;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour {

    public enum PlayerState {
        Inactive, //Not being used in this game/No player checked in during select.
        SelectScreen, //Join/Leave game state in Player Select,
        Idle, // In-between states during "scramble"
        Grabbing, //Animation State, locked input.
    }

    public int playerId; //The Rewired player id of this character
    private int score;
    public int Score { get { return score; } }
    public bool isPlaying = false;
    private Rewired.Player player;
    public PlayerState state;

    IEnumerator InactiveState() {
        Debug.Log("Entering Inactive for Player " + playerId);
        while(state == PlayerState.Inactive) {
            yield return 0;
        }
        Debug.Log("Exiting Inactive for Player " + playerId);
        NextState();
    }

    IEnumerator SelectScreenState() {
        Debug.Log("Entering Select Screen State for Player " + playerId);
        while(state == PlayerState.SelectScreen) {
            yield return 0;
        }
        Debug.Log("Exiting Select Screen State for Player " + playerId);
        NextState();
    }

    IEnumerator IdleState() {
        Debug.Log("Entering Idle for Player " + playerId);
        while(state == PlayerState.Idle) {
            yield return 0;
        }
        Debug.Log("Exiting Idle for Player " + playerId);
        NextState();
    }

    IEnumerator GrabbingState() {
        //Debug.Log("Entering Grabbing for Player " + playerId);
        StartLerp();
        while (state == PlayerState.Grabbing) {
            yield return 0;
        }
        //Debug.Log("Exiting Grabbing for Player " + playerId);
        NextState();
    }

    private void NextState() {
        string methodName = state.ToString() + "State";
        System.Reflection.MethodInfo info = GetType().GetMethod(methodName,
                                                                System.Reflection.BindingFlags.NonPublic |
                                                                System.Reflection.BindingFlags.Instance);
        StartCoroutine((IEnumerator)info.Invoke(this, null));
    }

    //Input Variables
    bool leftPressed = false;
    bool midPressed = false;
    bool rightPressed = false;
    bool escPressed = false;
    bool joinPressed = false;

    //Grabbing vars
    Vector2 originalLocation;
    Vector2 selectionLocation;

    [SerializeField]
    float lerpTime = 1f; //time it takes arm to travel to mochi 

    private float startTime;
    float fracJourney;

    //Food Interaction Variables
    FoodColor? selection;
    bool reachedMochi = false;
    bool hasMochi = false;

    void Awake() {
        player = ReInput.players.GetPlayer(playerId); //use to assign map and access button states
        NextState();
    }

    //Event hooks
    void OnEnable() {
        EventManager.StartListeningTypeInt("GameStateChange", ChangeState);
        EventManager.StartListening(playerId + "GrabbedMochi", GetMochi);
    }

    void OnDisable() {
        EventManager.StopListeningTypeInt("GameStateChange", ChangeState);
        EventManager.StopListening(playerId + "GrabbedMochi", GetMochi);
    }

    // Use this for initialization
    void Start () {
        originalLocation = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        GetInput();
        ProcessInput();
        if(state == PlayerState.Grabbing) {
            UpdateLerp();
        }
	}

    //Input Update Functions
    private void GetInput() {
        leftPressed = player.GetButtonDown("GreenSelect");
        midPressed = player.GetButtonDown("OrangeSelect");
        rightPressed = player.GetButtonDown("PinkSelect");
        joinPressed = player.GetButtonDown("JoinGame");
        escPressed = player.GetButtonDown("ExitGame");
    }

    private void ProcessInput() {
        if (escPressed) {
            Application.Quit();
        }
        switch (state) {
            case PlayerState.SelectScreen:
                if (joinPressed && isPlaying) {
                    EventManager.TriggerIntEvent("Leave", playerId);
                }
                else if(joinPressed && !isPlaying){ 
                    EventManager.TriggerIntEvent("Join", playerId);
                }
                break;
            //case PlayerState.Selecting:
            //    if (leftPressed) {
            //        EventManager.TriggerIntEvent("Green", playerId);
            //        selection = FoodColor.Green;
            //        state = PlayerState.Locked;
            //    }
            //    if (midPressed) {
            //        EventManager.TriggerIntEvent("Orange", playerId);
            //        selection = FoodColor.Orange;
            //        state = PlayerState.Locked;
            //    }
            //    if (rightPressed) {
            //        EventManager.TriggerIntEvent("Pink", playerId);
            //        selection = FoodColor.Pink;
            //        state = PlayerState.Locked;
            //    }
            //    break;
            ////Not a fan of the alreadySelected but should work for the time being.
            //case PlayerState.Scramble:
            //    if (leftPressed) {
            //        selection = FoodColor.Green;
            //        bool alreadySelected = GameManager.Instance.Mochis[(int)selection].GetComponent<Food>().isSelected;
            //        if (!alreadySelected) {
            //            state = PlayerState.Grabbing;
            //            EventManager.TriggerIntEvent("Green", playerId);
            //        }
                        
            //    }
            //    if (midPressed) {
            //        selection = FoodColor.Orange;
            //        bool alreadySelected = GameManager.Instance.Mochis[(int)selection].GetComponent<Food>().isSelected;
            //        if (!alreadySelected) {
            //            state = PlayerState.Grabbing;
            //            EventManager.TriggerIntEvent("Orange", playerId);
            //        }
            //    }
            //    if (rightPressed) {
            //        selection = FoodColor.Pink;
            //        bool alreadySelected = GameManager.Instance.Mochis[(int)selection].GetComponent<Food>().isSelected;
            //        if (!alreadySelected) {
            //            state = PlayerState.Grabbing;
            //            EventManager.TriggerIntEvent("Pink", playerId);
            //        }
            //    }
            //    break;         
        }
    }

    //Called when Game changes state, adapts accordingly
    void ChangeState(int newGameState) {
        switch ((GameState)newGameState){
            case GameState.Menu:
                state = PlayerState.Idle;
                break;
            case GameState.Scramble:
                state = PlayerState.Grabbing;
                break;
            case GameState.EndRound:
                if (!hasMochi) { EventManager.TriggerIntEvent("UIFail", playerId); }
                state = PlayerState.Idle;
                break;
            case GameState.EndGame:
                state = PlayerState.Idle;
                break;
            default:
                Debug.Log("Default Player State Change case");
                break;
        }
    }

    //Lerp functions
    void StartLerp() {
        //Set start vars
        selectionLocation = FoodFactory.Instance.nearestMochi(Enum.GetName(typeof(FoodColor), selection), transform.position);

        Vector2 diff = selectionLocation - originalLocation;
        diff.Normalize();
        float zRot = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, zRot);

        startTime = 0;
    }

    void UpdateLerp() {
        if((Vector2)transform.position == selectionLocation && reachedMochi == false) {
            startTime = 0;
            reachedMochi = true;
            EventManager.TriggerIntEvent("GrabMochi", playerId);
        } else if((Vector2)transform.position == originalLocation && reachedMochi == true) {
            if (hasMochi) {
                state = PlayerState.Idle;
                EventManager.TriggerIntEvent("UISuccess", playerId);
                return;
            } else {
                ResetPlayerVariables();
                //state = PlayerState.Scramble;
                return;
            }
        }
        if (reachedMochi) {
            startTime += Time.deltaTime;
            fracJourney = startTime / lerpTime;
            transform.position = Vector3.Lerp(selectionLocation, originalLocation, fracJourney);
        } else {
            startTime += Time.deltaTime;
            fracJourney = startTime / lerpTime;
            transform.position = Vector3.Lerp(originalLocation, selectionLocation, fracJourney);
        }
    }

    //Manipulation of object var functions
    void GetMochi() {
        hasMochi = true;
    }

    void ResetPlayerVariables() {
        reachedMochi = false;
        selection = null;
    }

    public void IncrementScore(int amount) {
        score += amount;
    }
}

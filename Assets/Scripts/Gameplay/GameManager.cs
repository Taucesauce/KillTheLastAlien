using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    //Gameplay Vars:
    //Current gameplay state
    [Header("Gameplay Variables")]
    public GameState state;
    //Number of rounds per game
    [SerializeField]
    private int numRounds;
    [HideInInspector]
    public int NumRounds { get { return numRounds; } }
    
    [SerializeField]
    private int currentRound;
    [HideInInspector]
    public int CurrentRound { get { return currentRound; } }

    private const float timeBetweenRounds = 3f;
    private float countdown = timeBetweenRounds;

    [Header("Players")]
    [SerializeField]
    private GameObject[] players = new GameObject[4];

    [Header("Mochis")]
    [SerializeField]
    private GameObject[] mochis = new GameObject[3];

    [HideInInspector]
    public GameObject[] Mochis { get { return mochis; } }

    public Dictionary<string, Vector2> mochiLocations;

    //State Routines
    IEnumerator MenuState() {
        Debug.Log("Entering Menu State");
        EventManager.TriggerIntEvent("GameStateChange", (int)GameState.Menu);
        while (state == GameState.Menu) {
            yield return 0;
        }
        Debug.Log("Exiting Menu State");
        NextState();
    }

    IEnumerator DecisionState() {
        Debug.Log("Entering Decision State");
        EventManager.TriggerIntEvent("GameStateChange", (int)GameState.Decision);
        while (state == GameState.Decision) {
            yield return 0;
        }
        Debug.Log("Exiting Decicion State");
        NextState();
    }

    IEnumerator ScrambleState() {
        Debug.Log("Entering Scramble State");
        EventManager.TriggerIntEvent("GameStateChange", (int)GameState.Scramble);
        while (state == GameState.Scramble) {
            yield return 0;
        }
        Debug.Log("Exiting Scramble State");
        NextState();
    }

    IEnumerator EndRoundState() {
        Debug.Log("Entering End Round State");
        countdown = timeBetweenRounds;
        EventManager.TriggerIntEvent("GameStateChange", (int)GameState.EndRound);
        while (state == GameState.EndRound) {
            yield return 0;
        }
        Debug.Log("Exiting End Round State");
        EventManager.TriggerEvent("RoundReset");
        NextState();
    }

    IEnumerator EndGameState() {
        Debug.Log("Entering End Game State");
        EventManager.TriggerIntEvent("GameStateChange", (int)GameState.EndGame);
        while (state == GameState.EndGame) {
            yield return 0;
        }
        Debug.Log("Exiting End Game State");
        NextState();
    }

    //Determines next state by current state's name and invokes the next coroutine
    private void NextState() {
        string methodName = state.ToString() + "State";
        System.Reflection.MethodInfo info = GetType().GetMethod(methodName,
                                                                System.Reflection.BindingFlags.NonPublic |
                                                                System.Reflection.BindingFlags.Instance);
        StartCoroutine((IEnumerator)info.Invoke(this, null));
    }

    //Singleton pattern
    //Private manager instance
    private static GameManager gameManager;

    public static GameManager Instance {
        get {
            if (!gameManager) {
                gameManager = FindObjectOfType(typeof(GameManager)) as GameManager;

                if(!gameManager) {
                    Debug.Log("No Game Manager object found in scene, fix that!");
                } else {
                    gameManager.Init();
                }
            }

            return gameManager;
        }
    }

    //Accessible position data for Player object
    void Init() {
        if(mochiLocations == null) {
            mochiLocations = new Dictionary<string, Vector2>();
            mochiLocations.Add("Green", (Vector2)mochis[(int)FoodColor.Green].GetComponent<Transform>().position);
            mochiLocations.Add("Orange", (Vector2)mochis[(int)FoodColor.Orange].GetComponent<Transform>().position);
            mochiLocations.Add("Pink", (Vector2)mochis[(int)FoodColor.Pink].GetComponent<Transform>().position);
        }
    }

    //Event hooks
    void OnEnable() {
        EventManager.StartListening("GameStart", GameStart);
    }

    void OnDisable() {
        EventManager.StopListening("GameStart", GameStart);
    }

	//Get GameManager up and running
	void Start () {
        Debug.Log(Instance);
        NextState();
    }
	
	// Handle Update logic based on current game state.
	void Update () {
        switch (state) {
            case GameState.Decision:
                DecisionStateLogic();
                break;
            case GameState.Scramble:
                ScrambleStateLogic();
                break;
            case GameState.EndRound:
                EndRoundStateLogic();
                break;
        }
	}

    //Game State Logic Functions
    void DecisionStateLogic() {
        foreach(GameObject player in players) {
            if (player.GetComponent<Player>().state == Player.PlayerState.Selecting) {
                return;
            }
        }
        state = GameState.Scramble;
    }

    void ScrambleStateLogic() {
        int playersIdle = 0;
        foreach(GameObject player in players) {
            if (player.GetComponent<Player>().state == Player.PlayerState.Idle) {
                playersIdle++;
            }
        }
        if (playersIdle >= 3)
            state = GameState.EndRound;
        else
            return;
    }

    void EndRoundStateLogic() {
        if(currentRound <= numRounds) {
            countdown -= Time.deltaTime;
            if(countdown <= 0.0f) {
                currentRound++;
                state = GameState.Decision;
            }
        } else {
            state = GameState.EndGame;
        }
    }

    void GameStart() {
        state = GameState.Decision;
    }
}

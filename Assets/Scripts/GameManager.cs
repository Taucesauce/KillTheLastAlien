using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public GameState state;
    Dictionary<int, bool> playersLocked;
    public Dictionary<string, Vector2> mochiLocations;

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
        EventManager.TriggerIntEvent("GameStateChange", (int)GameState.EndRound);
        while (state == GameState.EndRound) {
            yield return 0;
        }
        Debug.Log("Exiting End Round State");
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

    void Init() {
        if(playersLocked == null) {
            playersLocked = new Dictionary<int, bool>();
            playersLocked.Add(0, false);
            playersLocked.Add(1, false);
            playersLocked.Add(2, false);
            playersLocked.Add(3, false);

            mochiLocations = new Dictionary<string, Vector2>();
            mochiLocations.Add("Green", GameObject.Find("GreenMochi").transform.position);
            mochiLocations.Add("Orange", GameObject.Find("OrangeMochi").transform.position);
            mochiLocations.Add("Pink", GameObject.Find("PinkMochi").transform.position);
        }
    }

    void OnEnable() {
        EventManager.StartListeningTypeInt("PlayerLocked", PlayerLocked);
        EventManager.StartListeningTypeInt("PlayerUnlocked", PlayerUnlocked);
    }

    void OnDisable() {
        EventManager.StartListeningTypeInt("PlayerLocked", PlayerLocked);
        EventManager.StartListeningTypeInt("PlayerUnlocked", PlayerUnlocked);
    }

	// Use this for initialization
	void Start () {
        Debug.Log(Instance);
        NextState();
    }
	
	// Update is called once per frame
	void Update () {
        switch (state) {
            case GameState.Decision:
                DecisionStateLogic();
                break;
        }
	}

    void DecisionStateLogic() {
        foreach(KeyValuePair<int,bool> player in playersLocked) {
            if (!player.Value) {
                return;
            }
        }
        state = GameState.Scramble;
    }

    void PlayerLocked(int player) {
        playersLocked[player] = true;
    }

    void PlayerUnlocked(int player) {
        playersLocked[player] = false;
    }
}

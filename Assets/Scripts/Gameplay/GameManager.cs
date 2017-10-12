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

    private const float timeBetweenRounds = 6f;
    private float countdown = timeBetweenRounds;

    //State Routines
    IEnumerator MenuState() {
        //Debug.Log("Entering Menu State");
        EventManager.TriggerEvent("MenuScreen");
        EventManager.TriggerIntEvent("GameStateChange", (int)GameState.Menu);
        while (state == GameState.Menu) {
            yield return 0;
        }
        //Debug.Log("Exiting Menu State");
        NextState();
    }

    IEnumerator PlayerSelectState() {
        //Debug.Log("Entering Player Select State");
        EventManager.TriggerEvent("PlayerSelect");
        EventManager.TriggerIntEvent("GameStateChange", (int)GameState.PlayerSelect);
        while(state == GameState.PlayerSelect) {
            yield return 0;
        }
        //Debug.Log("Exiting Player Select State");
        EventManager.TriggerEvent("GameStart");
        NextState();
    }

    IEnumerator RoundActiveState() {
        //Debug.Log("Entering Round Active State");
        EventManager.TriggerIntEvent("GameStateChange", (int)GameState.RoundActive);
        while(state == GameState.RoundActive) {
            yield return 0;
        }
        //Debug.Log("Exiting Round Active State");
        NextState();
    }

    IEnumerator EndRoundState() {
        //Debug.Log("Entering End Round State");
        countdown = timeBetweenRounds;
        EventManager.TriggerEvent("EndRoundUI");
        EventManager.TriggerIntEvent("GameStateChange", (int)GameState.EndRound);
        while (state == GameState.EndRound) {
            yield return 0;
        }
        //Debug.Log("Exiting End Round State");
        EventManager.TriggerEvent("RoundReset");
        NextState();
    }

    IEnumerator EndGameState() {
        //Debug.Log("Entering End Game State");
        EventManager.TriggerEvent("EndGameUI");
        EventManager.TriggerIntEvent("GameStateChange", (int)GameState.EndGame);
        EndGameStateLogic();
        while (state == GameState.EndGame) {
            yield return 0;
        }
        //Debug.Log("Exiting End Game State");
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
        NextState();
    }
	
	// Handle Update logic based on current game state.
	void Update () {
        switch (state) {
            case GameState.PlayerSelect:
                PlayerSelectLogic();
                break;
            case GameState.RoundActive:
                RoundActiveStateLogic();
                break;
            case GameState.EndRound:
                EndRoundStateLogic();
                break;
            default:
                //Debug.Log("Default GameManager state case");
                break;
        }
	}

    //Game State Logic Functions
    void PlayerSelectLogic() {
        if (Input.GetKeyDown(KeyCode.Space) && PlayerManager.Instance.GetActivePlayerCount() >= 2) {
            state = GameState.RoundActive;
        }
    }

    void RoundActiveStateLogic() {
        if(FoodFactory.Instance.FoodList.Count <= 0) {
            if (PlayerManager.Instance.AllPlayersAtStartPos()) {
                state = GameState.EndRound;
            }
        }

        UIManager.Instance.SetCrown(DetermineCurrentWinner());
    }

    int DetermineCurrentWinner() {
        int highestScore = 0;
        int highestPlayerID = -1;

        foreach (Player player in PlayerManager.Instance.CurrentPlayers) {
            if (player.Score > highestScore) {
                highestScore = player.Score;
                highestPlayerID = player.playerId;
            }
        }

        return highestPlayerID;
    }

    void EndRoundStateLogic() {
        if(currentRound < numRounds) {
            countdown -= Time.deltaTime;
            UIManager.Instance.UpdateEndRoundCounter((int)countdown);
            if(countdown <= 0.0f) {
                currentRound++;
                UIManager.Instance.UpdateCurrentRound(currentRound);
                FoodFactory.Instance.SpawnFood(PlayerManager.Instance.GetActivePlayerCount());
                state = GameState.RoundActive;
            }
        } else {
            state = GameState.EndGame;
        }
    }

    //This feels like it could be optimized by sorting/selecting differently but I'm out of time!
    void EndGameStateLogic() {
        int highestScore = 1;
        Dictionary<int, int> playerScores = new Dictionary<int, int>();
        List<int> highestScoringPlayers = new List<int>();
        foreach(Player player in PlayerManager.Instance.CurrentPlayers) {
            playerScores.Add(player.playerId, player.Score);

            if(player.Score > highestScore) {
                highestScore = player.Score;
            }
        }

        foreach(KeyValuePair<int,int> playerScore in playerScores) {
            if(playerScore.Value == highestScore) {
                highestScoringPlayers.Add(playerScore.Key);
            }
        }

        UIManager.Instance.SetWinnerText(highestScoringPlayers);
    }

    void GameStart() {
        currentRound = 1;
        FoodFactory.Instance.SpawnFood(PlayerManager.Instance.GetActivePlayerCount());
    }

    public void StartButton() {
        EventManager.TriggerEvent("PlayerSelect");
        state = GameState.PlayerSelect;
    }

    public void ReturnToMenu() {
        state = GameState.Menu;
    }
    public void GameExit() {
        Application.Quit();
    }
}

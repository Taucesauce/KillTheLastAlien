using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    //Singleton pattern 
    //Private manager instance
    private static PlayerManager playerManager;
    [SerializeField]
    private List<Player> currentPlayers = new List<Player>();
    public List<Player>CurrentPlayers { get { return currentPlayers; } }

    public static PlayerManager Instance {
        get {
            if (!playerManager) {
                playerManager = FindObjectOfType(typeof(PlayerManager)) as PlayerManager;

                if(!playerManager) {
                    Debug.Log("No Player Manager object found in scene, fix that!");
                } else {
                    playerManager.Init();
                }
            }

            return playerManager;
        }
    }

    void Init() {

    }

	// Use this for initialization
	void Start () {
        
	}

    void OnEnable() {
        EventManager.StartListeningTypeInt("Join", ActivatePlayer);
        EventManager.StartListeningTypeInt("Leave", DeactivatePlayer);
        EventManager.StartListening("GameStart", RemovePlayers);
    }

    void OnDisable() {
        EventManager.StopListeningTypeInt("Join", ActivatePlayer);
        EventManager.StopListeningTypeInt("Leave", DeactivatePlayer);
        EventManager.StopListening("GameStart", RemovePlayers);
    }

    // Update is called once per frame
    void Update () {
		
	}

    void ActivatePlayer(int playerID) {
        currentPlayers[playerID].state = Player.PlayerState.Idle;
    }

    void DeactivatePlayer(int playerID) {
        currentPlayers[playerID].state = Player.PlayerState.SelectScreen;
    }

    void RemovePlayers() {
        foreach(Player player in currentPlayers) {
            if(player.state == Player.PlayerState.SelectScreen) {
                player.state = Player.PlayerState.Inactive;
            }
        }
    }
    public int GetPlayerScore(int playerID) {
        return currentPlayers[playerID].Score;
    }

    void IncrementScore(int playerID) {
        currentPlayers[playerID].IncrementScore(1);
    }
}

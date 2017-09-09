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
        EventManager.StartListening("PlayerSelect", PlayerSelect);
        EventManager.StartListening("GameStart", RemovePlayers);
    }

    void OnDisable() {
        EventManager.StopListeningTypeInt("Join", ActivatePlayer);
        EventManager.StopListeningTypeInt("Leave", DeactivatePlayer);
        EventManager.StopListening("PlayerSelect", PlayerSelect);
        EventManager.StopListening("GameStart", RemovePlayers);
    }

    // Update is called once per frame
    void Update () {
		
	}

    void PlayerSelect() {
        foreach(Player player in CurrentPlayers) {
            player.state = Player.PlayerState.SelectScreen;
        }
    }
    void ActivatePlayer(int playerID) {
        currentPlayers[playerID].isPlaying = true;
    }

    void DeactivatePlayer(int playerID) {
        currentPlayers[playerID].isPlaying = false;
    }

    void RemovePlayers() {
        foreach(Player player in currentPlayers) {
            if(!player.isPlaying) {
                player.state = Player.PlayerState.Inactive;
            } else {
                player.state = Player.PlayerState.Idle;
            }
        }
    }
    public int GetPlayerScore(int playerID) {
        return currentPlayers[playerID].Score;
    }

    public int GetActivePlayerCount() {
        return currentPlayers.FindAll(p => p.isPlaying).Count;
    }
    void IncrementScore(int playerID) {
        currentPlayers[playerID].IncrementScore(1);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    //Singleton pattern
    //Private manager instance
    private static PlayerManager playerManager;
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
        AddPlayer(0);
        AddPlayer(1);
        AddPlayer(2);
        AddPlayer(3);
	}

    void OnEnable() {
        EventManager.StartListeningTypeInt("AddPlayer", AddPlayer);
    }

    void OnDisable() {
        EventManager.StopListeningTypeInt("AddPlayer", RemovePlayer);
    }
    // Update is called once per frame
    void Update () {
		
	}

    void AddPlayer(int playerID) {
        currentPlayers.Add(new Player { playerId = playerID });
    }

    void RemovePlayer(int playerID) {
        currentPlayers.RemoveAll(p => p.playerId == playerID);
    }

    public int GetPlayerScore(int playerID) {
        return currentPlayers.Find(p => p.playerId == playerID).Score;
    }

    void IncrementScore(int playerID) {
        currentPlayers.Find(p => p.playerId == playerID).IncrementScore(1);
    }
}

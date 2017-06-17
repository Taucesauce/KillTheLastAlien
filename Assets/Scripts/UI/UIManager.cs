using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    [Header("Menu Vars")]
    [SerializeField]
    private Canvas MainMenuCanvas;

    [Header("Gameplay UI Vars")]
    [SerializeField]
    private GUIText P1Image;
    private GUIText P2Image;
    private GUIText P3Image;
    private GUIText P4Image;
    private GUIText RoundTimer;

	private static UIManager manager;

	public static UIManager Instance
	{
		get
		{
			if (!manager)
			{
				manager = FindObjectOfType(typeof(UIManager)) as UIManager;

				if (!manager)
				{
					Debug.Log("No UI Manager object found in scene, fix that!");
				}
				else
				{
					manager.Init();
				}
			}

			return manager;
		}
	}

	void Init()
	{
		
	}

    void OnEnable()
    {
        EventManager.StartListeningTypeBool("ScoreboardDisplay", DisplayScoreboard);
    }

    void OnDisable()
    {
        
    }

    void DisplayScoreboard(bool isShowing){
        //TODO Set scoreboard image to isShowing
    }
}

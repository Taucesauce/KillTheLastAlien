using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    [HideInInspector]
    public enum MenuButtons {
        Start,
        HowTo,
        Exit
    }

    [Header("Menu Canvases")]
    [SerializeField]
    private Canvas MainMenuCanvas;
    [SerializeField]
    private Canvas HowToCanvas;
    [SerializeField]
    private Canvas ConfirmExitCanvas;

    [Header("Menu Sprites")]
    [SerializeField]
    private GameObject startSprite;
    [SerializeField]
    private GameObject howToSprite;
    [SerializeField]
    private GameObject exitSprite;

    [Header("Gameplay UI Vars")]
    [SerializeField]
    private Canvas GameCanvas;

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
    void Start() {
        startSprite.GetComponent<Animator>().enabled = false;
        howToSprite.GetComponent<Animator>().enabled = false;
        exitSprite.GetComponent<Animator>().enabled = false;
    }

	void Init()
	{
        
    }

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        
    }

    //Canvas Display methods
    public void DisplayMenu(bool isDisplayed) {
        MainMenuCanvas.gameObject.SetActive(isDisplayed);
    }

    public void DisplayHowTo(bool isDisplayed) {
        HowToCanvas.gameObject.SetActive(isDisplayed);
    }

    public void DisplayConfirmExit(bool isDisplayed) {
        ConfirmExitCanvas.gameObject.SetActive(isDisplayed);
    }

    public void DisplayGameCanvas(bool isDisplayed) {
        GameCanvas.gameObject.SetActive(isDisplayed);
    }

    //Inspector doesn't accept Enums as parameters for event functions.
    //That's actually, really dumb.
    //Short for time so just casting it but makes the inspector input really unclear :(
    public void MenuButtonEnterHover(int button) {
        switch ((MenuButtons)button) {
            case MenuButtons.Start:
                startSprite.GetComponent<Animator>().enabled = true;
                break;
            case MenuButtons.HowTo:
                howToSprite.GetComponent<Animator>().enabled = true;
                break;
            case MenuButtons.Exit:
                exitSprite.GetComponent<Animator>().enabled = true;
                break;
        }
    }

    public void MenuButtonExitHover(int button) {
        switch ((MenuButtons)button) {
            case MenuButtons.Start:
                startSprite.GetComponent<Animator>().enabled = false;
                break;
            case MenuButtons.HowTo:
                howToSprite.GetComponent<Animator>().enabled = false;
                break;
            case MenuButtons.Exit:
                exitSprite.GetComponent<Animator>().enabled = false;
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    //Menu Enum
    [HideInInspector]
    public enum MenuButtons {
        Start,
        HowTo,
        Exit
    }

    //Menu vars:
    private int currentHowToIndex = 0;
    private Image currentHowToImage;

    [Header("Menu Canvases")]
    [SerializeField]
    private Canvas MainMenuCanvas;
    [SerializeField]
    private Canvas HowToCanvas;
    [SerializeField]
    private Canvas PlayerSelectCanvas;

    [Header("Menu Sprites")]
    [SerializeField]
    private GameObject startSprite;
    [SerializeField]
    private GameObject howToSprite;
    [SerializeField]
    private GameObject exitSprite;
    [SerializeField]
    private Image howToBack;
    [SerializeField]
    private Image howToNext;
    [SerializeField]
    private Sprite[] howToPanels = new Sprite[4];
    [SerializeField]
    private Sprite[] howToBackButtons = new Sprite[2];
    [SerializeField]
    private Sprite[] howToNextButtons = new Sprite[2];

    [Header("Player Select Sprites")]
    //Game UI Vars:
    [Header("Gameplay UI Vars")]
    [SerializeField]
    private Canvas GameCanvas;
    [SerializeField]
    private Transform[] PlayerUIObjects = new Transform[4]; 
    [SerializeField]
    private Text P1Score;
    [SerializeField]
    private Text P2Score;
    [SerializeField]
    private Text P3Score;
    [SerializeField]
    private Text P4Score;
    [SerializeField]
    private GameObject CurrentRoundObject;
    private Text CurrentRoundText;
    [SerializeField]
    private GameObject NextRoundCounter;
    private Text NextRoundText;
    [SerializeField]
    private GameObject[] WinnerText = new GameObject[4];
    [SerializeField]
    private GameObject ReturnButton;

    //UI Lerp Vars
    [Header("UI Lerp Vars")]
    [SerializeField]
    private Vector2[] originalPosAnchors = new Vector2[4];
    [SerializeField]
    private Vector2[] UIAnchors = new Vector2[4];
    private bool slideOut = false;
    private bool slideIn = false;
    private float slideStartTime = 0f;
    private float lerpTime = 2f;
    private float fracJourney;
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

    //--Setup functions--
    void Start() {
        startSprite.GetComponent<Animator>().enabled = false;
        howToSprite.GetComponent<Animator>().enabled = false;
        exitSprite.GetComponent<Animator>().enabled = false;
        currentHowToImage = HowToCanvas.GetComponentInChildren<Image>();
        NextRoundText = NextRoundCounter.GetComponent<Text>();
        CurrentRoundText = CurrentRoundObject.GetComponent<Text>();
    }

	void Init()
	{
        //No init logic needed right now? Handled in start
    }

    void Update() {
        if (slideOut) {
            ScoreSlideOut();
        }
        if(slideIn) {
            ScoreSlideIn();
        }
    }

    //Should have used a state machine here instead of all of these events, hacky rush job.
    void OnEnable()
    {
        EventManager.StartListeningTypeInt("PlayerSuccess", UpdateScore);
        EventManager.StartListening("EndRoundUI", EndRoundUI);
        EventManager.StartListening("PlayerSelect", PlayerSelect);
        EventManager.StartListening("RoundReset", RoundResetUI);
        EventManager.StartListening("EndGameUI", EndGameUI);
        EventManager.StartListening("MenuScreen", MenuFromGame);
        EventManager.StartListening("GameStart", GameStart);
    }

    void OnDisable()
    {
        EventManager.StopListeningTypeInt("PlayerSuccess", UpdateScore);
        EventManager.StopListening("EndRoundUI", EndRoundUI);
        EventManager.StartListening("PlayerSelect", PlayerSelect);
        EventManager.StopListening("RoundReset", RoundResetUI);
        EventManager.StartListening("EndGameUI", EndGameUI);
        EventManager.StartListening("MenuScreen", MenuFromGame);
        EventManager.StopListening("GameStart", GameStart);
    }

    //--End Setup Functions--

    //--Menu Nav functions--
    public void GameStart() {
        slideOut = false;
        DisplayMenu(false);
        DisplayGameCanvas(true);
        DisplayPlayerSelectCanvas(false);
        
        P1Score.text = "0";
        P2Score.text = "0";
        P3Score.text = "0";
        P4Score.text = "0";

        CurrentRoundObject.SetActive(true);
        CurrentRoundText.text = "Round 1";
        ReturnButton.SetActive(false);
        for (int i = 0; i < 4; i++) {
            PlayerUIObjects[i].position = originalPosAnchors[i];
            WinnerText[i].SetActive(false);
        }
    }

    public void PlayerSelect() {
        DisplayMenu(false);
        DisplayPlayerSelectCanvas(true);
    }
    public void InitializeHowToMenu() {
        DisplayMenu(false);
        DisplayHowTo(true);
    }

    public void BackToMain() {
        DisplayHowTo(false);
        DisplayMenu(true);
        currentHowToIndex = 0;
        currentHowToImage.sprite = howToPanels[0];
        howToBack.sprite = howToBackButtons[0];
        howToNext.sprite = howToNextButtons[0];
    }

    public void MenuFromGame() {
        DisplayGameCanvas(false);
        DisplayMenu(true);
    }

    public void navHowToBack() {
        if(currentHowToIndex == 0) {
            BackToMain();
            return;
        }

        currentHowToIndex--;
        switch (currentHowToIndex) {
            case 0:
                howToBack.sprite = howToBackButtons[0];
                break;
            case 2:
                howToNext.sprite = howToNextButtons[0];
                break;
            default:
                break;
        }

        currentHowToImage.sprite = howToPanels[currentHowToIndex];
    }

    public void navHowToNext() {
        if (currentHowToIndex == howToPanels.Length - 1) {
            BackToMain();
            return;
        }

        currentHowToIndex++;
        switch (currentHowToIndex) {
            case 1:
                howToBack.sprite = howToBackButtons[1];
                break;
            case 3:
                howToNext.sprite = howToNextButtons[1];
                break;
            default:
                break;
        }

        currentHowToImage.sprite = howToPanels[currentHowToIndex];
    }
    //--End Menu Nav Functions--

    //--Canvas Display methods--
    public void DisplayMenu(bool isDisplayed) {
        MainMenuCanvas.gameObject.SetActive(isDisplayed);
    }

    public void DisplayPlayerSelectCanvas(bool isDisplayed) {
        PlayerSelectCanvas.gameObject.SetActive(isDisplayed);
    }

    public void DisplayHowTo(bool isDisplayed) {
        HowToCanvas.gameObject.SetActive(isDisplayed);
    }

    public void DisplayGameCanvas(bool isDisplayed) {
        GameCanvas.gameObject.SetActive(isDisplayed);
    }
    //--End Canvas Display methods--

    //Gameplay Score Events
    private void UpdateScore(int playerID) {
        switch (playerID) {
            case 0:
                P1Score.text = PlayerManager.Instance.GetPlayerScore(playerID).ToString();
                break;
            case 1:
                P2Score.text = PlayerManager.Instance.GetPlayerScore(playerID).ToString();
                break;
            case 2:
                P3Score.text = PlayerManager.Instance.GetPlayerScore(playerID).ToString();
                break;
            case 3:
                P4Score.text = PlayerManager.Instance.GetPlayerScore(playerID).ToString();
                break;
        }
    }
    //--End Gameplay Update Events--

    //--Round End/Round Reset Functions--
    private void RoundResetUI() {
        slideStartTime = 0f;
        slideOut = false;
        slideIn = true;
        CurrentRoundObject.SetActive(true);
        NextRoundCounter.SetActive(false);
    }

    private void EndRoundUI() {
        slideStartTime = 0f;
        slideOut = true;
        slideIn = false;
        CurrentRoundObject.SetActive(false);
        NextRoundCounter.SetActive(true);
    }

    private void EndGameUI() {
        EndRoundUI();
        NextRoundCounter.SetActive(false);
        ReturnButton.SetActive(true);
    }

    private void ScoreSlideIn() {
        slideStartTime += Time.deltaTime;
        fracJourney = slideStartTime / lerpTime;
        for(int i = 0; i < 4; i++) {
            PlayerUIObjects[i].position = Vector2.Lerp(UIAnchors[i], originalPosAnchors[i], fracJourney);
        }
    }

    private void ScoreSlideOut() {
        slideStartTime += Time.deltaTime;
        fracJourney = slideStartTime / lerpTime;
        for (int i = 0; i < 4; i++) {
            PlayerUIObjects[i].position = Vector2.Lerp(originalPosAnchors[i], UIAnchors[i], fracJourney);
        }
    }

    public void UpdateEndRoundCounter(int time) {
        NextRoundText.text = "Next Round Begins In: " + time;
    }

    public void UpdateCurrentRound(int round) {
        CurrentRoundText.text = "Round " + round;
    }

    public void SetWinnerText(List<int> winners) {
        foreach(int winner in winners) {
            WinnerText[winner].SetActive(true);
        }
    }
    //--End Round End/Round Reset Functions--

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

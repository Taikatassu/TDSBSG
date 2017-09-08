using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	public static UIManager _instance;

	Toolbox toolbox;
	EventManager em;

	public Transform mainMenuHolder;
	Button startButton;
	Button quitButton;

    bool isPause = false;

    Button removeButton;
    Button restartButton;
    Button exitGameButton;


	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else if (_instance != this) {
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);

		toolbox = FindObjectOfType<Toolbox>();
		em = toolbox.GetComponent<EventManager>();

		GameObject newStartButton = Instantiate(Resources.Load("UI/MainMenuButton_Base") as GameObject, mainMenuHolder);
		startButton = newStartButton.GetComponent<Button>();
		startButton.GetComponentInChildren<Text>().text = "PLAY";
		startButton.onClick.AddListener(OnStartButtonPressed);

		GameObject newQuitButton = Instantiate(Resources.Load("UI/MainMenuButton_Base") as GameObject, mainMenuHolder);
		quitButton = newQuitButton.GetComponent<Button>();
		quitButton.GetComponentInChildren<Text>().text = "QUIT";
		quitButton.onClick.AddListener(OnQuitButtonPressed);


        GameObject newRemoveButton = Instantiate(Resources.Load("UI/MainMenuButton_Base") as GameObject, mainMenuHolder);
        removeButton = newRemoveButton.GetComponent<Button>();
        removeButton.GetComponentInChildren<Text>().text = "REMOVE";
        removeButton.onClick.AddListener(OnRemoveButtonPressed);
        removeButton.gameObject.SetActive(isPause);

        GameObject newRestartButton = Instantiate(Resources.Load("UI/MainMenuButton_Base") as GameObject, mainMenuHolder);
        restartButton = newRestartButton.GetComponent<Button>();
        restartButton.GetComponentInChildren<Text>().text = "RESTART";
        restartButton.onClick.AddListener(OnRestartButtonPressed);
        restartButton.gameObject.SetActive(isPause);

        GameObject newExitGameButton = Instantiate(Resources.Load("UI/MainMenuButton_Base") as GameObject, mainMenuHolder);
        exitGameButton = newExitGameButton.GetComponent<Button>();
        exitGameButton.GetComponentInChildren<Text>().text = "EXITGAME";
        exitGameButton.onClick.AddListener(OnExitGameButtonPressed);
        exitGameButton.gameObject.SetActive(isPause);

    }

	private void OnStartButtonPressed() {
		Debug.Log("Start button pressed");
		//Start the game (load first level, close main menu)
		em.BroadcastRequestLoadLevel("Level_ShotaTest");
	}

	private void OnQuitButtonPressed() {
		Debug.Log("Quit button pressed");
		//Stop everything, close application
	}

    private void OnRemoveButtonPressed()
    {
        DisablePause();
        isPause = false;
    }

    private void OnRestartButtonPressed()
    {

    }

    private void OnExitGameButtonPressed()
    {
        DisablePause();
        em.BroadcastRequestLoadLevel("MainMenu");
    }

    void DisableMainMenu()
	{
		startButton.gameObject.SetActive(false);
		quitButton.gameObject.SetActive(false);
	}

    void DisablePause()
    {
        removeButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        exitGameButton.gameObject.SetActive(false);
    }

	void OnLevelFinishedLoadingPlayScene(Scene scene, LoadSceneMode mode)
	{
		DisableMainMenu();

		SceneManager.sceneLoaded -= OnLevelFinishedLoadingPlayScene;
	}
}

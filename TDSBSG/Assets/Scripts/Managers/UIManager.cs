using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
	public static UIManager _instance;

	Toolbox toolbox;
	EventManager em;

	// MainMenu's button
	public Transform mainMenuHolder;
	Button startButton;
	Button quitButton;

	// PauseMenu's button
	public Transform pauseMenuHolder;
	Button removeButton;
	Button restartButton;
	Button exitGameButton;


	private void Awake() {
		if (_instance == null) {
			_instance = this;
		}
		else if (_instance != this) {
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);

		toolbox = FindObjectOfType<Toolbox>();
		em = toolbox.GetComponent<EventManager>();

		CreateMainMenu();
		CreatePauseMenu();

		EnableMainMenu();
		DisablePauseMenu();
	}

	private void OnEnable() {
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
		em.OnPauseStateChange += OnPauseStateChange;
	}

	private void OnDisable() {
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
		em.OnPauseStateChange -= OnPauseStateChange;
	}

	private void CreateMainMenu() {
		GameObject newStartButton = Instantiate(Resources.Load("UI/MainMenuButton_Base") as GameObject, mainMenuHolder);
		startButton = newStartButton.GetComponent<Button>();
		startButton.GetComponentInChildren<Text>().text = "PLAY";
		startButton.onClick.AddListener(OnStartButtonPressed);

		GameObject newQuitButton = Instantiate(Resources.Load("UI/MainMenuButton_Base") as GameObject, mainMenuHolder);
		quitButton = newQuitButton.GetComponent<Button>();
		quitButton.GetComponentInChildren<Text>().text = "QUIT";
		quitButton.onClick.AddListener(OnQuitButtonPressed);
	}

	private void CreatePauseMenu() {
		GameObject newResumeButton = Instantiate(Resources.Load("UI/MainMenuButton_Base") as GameObject, pauseMenuHolder);
		removeButton = newResumeButton.GetComponent<Button>();
		removeButton.GetComponentInChildren<Text>().text = "RESUME";
		removeButton.onClick.AddListener(OnResumeButtonPressed);

		GameObject newRestartButton = Instantiate(Resources.Load("UI/MainMenuButton_Base") as GameObject, pauseMenuHolder);
		restartButton = newRestartButton.GetComponent<Button>();
		restartButton.GetComponentInChildren<Text>().text = "RESTART";
		restartButton.onClick.AddListener(OnRestartButtonPressed);

		GameObject newExitGameButton = Instantiate(Resources.Load("UI/MainMenuButton_Base") as GameObject, pauseMenuHolder);
		exitGameButton = newExitGameButton.GetComponent<Button>();
		exitGameButton.GetComponentInChildren<Text>().text = "EXIT";
		exitGameButton.onClick.AddListener(OnExitGameButtonPressed);
	}

		private void OnStartButtonPressed() {
		Debug.Log("Start button pressed");
		//Start the game (load first level, close main menu)
		em.BroadcastRequestLoadLevel("Level_JuhoTest");
	}

	private void OnQuitButtonPressed() {
		Debug.Log("Quit button pressed");
		//Stop everything, close application
	}

	private void OnResumeButtonPressed() {
		em.BroadcastPauseStateChange(false);
	}

	private void OnRestartButtonPressed() {

	}

	private void OnExitGameButtonPressed() {
		em.BroadcastRequestLoadLevel("MainMenu");
	}

	private void EnableMainMenu() {
		mainMenuHolder.gameObject.SetActive(true);
	}

	private void DisableMainMenu() {
		mainMenuHolder.gameObject.SetActive(false);
	}

	private void EnablePauseMenu() {
		pauseMenuHolder.gameObject.SetActive(true);
	}

	private void DisablePauseMenu() {
		pauseMenuHolder.gameObject.SetActive(false);
	}

	void OnPauseStateChange(bool newPauseState) {
		if (newPauseState) {
			Debug.Log("PauseGame");
			EnablePauseMenu();
		} else {
			Debug.Log("ResumeGame");
			DisablePauseMenu();
		}
	}

	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
		if (scene.name == "Level_ShotaTest") {
			DisableMainMenu();
		}
		if (scene.name == "Level_JuhoTest") {
			DisableMainMenu();
		}
	}
}
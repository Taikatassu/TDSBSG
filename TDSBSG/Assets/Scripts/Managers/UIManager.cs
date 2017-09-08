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

<<<<<<< HEAD
	private void Awake() {
		if (_instance == null) {
=======
    Button removeButton;
    Button restartButton;


	private void Awake()
	{
		if (_instance == null)
		{
>>>>>>> e8e0994c7343bb137f34327e1de39c8b0412f86a
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

<<<<<<< HEAD
	void DisableMainMenu() {
=======
    private void OnRemoveButtonPressed()
    {

    }

	void DisableMainMenu()
	{
>>>>>>> e8e0994c7343bb137f34327e1de39c8b0412f86a
		startButton.gameObject.SetActive(false);
		quitButton.gameObject.SetActive(false);
	}
}

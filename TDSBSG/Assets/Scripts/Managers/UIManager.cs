using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager _instance;

    Toolbox toolbox;
    EventManager em;

    public Transform mainMenuHolder;
    Button startButton;
    Button quitButton;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
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
    }

    private void OnEnable()
    {

    }

    private void OnStartButtonPressed()
    {
        Debug.Log("Start button pressed");
        //Start the game (load first level, close main menu)

        SceneManager.sceneLoaded += OnLevelFinishedLoadingPlayScene;

        SceneManager.LoadScene("Level_ShotaTest");

    }

    private void OnQuitButtonPressed()
    {
        Debug.Log("Quit button pressed");
        //Stop everything, close application
    }

    void DisableMainMenu()
    {
        startButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
    }

    void OnLevelFinishedLoadingPlayScene(Scene scene, LoadSceneMode mode)
    {
        DisableMainMenu();

        SceneManager.sceneLoaded -= OnLevelFinishedLoadingPlayScene;
    }
}

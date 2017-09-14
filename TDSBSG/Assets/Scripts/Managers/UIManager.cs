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

    // MainMenu
    public Transform mainMenuHolder;
    Button startButton;
    Button quitButton;

    // PauseMenu
    public Transform pauseMenuHolder;
    Button resumeButton;
    Button restartButton;
    Button exitGameButton;

    // BlackPanel
    [SerializeField]
    Image blackPanel;
    [SerializeField]
    float fadeSpeed = 0.02f;
    bool isFading = false;

    bool pauseMenuAvailable = false;
    int mainMenuIndex = -1;
    int firstLevelIndex = -1;
    int lastLevelIndex = -1;

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

        CreateMainMenu();
        CreatePauseMenu();

        EnableMainMenu();
        DisablePauseMenu();

		isFading = false;

	}

    private void Start()
    {
        Vector3 sceneIndices = em.BroadcastRequestSceneIndices();
        mainMenuIndex = (int)sceneIndices.x;
        firstLevelIndex = (int)sceneIndices.y;
        lastLevelIndex = (int)sceneIndices.z;
    }
    private void Update() {
        if (isFading) {
            Debug.Log("Fading");
            Color panelColor = blackPanel.color;
            panelColor.a -= fadeSpeed;
            Debug.Log("Alpha = " + panelColor.a);
            if (panelColor.a <= 0.0f) {
                isFading = false;
            }
            blackPanel.color = new Color(panelColor.r, panelColor.g, panelColor.b, panelColor.a);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        em.OnPauseStateChange += OnPauseStateChange;
		em.OnRequestLoadLevel += OnRequestLoadLevel;

	}

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        em.OnPauseStateChange -= OnPauseStateChange;
		em.OnRequestLoadLevel -= OnRequestLoadLevel;
	}

    private void CreateMainMenu()
    {
        GameObject newStartButton = Instantiate(Resources.Load("UI/MenuButton_Base") as GameObject, mainMenuHolder);
        startButton = newStartButton.GetComponent<Button>();
        startButton.GetComponentInChildren<Text>().text = "play";
        startButton.onClick.AddListener(OnStartButtonPressed);

        GameObject newQuitButton = Instantiate(Resources.Load("UI/MenuButton_Base") as GameObject, mainMenuHolder);
        quitButton = newQuitButton.GetComponent<Button>();
        quitButton.GetComponentInChildren<Text>().text = "quit";
        quitButton.onClick.AddListener(OnQuitButtonPressed);
    }

    private void CreatePauseMenu()
    {
        GameObject newResumeButton = Instantiate(Resources.Load("UI/MenuButton_Base") as GameObject, pauseMenuHolder);
        resumeButton = newResumeButton.GetComponent<Button>();
        resumeButton.GetComponentInChildren<Text>().text = "resume";
        resumeButton.onClick.AddListener(OnResumeButtonPressed);

        GameObject newRestartButton = Instantiate(Resources.Load("UI/MenuButton_Base") as GameObject, pauseMenuHolder);
        restartButton = newRestartButton.GetComponent<Button>();
        restartButton.GetComponentInChildren<Text>().text = "restart";
        restartButton.onClick.AddListener(OnRestartButtonPressed);

        GameObject newExitGameButton = Instantiate(Resources.Load("UI/MenuButton_Base") as GameObject, pauseMenuHolder);
        exitGameButton = newExitGameButton.GetComponent<Button>();
        exitGameButton.GetComponentInChildren<Text>().text = "exit";
        exitGameButton.onClick.AddListener(OnExitGameButtonPressed);
    }

    private void OnStartButtonPressed()
    {
        //Start the game (load first level, close main menu)
        //em.BroadcastRequestLoadLevel(firstLevelIndex);
        em.BroadcastRequestLoadLevel(firstLevelIndex);
    }

    private void OnQuitButtonPressed()
    {
        //Stop everything, close application
        Debug.Log("Quit button pressed");
        em.BroadcastRequestExitApplication();
    }

    private void OnResumeButtonPressed()
    {
        em.BroadcastRequestPauseStateChange(false);
    }

    private void OnRestartButtonPressed()
    {
        //TODO: Restart the level (or the whole game?)
        Debug.Log("Restart button pressed (not yet implemented)");
        int currentSceneIndex = em.BroadcastRequestCurrentSceneIndex();
        em.BroadcastRequestLoadLevel(currentSceneIndex);
    }

    private void OnExitGameButtonPressed()
    {
        Debug.Log("OnExitGameButtonPressed");
        em.BroadcastRequestLoadLevel(mainMenuIndex);
    }

    private void EnableMainMenu()
    {
        mainMenuHolder.gameObject.SetActive(true);
    }

    private void DisableMainMenu()
    {
        mainMenuHolder.gameObject.SetActive(false);
    }

    private void EnablePauseMenu()
    {
        pauseMenuHolder.gameObject.SetActive(true);
    }

    private void DisablePauseMenu()
    {
        pauseMenuHolder.gameObject.SetActive(false);
    }

    void OnPauseStateChange(bool newPauseState)
    {
        if (pauseMenuAvailable)
        {
            if (newPauseState)
            {
                EnablePauseMenu();
            }
            else
            {
                DisablePauseMenu();
            }
        }
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        isFading = true;
        if (mainMenuIndex == -1 || firstLevelIndex == -1 || lastLevelIndex == -1)
        {
            Vector3 sceneIndices = em.BroadcastRequestSceneIndices();
            mainMenuIndex = (int)sceneIndices.x;
            firstLevelIndex = (int)sceneIndices.y;
            lastLevelIndex = (int)sceneIndices.z;
        }

        if (scene.buildIndex == mainMenuIndex)
        {
            DisablePauseMenu();
            EnableMainMenu();
            pauseMenuAvailable = false;
        }
        if (scene.buildIndex >= firstLevelIndex/* && scene.buildIndex <= lastLevelIndex*/)
        {
            DisableMainMenu();
            DisablePauseMenu();
            pauseMenuAvailable = true;
        }
    }

    private void OnRequestLoadLevel(int desiredSceneBuildIndex) {
		Color panelColor = blackPanel.color;
		panelColor.a = 1.0f;
		blackPanel.color = new Color(panelColor.r, panelColor.g, panelColor.b, panelColor.a);
	}
}
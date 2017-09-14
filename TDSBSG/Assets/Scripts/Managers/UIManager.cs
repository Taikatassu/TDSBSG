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

    //GameOver screen
    public GameObject gameOverScreen;

    //GameCompleted screen
    public GameObject gameCompletedScreen;

    // BlackPanel
    [SerializeField]
    Image blackPanel;
    [SerializeField]
    //float fadeSpeed = 0.05f;
    bool isFading = false;
    [SerializeField]
    float fadeTime = 0.5f;
    float timeStartedLerping = 0f;
    float gameCompletedScreenDuration = 5f;
    float gameCompletedScreenTimer = 0f;

    bool gameCompletedScreenVisible = false;
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

        if(gameCompletedScreen != null)
        {
            gameCompletedScreen.SetActive(false);
        }

    }

    private void Start()
    {
        Vector3 sceneIndices = em.BroadcastRequestSceneIndices();
        mainMenuIndex = (int)sceneIndices.x;
        firstLevelIndex = (int)sceneIndices.y;
        lastLevelIndex = (int)sceneIndices.z;
    }

    private void FixedUpdate()
    {
        if (isFading)
        {
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageCompleted = timeSinceStarted / fadeTime;

            if(gameOverScreen != null)
            {
                Color gameOverColor = gameOverScreen.GetComponent<Image>().color;
                gameOverColor.a = Mathf.Lerp(1, 0, percentageCompleted);
                gameOverScreen.GetComponent<Image>().color = gameOverColor;
            }

            Color panelColor = blackPanel.color;
            panelColor.a = Mathf.Lerp(1, 0, percentageCompleted);
            blackPanel.color = panelColor;


            if (percentageCompleted >= 1)
            {
                isFading = false;

                if (gameOverScreen != null)
                {
                    gameOverScreen.SetActive(false);
                }
            }
        }

        if (gameCompletedScreenVisible)
        {
            gameCompletedScreenTimer -= Time.fixedDeltaTime;

            if(gameCompletedScreenTimer <= 0)
            {
                gameCompletedScreen.SetActive(false);
                gameCompletedScreenVisible = false;
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        em.OnPauseStateChange += OnPauseStateChange;
        em.OnRequestLoadLevel += OnRequestLoadLevel;
        em.OnPlayerCatched += OnPlayerCatched;
        em.OnLevelCompleted += OnLevelCompleted;

        blackPanel.gameObject.SetActive(true);

        if (gameOverScreen != null)
        {
            Color startColor = gameOverScreen.GetComponent<Image>().color;
            startColor.a = 1.0f;
            gameOverScreen.GetComponent<Image>().color = startColor;
            gameOverScreen.SetActive(false);
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        em.OnPauseStateChange -= OnPauseStateChange;
        em.OnRequestLoadLevel -= OnRequestLoadLevel;
        em.OnPlayerCatched -= OnPlayerCatched;
        em.OnLevelCompleted -= OnLevelCompleted;
    }

    private void OnLevelCompleted(int sceneIndex, ERobotType finishedRobotType)
    {
        if(sceneIndex == lastLevelIndex)
        {
            if (gameCompletedScreen != null)
            {
                gameCompletedScreen.SetActive(true);
                gameCompletedScreenTimer = gameCompletedScreenDuration;
                gameCompletedScreenVisible = true;
            }
        }
    }

    private void OnPlayerCatched()
    {
        if (gameOverScreen != null)
        {
            Color startColor = gameOverScreen.GetComponent<Image>().color;
            startColor.a = 1.0f;
            gameOverScreen.GetComponent<Image>().color = startColor;
            gameOverScreen.SetActive(true);
        }
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
        em.BroadcastRequestExitApplication();
    }

    private void OnResumeButtonPressed()
    {
        em.BroadcastRequestPauseStateChange(false);
    }

    private void OnRestartButtonPressed()
    {
        //TODO: Restart the level (or the whole game?)
        int currentSceneIndex = em.BroadcastRequestCurrentSceneIndex();
        em.BroadcastRequestLoadLevel(currentSceneIndex);
    }

    private void OnExitGameButtonPressed()
    {
        em.BroadcastRequestLoadLevel(mainMenuIndex);
    }

    private void EnableMainMenu()
    {
        mainMenuHolder.parent.gameObject.SetActive(true);
    }

    private void DisableMainMenu()
    {
        mainMenuHolder.parent.gameObject.SetActive(false);
    }

    private void EnablePauseMenu()
    {
        pauseMenuHolder.parent.gameObject.SetActive(true);
    }

    private void DisablePauseMenu()
    {
        pauseMenuHolder.parent.gameObject.SetActive(false);
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

        timeStartedLerping = Time.time;
        isFading = true;
    }

    private void OnRequestLoadLevel(int desiredSceneBuildIndex)
    {
        Color panelColor = blackPanel.color;
        panelColor.a = 1.0f;
        blackPanel.color = panelColor;

    }
}
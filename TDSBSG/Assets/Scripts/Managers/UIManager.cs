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
    GameObject mainMenu;
    public Transform mainMenuButtonHolder;
    public Transform mainMenuVolumeSliderHolder;
    Button startButton;
    Button quitButton;
    Button creditsButton;
    Slider mainMenuVolumeSlider;

    // PauseMenu
    GameObject pauseMenu;
    public Transform pauseMenuButtonHolder;
    public Transform pauseMenuVolumeSliderHolder;
    Button resumeButton;
    Button restartButton;
    Button exitGameButton;
    Slider pauseMenuVolumeSlider;

    //GameOver screen
    public GameObject gameOverScreen;

    //GameCompleted screen
    public GameObject gameCompletedScreen;

    //Credits screen
    public GameObject creditsScreen;
    public Transform closeCreditsScreenButtonHolder;
    Button closeCreditsScreenButton;

    // BlackPanel
    [SerializeField]
    Image blackPanel;
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

    float volumeSliderValue = 0f;

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
    }

    private void Start()
    {
        CreateMainMenu();
        CreatePauseMenu();

        EnableMainMenu();
        DisablePauseMenu();

        isFading = false;
    }

    private void Update()
    {
        if (mainMenu.activeSelf == true)
        {
            if (mainMenuVolumeSlider.value != volumeSliderValue)
            {
                volumeSliderValue = mainMenuVolumeSlider.value;
                em.BroadcastVolumeSliderValueChange(volumeSliderValue);
            }
        }

        if (pauseMenu.activeSelf == true)
        {
            if (pauseMenuVolumeSlider.value != volumeSliderValue)
            {
                volumeSliderValue = pauseMenuVolumeSlider.value;
                em.BroadcastVolumeSliderValueChange(volumeSliderValue);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isFading)
        {
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageCompleted = timeSinceStarted / fadeTime;
            
            Color panelColor = blackPanel.color;
            panelColor.a = Mathf.Lerp(1, 0, percentageCompleted);
            blackPanel.color = panelColor;

            if (percentageCompleted >= 1)
            {
                isFading = false;
            }
        }

        if (gameCompletedScreenVisible)
        {
            gameCompletedScreenTimer -= Time.fixedDeltaTime;

            if (gameCompletedScreenTimer <= 0)
            {
                gameCompletedScreen.SetActive(false);
                gameCompletedScreenVisible = false;
            }
        }

    }

    private void OnEnable()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();

        Vector3 sceneIndices = em.BroadcastRequestSceneIndices();
        mainMenuIndex = (int)sceneIndices.x;
        firstLevelIndex = (int)sceneIndices.y;
        lastLevelIndex = (int)sceneIndices.z;

        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        em.OnPauseStateChange += OnPauseStateChange;
        em.OnRequestLoadLevel += OnRequestLoadLevel;
        em.OnPlayerCatched += OnPlayerCatched;
        em.OnLevelCompleted += OnLevelCompleted;

        blackPanel.gameObject.SetActive(true);

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }

        if (gameCompletedScreen != null)
        {
            gameCompletedScreen.SetActive(false);
        }

        if (creditsScreen != null)
        {
            creditsScreen.SetActive(false);
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

    private void OnInputEvent(EInputType newInput)
    {
        if (newInput == EInputType.PAUSE_KEYDOWN)
        {
            CloseCreditsScreen();
        }
    }

    private void OnLevelCompleted(int sceneIndex, ERobotType finishedRobotType)
    {
        if (sceneIndex == lastLevelIndex)
        {
            if (gameCompletedScreen != null)
            {
                gameCompletedScreen.SetActive(true);
                gameCompletedScreenTimer = gameCompletedScreenDuration;
                gameCompletedScreenVisible = true;
                OpenCreditsScreen();
            }
        }
    }

    private void OnPlayerCatched()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }
    }

    private void CreateMainMenu()
    {
        mainMenu = mainMenuButtonHolder.parent.gameObject;

        GameObject newStartButton = Instantiate(Resources.Load("UI/MenuButton_Base") as GameObject, mainMenuButtonHolder);
        startButton = newStartButton.GetComponent<Button>();
        startButton.GetComponentInChildren<Text>().text = "play";
        startButton.onClick.AddListener(OnStartButtonPressed);

        GameObject newQuitButton = Instantiate(Resources.Load("UI/MenuButton_Base") as GameObject, mainMenuButtonHolder);
        quitButton = newQuitButton.GetComponent<Button>();
        quitButton.GetComponentInChildren<Text>().text = "quit";
        quitButton.onClick.AddListener(OnQuitButtonPressed);

        GameObject newCreditsButton = Instantiate(Resources.Load("UI/MenuButton_Base") as GameObject, mainMenuButtonHolder);
        creditsButton = newCreditsButton.GetComponent<Button>();
        creditsButton.GetComponentInChildren<Text>().text = "credits";
        creditsButton.onClick.AddListener(OnCreditsButtonPressed);

        GameObject newCloseCreditsScreenButton = Instantiate(Resources.Load("UI/MenuButton_Base") as GameObject, closeCreditsScreenButtonHolder);
        closeCreditsScreenButton = newCloseCreditsScreenButton.GetComponent<Button>();
        closeCreditsScreenButton.GetComponentInChildren<Text>().text = "close";
        closeCreditsScreenButton.onClick.AddListener(CloseCreditsScreen);

        GameObject newMainMenuVolumeSlider = Instantiate(Resources.Load("UI/MenuSlider") as GameObject, mainMenuVolumeSliderHolder);
        mainMenuVolumeSlider = newMainMenuVolumeSlider.GetComponent<Slider>();
        mainMenuVolumeSlider.GetComponentInChildren<Text>().text = "volume";
        volumeSliderValue = em.BroadcastRequestVolumeLevel();
        mainMenuVolumeSlider.value = volumeSliderValue;
    }

    private void CreatePauseMenu()
    {
        pauseMenu = pauseMenuButtonHolder.parent.gameObject;

        GameObject newResumeButton = Instantiate(Resources.Load("UI/MenuButton_Base") as GameObject, pauseMenuButtonHolder);
        resumeButton = newResumeButton.GetComponent<Button>();
        resumeButton.GetComponentInChildren<Text>().text = "resume";
        resumeButton.onClick.AddListener(OnResumeButtonPressed);

        GameObject newRestartButton = Instantiate(Resources.Load("UI/MenuButton_Base") as GameObject, pauseMenuButtonHolder);
        restartButton = newRestartButton.GetComponent<Button>();
        restartButton.GetComponentInChildren<Text>().text = "restart";
        restartButton.onClick.AddListener(OnRestartButtonPressed);

        GameObject newExitGameButton = Instantiate(Resources.Load("UI/MenuButton_Base") as GameObject, pauseMenuButtonHolder);
        exitGameButton = newExitGameButton.GetComponent<Button>();
        exitGameButton.GetComponentInChildren<Text>().text = "exit";
        exitGameButton.onClick.AddListener(OnExitGameButtonPressed);

        GameObject newPauseMenuVolumeSlider = Instantiate(Resources.Load("UI/MenuSlider") as GameObject, pauseMenuVolumeSliderHolder);
        pauseMenuVolumeSlider = newPauseMenuVolumeSlider.GetComponent<Slider>();
        pauseMenuVolumeSlider.GetComponentInChildren<Text>().text = "volume";
        volumeSliderValue = em.BroadcastRequestVolumeLevel();
        pauseMenuVolumeSlider.value = volumeSliderValue;
    }

    private void OnStartButtonPressed()
    {
        em.BroadcastRequestLoadLevel(firstLevelIndex);
    }

    private void OnQuitButtonPressed()
    {
        em.BroadcastRequestExitApplication();
    }

    private void OnCreditsButtonPressed()
    {
        if (creditsScreen != null)
        {
            if (creditsScreen.activeSelf == true)
            {
                creditsScreen.SetActive(false);
            }
            else
            {
                creditsScreen.SetActive(true);
            }

        }
    }

    private void OnResumeButtonPressed()
    {
        em.BroadcastRequestPauseStateChange(false);
    }

    private void OnRestartButtonPressed()
    {
        int currentSceneIndex = em.BroadcastRequestCurrentSceneIndex();
        em.BroadcastRequestLoadLevel(currentSceneIndex);
    }

    private void OpenCreditsScreen()
    {
        if (creditsScreen != null)
        {
            creditsScreen.SetActive(true);
        }
    }

    private void CloseCreditsScreen()
    {
        if (creditsScreen != null)
        {
            creditsScreen.SetActive(false);
        }
    }

    private void OnExitGameButtonPressed()
    {
        em.BroadcastRequestLoadLevel(mainMenuIndex);
    }

    private void EnableMainMenu()
    {
        mainMenu.SetActive(true);
        mainMenuVolumeSlider.value = em.BroadcastRequestVolumeLevel();
    }

    private void DisableMainMenu()
    {
        mainMenu.SetActive(false);
    }

    private void EnablePauseMenu()
    {
        pauseMenu.SetActive(true);
        pauseMenuVolumeSlider.value = em.BroadcastRequestVolumeLevel();
    }

    private void DisablePauseMenu()
    {
        pauseMenu.SetActive(false);
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
            em.OnInputEvent += OnInputEvent;
        }
        if (scene.buildIndex >= firstLevelIndex/* && scene.buildIndex <= lastLevelIndex*/)
        {
            DisableMainMenu();
            DisablePauseMenu();
            pauseMenuAvailable = true;
            em.OnInputEvent -= OnInputEvent;
        }

        timeStartedLerping = Time.time;
        isFading = true;
    }

    private void OnRequestLoadLevel(int desiredSceneBuildIndex)
    {
        Color panelColor = blackPanel.color;
        panelColor.a = 1.0f;
        blackPanel.color = panelColor;

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }

    }
}
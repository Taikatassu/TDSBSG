using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationManager : MonoBehaviour
{

    #region References & variables
    public static ApplicationManager _instance;
    Toolbox toolbox;
    EventManager em;

    int currentSceneIndex = 0;
    int introSceneIndex = 0;
    [SerializeField]
    int mainMenuIndex = 2; //TODO: Update this index if neccessary!!
    [SerializeField]
    int firstLevelIndex = 3; //TODO: Update this index if neccessary!!
    [SerializeField]
    int lastLevelIndex = 4; //TODO: Update this index if neccessary!!
    int levelToLoadAfterLoadingScreen = 0;
    [SerializeField]
    GameObject loadingScreenBackground;
    [SerializeField]
    GameObject loadingScreenCleanerBot;
    [SerializeField]
    GameObject loadingScreenMiniCleaner;
    GameObject loadingScreenHolder;
    #endregion

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
        loadingScreenHolder = loadingScreenBackground.transform.parent.gameObject;
        SetLoadingScreenState(false);
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == introSceneIndex)
        {
            SceneManager.LoadScene(mainMenuIndex);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        em.OnRequestLoadLevel += OnRequestLoadLevel;
        em.OnRequestExitApplication += OnRequestExitApplication;
        em.OnRequestCurrentSceneIndex += OnRequestCurrentSceneIndex;
        em.OnRequestSceneIndices += OnRequestSceneIndices;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        em.OnRequestLoadLevel -= OnRequestLoadLevel;
        em.OnRequestExitApplication -= OnRequestExitApplication;
        em.OnRequestCurrentSceneIndex -= OnRequestCurrentSceneIndex;
        em.OnRequestSceneIndices -= OnRequestSceneIndices;
    }

    private void SetLoadingScreenState(bool newState)
    {
        if (newState)
        {
            ERobotType robotType = em.BroadcastRequestSpawningRobotType();
            switch (robotType)
            {
                case ERobotType.DEBUG:
                    loadingScreenCleanerBot.SetActive(false);
                    loadingScreenMiniCleaner.SetActive(false);
                    break;
                case ERobotType.NONE:
                    loadingScreenCleanerBot.SetActive(false);
                    loadingScreenMiniCleaner.SetActive(false);
                    break;
                case ERobotType.DEFAULT:
                    loadingScreenCleanerBot.SetActive(true);
                    loadingScreenMiniCleaner.SetActive(false);
                    break;
                case ERobotType.WORKER:
                    loadingScreenCleanerBot.SetActive(false);
                    loadingScreenMiniCleaner.SetActive(false);
                    break;
                case ERobotType.SMALL:
                    loadingScreenCleanerBot.SetActive(false);
                    loadingScreenMiniCleaner.SetActive(true);
                    break;
                default:
                    break;
            }

            loadingScreenHolder.SetActive(true);
            loadingScreenTimer = loadingScreenDuration;
            loading = true;
        }
        else
        {
            loadingScreenHolder.SetActive(false);
            loading = false;
        }
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex >= firstLevelIndex/* && scene.buildIndex <= lastLevelIndex*/)
        {

        }
    }

    void OnRequestLoadLevel(int sceneBuildIndex)
    {
        //if (sceneBuildIndex == currentSceneIndex) {
        //	return;
        //}
        em.BroadcastRequestPauseStateChange(false);

        if (currentSceneIndex == introSceneIndex && sceneBuildIndex == mainMenuIndex)
        {
            currentSceneIndex = sceneBuildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }
        else
        {
            levelToLoadAfterLoadingScreen = sceneBuildIndex;
            em.BroadcastPauseActorsStateChange(true);
            SetLoadingScreenState(true);
        }
    }

    Vector3 OnRequestSceneIndices()
    {
        return new Vector3(mainMenuIndex, firstLevelIndex, lastLevelIndex);
    }

    int OnRequestCurrentSceneIndex()
    {
        return currentSceneIndex;
    }

    void OnRequestExitApplication()
    {
        Application.Quit();
    }

    bool loading = false;
    float loadingScreenTimer = 0;
    float loadingScreenDuration = 2;
    private void FixedUpdate()
    {
        if (loading)
        {
            Debug.Log("LoadingScreenController: FixedUpdate, loading = true");
            loadingScreenTimer -= Time.fixedDeltaTime;

            if (loadingScreenTimer <= 0)
            {
                Debug.Log("LoadingScreenController: FixedUpdate, loadingScreenTimer <= 0, loading wanted scene");
                SetLoadingScreenState(false);

                currentSceneIndex = levelToLoadAfterLoadingScreen;
                SceneManager.LoadScene(currentSceneIndex);
            }
        }
    }
}
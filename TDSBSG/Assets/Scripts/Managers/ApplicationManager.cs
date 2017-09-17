﻿using System.Collections;
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
    bool levelLoaded = false;
    bool loadingScreenTimerFinished = false;
    bool loading = false;
    float loadingScreenTimer = 0;
    float loadingScreenDuration = 0;
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
    }

    private void Start()
    {
        loadingScreenHolder = loadingScreenBackground.transform.parent.gameObject;
        SetLoadingScreenState(false);

        if (SceneManager.GetActiveScene().buildIndex == introSceneIndex)
        {
            SceneManager.LoadScene(mainMenuIndex);
        }
    }

    private void OnEnable()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();

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
        if(loadingScreenHolder != null)
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

                em.BroadcastLoadingScreenStateChange(true);
                loadingScreenHolder.SetActive(true);
                loadingScreenTimerFinished = false;
                loadingScreenTimer = loadingScreenDuration;
                loading = true;
            }
            else
            {
                em.BroadcastLoadingScreenStateChange(false);
                loadingScreenHolder.SetActive(false);
                loading = false;
            }
        }
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        levelLoaded = true;
        if (loadingScreenTimerFinished)
        {
            SetLoadingScreenState(false);
        }
    }

    void OnRequestLoadLevel(int sceneBuildIndex)
    {
        levelLoaded = false;
        em.BroadcastRequestPauseStateChange(false);

        if (currentSceneIndex == introSceneIndex && sceneBuildIndex == mainMenuIndex)
        {
            currentSceneIndex = sceneBuildIndex;
            SceneManager.LoadSceneAsync(currentSceneIndex);
        }
        else
        {
            levelToLoadAfterLoadingScreen = sceneBuildIndex;
            em.BroadcastPauseActorsStateChange(true);
            SetLoadingScreenState(true);
            currentSceneIndex = levelToLoadAfterLoadingScreen;
            SceneManager.LoadSceneAsync(currentSceneIndex);
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

    private void FixedUpdate()
    {
        if (loading)
        {
            loadingScreenTimer -= Time.fixedDeltaTime;

            if (loadingScreenTimer <= 0)
            {
                loadingScreenTimerFinished = true;

                if (levelLoaded)
                {
                    SetLoadingScreenState(false);
                }
            }
        }
    }
}
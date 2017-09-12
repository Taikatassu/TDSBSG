using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    Toolbox toolbox;
    EventManager em;
    bool isPaused = false;
    bool pausingAvailable = false;
    ERobotType robotTypeToSpawnPlayerAs = ERobotType.DEFAULT;
    int mainMenuIndex = 1; //TODO: Update this index if neccessary!!
    int firstLevelIndex = 2; //TODO: Update this index if neccessary!!
    int lastLevelIndex = 2; //TODO: Update this index if neccessary!!
    [SerializeField]
    bool loopLevels = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();
    }

    private void OnEnable()
    {
        em.OnInputEvent += OnInputEvent;
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        em.OnRequestPauseStateChange += OnRequestPauseStateChange;
        em.OnLevelCompleted += OnLevelCompleted;
    }

    private void OnDisable()
    {
        em.OnInputEvent -= OnInputEvent;
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        em.OnRequestPauseStateChange -= OnRequestPauseStateChange;
        em.OnLevelCompleted -= OnLevelCompleted;
    }

    //void Start()
    //{
    //    em.BroadcastInitializeGame();
    //    em.BroadcastStartGame();
    //}

    private void PauseGame()
    {
        Time.timeScale = 0.0f;
        isPaused = true;
        em.BroadcastPauseStateChange(true);
    }

    private void ResumeGame()
    {
        Time.timeScale = 1.0f;
        isPaused = false;
        em.BroadcastPauseStateChange(false);
    }

    private void TogglePauseGame()
    {
        if(pausingAvailable)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void OnInputEvent(EInputType newInput)
    {
        if (newInput == EInputType.PAUSE_KEYDOWN)
        {
            TogglePauseGame();
        }
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == mainMenuIndex)
        {
            pausingAvailable = false;
            ResumeGame();
        }
        if (scene.buildIndex >= firstLevelIndex/* && scene.buildIndex <= lastLevelIndex*/)
        {
            //TODO: Spawn player
            //em.BroadcastSpawnPlayer(robotTypeToSpawnPlayerWith)
            em.BroadcastSpawnPlayer(robotTypeToSpawnPlayerAs);
            em.BroadcastInitializeGame();
            em.BroadcastStartGame();
            pausingAvailable = true;
            ResumeGame();
        }
    }

    void OnRequestPauseStateChange(bool newState)
    {
        if(newState != isPaused)
        {
            if (newState)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }

    void OnLevelCompleted(ERobotType lastPossessedRobotType)
    {
        robotTypeToSpawnPlayerAs = lastPossessedRobotType;
        Debug.Log("GameManager: OnLevelCompleted");
        LoadNextLevel();
    }

    void LoadNextLevel()
    {
        int currentSceneIndex = em.BroadcastRequestCurrentSceneIndex();
        Debug.Log("LoadNextLevel, currentSceneIndex: " + currentSceneIndex + ", loopLevels: " + loopLevels);
        if (currentSceneIndex != -1)
        {
            if (currentSceneIndex < lastLevelIndex)
            {
                Debug.Log("currentSceneIndex < lastLevelIndex, loading the next level");
                currentSceneIndex++;
                em.BroadcastRequestLoadLevel(currentSceneIndex);
            }
            else if (loopLevels)
            {
                Debug.Log("loopLevels = true, loading first level");
                currentSceneIndex = firstLevelIndex;
                em.BroadcastRequestLoadLevel(currentSceneIndex);
            }
            else
            {
                //TODO: Implement proper game completed screen
                Debug.Log("Game completed! Loading main menu");
                em.BroadcastRequestLoadLevel(mainMenuIndex);
            }
        }
        else
        {
            Debug.Log("Scene index not found!");
        }

    }
}

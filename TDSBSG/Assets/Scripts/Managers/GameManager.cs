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
    }

    private void OnDisable()
    {
        em.OnInputEvent -= OnInputEvent;
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        em.OnRequestPauseStateChange -= OnRequestPauseStateChange;
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
            
        }
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
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
        if (scene.name == "MainMenu")
        {
            pausingAvailable = false;
            ResumeGame();
        }
        if (scene.name == "Level_ShotaTest")
        {
            em.BroadcastInitializeGame();
            em.BroadcastStartGame();
            pausingAvailable = true;
            ResumeGame();
        }
        if (scene.name == "Level_JuhoTest")
        {
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
}

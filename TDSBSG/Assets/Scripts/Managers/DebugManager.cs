using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugManager : MonoBehaviour
{
    public static DebugManager _instance;
    //Toolbox toolbox;
    //EventManager em;

    [SerializeField]
    bool showDebugIndicators = false;
    bool lastState = false;
    bool first = true;

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

        //toolbox = FindObjectOfType<Toolbox>();
        //em = toolbox.GetComponent<EventManager>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        GameObject[] debugIndicators = GameObject.FindGameObjectsWithTag("DebugIndicator");

        for (int i = 0; i < debugIndicators.Length; i++)
        {
            debugIndicators[i].GetComponent<MeshRenderer>().enabled = showDebugIndicators;
        }
    }

    void Update()
    {
        if (showDebugIndicators != lastState || first)
        {
            first = false;
            GameObject[] debugIndicators = GameObject.FindGameObjectsWithTag("DebugIndicator");

            for (int i = 0; i < debugIndicators.Length; i++)
            {
                debugIndicators[i].GetComponent<MeshRenderer>().enabled = showDebugIndicators;
            }
        }

        lastState = showDebugIndicators;
    }


}

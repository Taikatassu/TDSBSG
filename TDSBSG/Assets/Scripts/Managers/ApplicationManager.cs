using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationManager : MonoBehaviour {

	#region References & variables
	public static ApplicationManager _instance;
	Toolbox toolbox;
	EventManager em;

	int currentSceneIndex = 0;
    int introSceneIndex = 0;
    int mainMenuIndex = 1; //TODO: Update this index if neccessary!!
    int firstLevelIndex = 2; //TODO: Update this index if neccessary!!
    int lastLevelIndex = 2; //TODO: Update this index if neccessary!!
    #endregion

    private void Awake() {
		if (_instance == null) {
			_instance = this;
		} else if (_instance != this) {
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);

		toolbox = FindObjectOfType<Toolbox>();
		em = toolbox.GetComponent<EventManager>();
	}

    private void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex == introSceneIndex)
        {
            SceneManager.LoadScene(mainMenuIndex);
        }
    }

    private void OnEnable() {
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
		em.OnRequestLoadLevel += OnRequestLoadLevel;
        em.OnRequestExitApplication += OnRequestExitApplication;
        em.OnRequestCurrentSceneIndex += OnRequestCurrentSceneIndex;
    }

	private void OnDisable() {
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
		em.OnRequestLoadLevel -= OnRequestLoadLevel;
        em.OnRequestExitApplication -= OnRequestExitApplication;
        em.OnRequestCurrentSceneIndex -= OnRequestCurrentSceneIndex;
    }

	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
		if (scene.buildIndex >= firstLevelIndex/* && scene.buildIndex <= lastLevelIndex*/) {

		}
	}

	void OnRequestLoadLevel(int sceneBuildIndex) {
		//if (sceneBuildIndex == currentSceneIndex) {
		//	return;
		//}
		currentSceneIndex = sceneBuildIndex;
		SceneManager.LoadScene(sceneBuildIndex);
	}

    int OnRequestCurrentSceneIndex()
    {
        return currentSceneIndex;
    }

    void OnRequestExitApplication()
    {
        Application.Quit();
    }
}
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
    [SerializeField]
    int mainMenuIndex = 1; //TODO: Update this index if neccessary!!
    [SerializeField]
    int firstLevelIndex = 2; //TODO: Update this index if neccessary!!
    [SerializeField]
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
        em.OnRequestSceneIndices += OnRequestSceneIndices;
    }

	private void OnDisable() {
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
		em.OnRequestLoadLevel -= OnRequestLoadLevel;
        em.OnRequestExitApplication -= OnRequestExitApplication;
        em.OnRequestCurrentSceneIndex -= OnRequestCurrentSceneIndex;
        em.OnRequestSceneIndices -= OnRequestSceneIndices;
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
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationManager : MonoBehaviour {

	#region References & variables
	public static ApplicationManager _instance;

	Toolbox toolbox;
	EventManager em;

	string currentSceneName = null;
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
        if(SceneManager.GetActiveScene().name == "Scene00")
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void OnEnable() {
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
		em.OnRequestLoadLevel += OnRequestLoadLevel;
        em.OnRequestExitApplication += OnRequestExitApplication;

    }

	private void OnDisable() {
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
		em.OnRequestLoadLevel -= OnRequestLoadLevel;
        em.OnRequestExitApplication -= OnRequestExitApplication;
    }

	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
		if (scene.name == "Level_ShotaTest") {

		}
		if (scene.name == "Level_JuhoTest") {

		}
	}

	void OnRequestLoadLevel(string nameOfLoadScene) {
		if (nameOfLoadScene == currentSceneName) {
			return;
		}
		currentSceneName = nameOfLoadScene;
		SceneManager.LoadScene(nameOfLoadScene);
	}

    void OnRequestExitApplication()
    {
        Application.Quit();
    }
}
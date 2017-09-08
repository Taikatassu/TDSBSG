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
		}
		else if (_instance != this) {
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);

		toolbox = FindObjectOfType<Toolbox>();
		em = toolbox.GetComponent<EventManager>();
	}

	void OnLevelFinishedLoading(scene)
}

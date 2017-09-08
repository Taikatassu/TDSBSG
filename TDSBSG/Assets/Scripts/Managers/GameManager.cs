using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager instance;
	Toolbox toolbox;
	EventManager em;

	private void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad(gameObject);

		toolbox = FindObjectOfType<Toolbox>();
		em = toolbox.GetComponent<EventManager>();
	}

	void Start() {
		em.BroadcastInitializeGame();
		em.BroadcastStartGame();
	}

	void PuaseGame() {
		Time.timeScale = 0.0f;
	}

	void ResumeGame() {
		Time.timeScale = 1.0f;
	}
}

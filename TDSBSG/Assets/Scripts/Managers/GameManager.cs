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

	private void OnEnable() {
		em.OnPauseStateChange += OnPauseStateChange;
		em.OnInputEvent += OnInputEvent;
	}

	private void OnDisable() {
		em.OnPauseStateChange -= OnPauseStateChange;
		em.OnInputEvent -= OnInputEvent;
	}

	void Start() {
		em.BroadcastInitializeGame();
		em.BroadcastStartGame();
	}

	void PauseGame() {
		Time.timeScale = 0.0f;
	}

	void ResumeGame() {
		Time.timeScale = 1.0f;
	}

	void OnPauseStateChange(bool newPauseState) {
		if (newPauseState) {
			PauseGame();
		}
		else {
			ResumeGame();
		}
	}

	private void OnInputEvent(EInputType newInput) {
		if (newInput == EInputType.PAUSE_KEYDOWN) {
			em.BroadcastPauseStateChange(true);
		}
	}
}

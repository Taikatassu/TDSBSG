using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {
	public static EventManager _instance;

	private void Awake() {
		if (_instance == null) {
			_instance = this;
		}
		else if (_instance != this) {
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad(gameObject);
	}

	public delegate void EmptyVoid();
	public delegate void InputVoid(EInputType inputType);

	public event EmptyVoid OnGameStarted;
	public void BroadcastGameStarted() {
		if (OnGameStarted != null) {
			OnGameStarted();
		}
	}

	public event InputVoid OnInputEvent;
	public void BroadcastInputEvent(EInputType newInput) {
		if (OnInputEvent != null) {
			OnInputEvent(newInput);
		}
	}

	public void BroadcastRoomEntered(int LevelOfSecurity, bool isAllowed) {
		if (isAllowed) {
			Debug.Log("called BroadcastRoomEnterd true");
		}
	}
}

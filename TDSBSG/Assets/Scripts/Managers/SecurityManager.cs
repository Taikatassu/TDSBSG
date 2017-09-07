using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityManager : MonoBehaviour {
	public static SecurityManager _instance;

	Toolbox toolbox;
	EventManager em;

	int securityPoints = 0; // total point of security
	int securityTier = 0; // now security tier
	[SerializeField]
	int numOfTiers = 5; // maximum of Tier ex) this number is 5, tier 0 from between 5
	[SerializeField]
	int pointPerTier = 100;
	[SerializeField, Range(0, 100)]
	int roomEnterIncrease = 20; // value of Increase when enterted room
	[SerializeField,Range(0, 100)]
	int doorEnterIncrease = 20; // value of Increase when enterted door

	int maximumOfSecurityPoint;

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

		maximumOfSecurityPoint = numOfTiers * pointPerTier;
	}

	private void OnEnable() {
		em.OnRoomEntered += OnRoomEntered;
		em.OnDoorEntered += OnDoorEntered;
	}

	private void OnDisable() {
		em.OnRoomEntered -= OnRoomEntered;
		em.OnDoorEntered -= OnDoorEntered;
	}

	private void OnRoomEntered(int roomSecurityLevel, bool isAllowed) {
		IncreaseSecurityPoints(roomSecurityLevel, isAllowed);
	}

	private void OnDoorEntered(int doorSecurityLevel, bool isAllowed) {
		IncreaseSecurityPoints(doorSecurityLevel, isAllowed);
	}

	private void IncreaseSecurityPoints(int securityLevel, bool isAllowed) {
		if (isAllowed) {
			return;
		}
		int valueOfIncrease = doorEnterIncrease * securityLevel;
		securityPoints += valueOfIncrease;
		if (securityPoints >= maximumOfSecurityPoint) {
			securityPoints = maximumOfSecurityPoint;
		}
		int previousSecurityTier = securityTier;
		securityTier = securityPoints / pointPerTier;
		if (securityTier > previousSecurityTier) {
			StartAlarm();
		}

		Debug.Log("Increment Security Point" + securityPoints);
	}

	private void StartAlarm() {
		em.BroadcastStartAlarm();
		Debug.Log("start alarm");
	}
}

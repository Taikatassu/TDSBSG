using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
	Toolbox toolbox;
	EventManager em;
	[SerializeField]
	private int levelOfSecurity = 0; // door's level of security
	[SerializeField, Header("List of allowed robot type")]
	List<ERobotType> listOfAllowedRobotType = new List<ERobotType>();

	private void Awake() {
		toolbox = FindObjectOfType<Toolbox>();
		em = toolbox.GetComponent<EventManager>();
	}

	// Get room's level of securityS
	public int GetLevelOfSecurity() { return levelOfSecurity; }

	void OnTriggerEnter(Collider other) {
		if (!other.GetComponent(typeof(IPossessable))) { return; }
		IPossessable iPossessable = other.GetComponent(typeof(IPossessable)) as IPossessable;
		if (iPossessable.GetIsPossessed()) {
			ERobotType typeOfPlayer = iPossessable.GetRobotType();
			bool isSameType = false;
			foreach (ERobotType i in listOfAllowedRobotType) {
				if (typeOfPlayer == i) {
					isSameType = true;
					break;
				}
			}
			em.BroadcastDoorEntered(levelOfSecurity, isSameType);
		}
	}
}

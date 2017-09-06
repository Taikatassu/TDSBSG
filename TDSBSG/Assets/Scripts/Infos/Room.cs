using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

	Toolbox toolbox;
	EventManager em;
	int levelOfSecurity = 0; // room's level of security
	bool haveEntered = false; // if entered room this flag is true
	[SerializeField,Header ("List of allowed robot type")]
	List<ERobotType> listOfAllowedRobotType = new List<ERobotType>();

	private void Awake() {
		haveEntered = false;

		toolbox = FindObjectOfType<Toolbox>();
		em = toolbox.GetComponent<EventManager>();
	}

	// Get room's level of securityS
	public int GetLevelOfSecurity() { return levelOfSecurity; }

	void OnTriggerEnter(Collider other) {
		if (haveEntered) { return; }
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
			em.BroadcastRoomEntered(levelOfSecurity, isSameType);
			
			haveEntered = true;
		}
	}
}

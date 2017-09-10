using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPossessable
{
    bool GetIsPossessed();

    bool GetIsDisobeying();

    void AddDisobeyingToList(GameObject go);

    void RemoveDisobeyingFromList(GameObject go);

    void AddToConnectedPossessablesList(IPossessable newConnection);

    List<IPossessable> GetConnectedPossessablesList();

    EPossessableType GetPossessableType();

	ERobotType GetRobotType();

	GameObject GetGameObject();

    void Possess();

    void UnPossess();

    void GiveInput(EInputType newInput);

}

public enum EPossessableType
{
    PRIMARY, //Primary host body of the player. Only one simultaneous possession
    SECONDARY, //For example a stationary turret. Multiple simultaneous possessions possible

}

public enum ERobotType {
    DEBUG,
    NONE, // Use when all types should be restricted
	DEFAULT,
	WORKER,
    SMALL,
}

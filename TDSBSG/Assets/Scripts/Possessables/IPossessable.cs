﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPossessable
{
    bool GetIsPossessed();

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
	CLEANING,
	RADIO,
}

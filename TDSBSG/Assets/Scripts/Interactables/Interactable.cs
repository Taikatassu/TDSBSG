﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

    private bool isInUse = false;
    protected bool stationaryInteractable;
    protected float startDurationTime;
    protected float endDurationTime;
    protected List<ERobotType> permissionList;


    public bool GetIsInUse() { return isInUse; }

    protected virtual float InteractionStartDuration()
    {
        return 0.0f;
    }

    protected virtual float InteractionEndDuration()
    {
        return 0.0f;
    }

    public virtual float StartInteraction()
    {
        if (isInUse) { return -1.0f; }

        isInUse = true;
        return startDurationTime;
    }

    public virtual float EndInteraction()
    {
        if (!isInUse) { return -1.0f; }
        isInUse = false;
        return endDurationTime;

    }

    public bool ContainPermissionList(ERobotType robotTypeOfPlayer)
    {
        foreach (ERobotType i in permissionList)
        {
            if(robotTypeOfPlayer == i)
            {
                return true;
            }
        }
        return false;
    }
}

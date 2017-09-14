using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

    protected bool isInUse = false;
    [SerializeField]
    protected bool stationaryInteractable = false;
    protected float startDurationTime = 0.0f;
    protected float endDurationTime = 0.0f;
    [SerializeField]
    protected List<ERobotType> permissionList;


    public bool GetIsStationaryInteractable()
    {
        return stationaryInteractable;
    }

    public bool GetIsInUse() { return isInUse; }

    protected virtual float InteractionStartDuration()
    {
        return 0.0f;
    }

    protected virtual float InteractionEndDuration()
    {
        return 0.0f;
    }

    public virtual float StartInteraction(IPossessable user)
    {
        if (isInUse) { return -1.0f; }
        isInUse = true;
        return startDurationTime;
    }

    public virtual float EndInteraction(IPossessable user)
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

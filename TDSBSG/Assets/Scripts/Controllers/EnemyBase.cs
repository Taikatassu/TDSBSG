using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    bool isAlerted = false;
    bool isTrackingTarget = false;
    readonly bool canPatrol = true;
    List<Transform> patrolPoints = new List<Transform>();

    public bool GetIsAlerted()
    {
        return isAlerted;
    }

    public bool GetIsTrackingTarget()
    {
        return isTrackingTarget;
    }

    public void SetPatrolPoints(List<Transform> newPatrolPoints)
    {

    }

    //TODO: Implement method for detecting targets (raycast?)

}

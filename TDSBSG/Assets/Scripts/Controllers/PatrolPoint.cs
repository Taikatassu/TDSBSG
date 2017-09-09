using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    [SerializeField]
    float guardingDuration = 0;
    [SerializeField]
    bool shouldLookAround = false;
    [SerializeField]
    float lookAroundAngle = 30;
    [SerializeField]
    float lookAroundDuration = 2;

    public float GetGuardingDuration()
    {
        return guardingDuration;
    }

    public bool GetShouldLookAround()
    {
        return shouldLookAround;
    }

    public float GetLookAroundAngle()
    {
        return Mathf.Abs(lookAroundAngle);
    }

    public float GetLookAroundDuration()
    {
        return lookAroundDuration;
    }

}

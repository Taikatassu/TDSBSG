﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{

    Toolbox toolbox;
    EventManager em;
    [SerializeField]
    private int levelOfSecurity = 0; // room's level of security
    [SerializeField, Header("List of allowed robot type")]
    List<ERobotType> listOfAllowedRobotType = new List<ERobotType>();

    private void Awake()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();
    }

    // Get room's level of securityS
    public int GetLevelOfSecurity() { return levelOfSecurity; }

    //void OnTriggerEnter(Collider other)
    //{
    //    if (!other.GetComponent(typeof(IPossessable))) { return; }
    //    IPossessable iPossessable = other.GetComponent(typeof(IPossessable)) as IPossessable;
    //    if (iPossessable.GetIsPossessed())
    //    {
    //        ERobotType typeOfPlayer = iPossessable.GetRobotType();
    //        bool isSameType = false;
    //        foreach (ERobotType i in listOfAllowedRobotType)
    //        {
    //            if (typeOfPlayer == i)
    //            {
    //                isSameType = true;
    //                break;
    //            }
    //        }

    //        iPossessable.AddDisobeyingToList(gameObject);
    //        em.BroadcastRoomEntered(levelOfSecurity, isSameType,
    //            other.GetComponent<IPossessable>().GetRobotType());
    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        //Ignore stationary possessables for now 
        if (other.GetComponent(typeof(Poss_Mobile)))
        {
            IPossessable iPossessable = other.GetComponent<IPossessable>();
            if (iPossessable.GetIsPossessed())
            {
                ERobotType robotType = iPossessable.GetRobotType();
                bool isSameType = false;
                foreach (ERobotType i in listOfAllowedRobotType)
                {
                    if (robotType == i)
                    {
                        isSameType = true;
                        break;
                    }
                }

                if (!isSameType)
                {
                    iPossessable.AddDisobeyingToList(gameObject);
                }

                em.BroadcastRoomEntered(levelOfSecurity, isSameType, robotType);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent(typeof(Poss_Mobile))) { return; }
        IPossessable iPossessable = other.GetComponent<IPossessable>();

        iPossessable.RemoveDisobeyingFromList(gameObject);
    }
}
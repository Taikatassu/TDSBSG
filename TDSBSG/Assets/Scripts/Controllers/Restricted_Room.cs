using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restricted_Room : MonoBehaviour
{
    [SerializeField, Header("List of allowed robot type")]
    List<ERobotType> listOfAllowedRobotType = new List<ERobotType>();
    
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restricted_Door : MonoBehaviour
{
    [SerializeField, Header("Allowed robot types")]
    List<ERobotType> listOfAllowedRobotType = new List<ERobotType>();

    void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent(typeof(IPossessable))) { return; }
        IPossessable iPossessable = other.GetComponent(typeof(IPossessable)) as IPossessable;
        if (iPossessable.GetIsPossessed())
        {
            ERobotType typeOfPlayer = iPossessable.GetRobotType();
            bool isSameType = false;
            foreach (ERobotType i in listOfAllowedRobotType)
            {
                if (typeOfPlayer == i)
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

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent(typeof(IPossessable))) { return; }
        IPossessable iPossessable = other.GetComponent<IPossessable>();

        iPossessable.RemoveDisobeyingFromList(gameObject);
    }
}

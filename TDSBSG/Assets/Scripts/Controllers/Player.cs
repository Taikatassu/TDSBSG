using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region References & variables
    Toolbox toolbox;
    EventManager em;
    PossessableInfo possInfo;
    IPossessable primaryPossession;
    List<IPossessable> secondaryPossessions = new List<IPossessable>();
    Transform primaryPossessionTransform;

    float possessionRange = 5;
    #endregion


    private void Awake()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();
        possInfo = toolbox.GetComponent<PossessableInfo>();
    }

    private void OnEnable()
    {
        em.OnGameStarted += OnGameStarted;
        em.OnInputEvent += OnInputEvent;
        em.OnRequestPlayerReference += OnRequestPlayerReference;
    }

    private void OnDisable()
    {
        em.OnGameStarted -= OnGameStarted;
        em.OnInputEvent -= OnInputEvent;
        em.OnRequestPlayerReference -= OnRequestPlayerReference;
    }

    private void OnGameStarted()
    {
        ResetAll();
        //PossessClosestPossessable();
    }

    private GameObject OnRequestPlayerReference()
    {
        return gameObject;
    }

    private bool PossessClosestPossessable()
    {
        if(possInfo.possessables.Count > 0)
        {
            IPossessable closestPossessable = possInfo.possessables[0];
            float distanceToClosest = -1;
            bool first = true;
            int count = possInfo.possessables.Count;
            for (int i = 0; i < count; i++)
            {
                if(possInfo.possessables[i] != primaryPossession)
                {
                    float distanceToThis = (transform.position -
                        possInfo.possessables[i].GetGameObject().transform.position).magnitude;
                    if (first || distanceToThis < distanceToClosest)
                    {
                        first = false;
                        closestPossessable = possInfo.possessables[i];

                        distanceToClosest = distanceToThis;

                    }
                }
            }

            //Debug.Log("ClosestPossessable found, distanceToClosest: " + distanceToClosest);

            if(distanceToClosest > possessionRange)
            {
                return false;
            }

            PossessPossessable(closestPossessable);
            return true;
        }
        Debug.LogWarning("No possessables found");
        return false;
    }

    private void FixedUpdate()
    {
        if(primaryPossession != null)
        {
            transform.position = primaryPossessionTransform.position;
        }
    }

    private void PossessPossessable(IPossessable newPossession)
    {
        switch (newPossession.GetPossessableType())
        {
            case EPossessableType.PRIMARY:
                if(primaryPossession != null)
                {
                    primaryPossession.UnPossess();
                }
                newPossession.Possess();
                primaryPossession = newPossession;
                primaryPossessionTransform = primaryPossession.GetGameObject().transform;
                break;
            case EPossessableType.SECONDARY:
                newPossession.Possess();
                secondaryPossessions.Add(newPossession);
                break;
            default:
                break;
        }
    }

    private void OnInputEvent(EInputType newInput)
    {
        if(newInput == EInputType.POSSESS_KEYDOWN)
        {
            PossessClosestPossessable();
        }

        else if(primaryPossession != null)
        {
            primaryPossession.GiveInput(newInput);
        }

        //if (currentSecondaryPossessions.Count > 0)
        //{
        //    for (int i = 0; i < currentSecondaryPossessions.Count; i++)
        //    {
        //        currentSecondaryPossessions[i].GiveInput(newInput);
        //    }
        //}

        //TODO: Handle player only inputs 
        //(the ones not specific to bot, like opening menu etc.)

    }

    private void ResetAll()
    {
        if (secondaryPossessions.Count > 0)
        {
            for (int i = 0; i < secondaryPossessions.Count; i++)
            {
                if (secondaryPossessions[i] != null)
                {
                    secondaryPossessions[i].UnPossess();
                }
            }
        }

        secondaryPossessions = new List<IPossessable>();
    }
}

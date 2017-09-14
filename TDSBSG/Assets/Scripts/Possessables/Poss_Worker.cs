﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poss_Worker : Poss_Mobile
{

    private Interactable_Liftable currentLiftable = null;
    [SerializeField]
    Transform lifter;
    [SerializeField]
    float heightOfLifter = 2.0f;

    [SerializeField]
    BoxCollider liftableDetector;
    [SerializeField]
    Vector3 triggerOriginalPos = new Vector3(0.0f, 0.6f, 1.6f);
    [SerializeField]
    Vector3 triggerOriginalScale = new Vector3(1.5f, 1.1f, 0.75f);
    [SerializeField]
    Vector3 triggerLiftPos = new Vector3(0.0f, 0.6f, 1.75f);
    [SerializeField]
    Vector3 triggerLiftScale = new Vector3(1.5f, 1.1f, 1.5f);

    [SerializeField]
    private float animationTime = 1.0f;
    Vector3 startPosOfLerp;
    Vector3 EndPosOfLerp;
    private bool isLerping = false;
    private float timeStartedLerping;
    bool isLerpUp = true;
    bool liftingObject = false;
    List<Interactable_Liftable> liftableCandidates = new List<Interactable_Liftable>();
    List<GameObject> overlappingObjects = new List<GameObject>();
    Vector3 lifterDownPos = Vector3.zero;
    Vector3 lifterUpPos = Vector3.zero;


    protected void ChangeColliderParameters(Vector3 pos, Vector3 scale)
    {
        liftableDetector.center = pos;
        liftableDetector.size = scale;
    }

    protected override void OnInitializeGame()
    {
        base.OnInitializeGame();

        lifter.GetComponent<Collider>().enabled = false;

        lifterDownPos = lifter.localPosition;
        lifterUpPos = lifterDownPos;
        lifterUpPos.y += heightOfLifter;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (isLerping)
        {
            LerpInteractableObject();
        }

        //if (!liftableCandidate) {
        //    liftableDetector.center = originalPos;
        //    liftableDetector.size = originalScale;
        //    return;
        //}

        //liftableDetector.center = liftPos;
        //liftableDetector.size = liftScale;

        // end liftdown
        //if (!isLerpUp) {
        //    if (!isLerping) {
        //        currentLiftable.EndInteraction(this);
        //        currentLiftable = null;
        //        isLerpUp = true;
        //    }
        //}
    }
    public override void GiveInput(EInputType newInput)
    {
        base.GiveInput(newInput);
        switch (newInput)
        {
            case EInputType.USE_KEYDOWN:
                Debug.Log("Worker.GiveInput: USE_KEYDOWN");
                if (liftingObject)
                {
                    Debug.Log("Worker.GiveInput: USE_KEYDOWN, liftingObject = true, overlappingObjects.Count: " + overlappingObjects.Count);
                    if (overlappingObjects.Count == 0)
                    {
                        StartLerp(false);
                    }
                }
                else
                {
                    //Debug.Log("Start Interaction Liftable");
                    Interactable_Liftable closestLiftable = GetClosestLiftable();
                    Debug.Log("Worker.GiveInput: USE_KEYDOWN, liftingObject = false, closestLiftable: " + closestLiftable);
                    if (closestLiftable != null)
                    {
                        float result = closestLiftable.StartInteraction(lifter);
                        if (result >= 0)
                        {
                            Debug.Log("Worker.GiveInput: USE_KEYDOWN, liftingObject = false, closestLiftable != null, result >= 0 (started interaction successfully)");

                            StartLerp(true);
                            currentLiftable = closestLiftable;
                            liftingObject = true;
                        }
                    }
                }
                break;
            case EInputType.USE_KEYUP:
                break;
            default:
                break;
        }
    }

    private Interactable_Liftable GetClosestLiftable()
    {
        Interactable_Liftable lc = null;
        bool first = true;
        for (int i = 0; i < liftableCandidates.Count; i++)
        {
            float distance = (liftableCandidates[i].transform.position
               - liftableDetector.transform.position).magnitude;
            float closestDistance = 0;

            if (first || distance < closestDistance)
            {
                closestDistance = distance;
                lc = liftableCandidates[i];
            }
        }

        return lc;
    }

    private void StartLerp(bool up)
    {
        isLerping = true;
        interactionPause = true;
        timeStartedLerping = Time.time;
        if (up)
        {
            startPosOfLerp = lifter.localPosition;
            EndPosOfLerp = lifterUpPos;
            isLerpUp = true;
        }
        else
        {
            startPosOfLerp = lifter.localPosition;
            EndPosOfLerp = lifterDownPos;
            isLerpUp = false;
        }
    }

    private void LerpInteractableObject()
    {
        float timeSinceStarted = Time.time - timeStartedLerping;
        float percentageComplete = timeSinceStarted / animationTime;

        lifter.localPosition = Vector3.Lerp(startPosOfLerp, EndPosOfLerp, percentageComplete);
        if (percentageComplete >= 1.0f)
        {
            isLerping = false;
            interactionPause = false;
            if (isLerpUp)
            {
                ChangeColliderParameters(triggerLiftPos, triggerLiftScale);
                liftableCandidates.Clear();
                lifter.GetComponent<Collider>().enabled = true;
            }
            else
            {
                ChangeColliderParameters(triggerOriginalPos, triggerOriginalScale);
                overlappingObjects.Clear();
                lifter.GetComponent<Collider>().enabled = false;
                currentLiftable.EndInteraction(this);
                liftingObject = false;
            }

        }
    }

    //private void OnTriggerEnter(Collider other) {
    //    if (liftableCandidate == null) {
    //        if (other.GetComponent<Interactable_Liftable>())
    //            Debug.Log("Collide liftableObject");
    //        liftableCandidate = other.GetComponent<Interactable_Liftable>();
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        //if (other != liftableCandidate) {
        //    Debug.Log("Exit liftableObject");
        //    liftableCandidate = null;
        //}

        if (liftingObject)
        {
            if (overlappingObjects.Contains(other.gameObject))
            {
                //Debug.Log("removing from overlapping list");
                overlappingObjects.Remove(other.gameObject);
            }
        }
        else
        {
            if (liftableCandidates.Contains(other.GetComponent<Interactable_Liftable>()))
            {
                //Debug.Log("removing from liftable list");
                liftableCandidates.Remove(other.GetComponent<Interactable_Liftable>());
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!liftingObject)
        {
            if (other.GetComponent<Interactable_Liftable>())
            {
                Interactable_Liftable hitLiftable = other.GetComponent<Interactable_Liftable>();

                if (!liftableCandidates.Contains(hitLiftable))
                {
                    //Debug.Log("adding to liftable list");
                    liftableCandidates.Add(hitLiftable);
                }
            }
        }
        else
        {
            GameObject overlapper = other.gameObject;

            if (!overlappingObjects.Contains(overlapper))
            {
                //Debug.Log("adding to overlapping list");
                overlappingObjects.Add(overlapper);
            }
        }
    }
}

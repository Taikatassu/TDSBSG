using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poss_Worker : Poss_Mobile {

    private Interactable_Liftable liftableCandidate = null;
    [SerializeField]
    Transform lifter;
    [SerializeField]
    float heightOfLifter = 2.0f;

    [SerializeField]
    Collider liftableDetector;

    [SerializeField]
    private float animationTime = 1.0f;
    Vector3 startPosOfLerp;
    Vector3 EndPosOfLerp;
    private bool isLerping = false;
    private float timeStartedLerping;
    bool isLerpUp = true;

    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (isLerping) {
            LerpInteractableObject();
        }

        if(!liftableCandidate) {
            return;
        }
        if (!isLerpUp) {
            if (!isLerping) {
                liftableCandidate.EndInteraction(this);
                liftableCandidate = null;
                isLerpUp = true;
            }
        }
    }
    public override void GiveInput(EInputType newInput) {
        base.GiveInput(newInput);
        switch (newInput) {
            case EInputType.MOVEUP_KEYDOWN:
            break;
            case EInputType.MOVEDOWN_KEYDOWN:
            break;
            case EInputType.MOVERIGHT_KEYDOWN:
            break;
            case EInputType.MOVELEFT_KEYDOWN:
            break;
            case EInputType.POSSESS_KEYDOWN:
            break;
            case EInputType.MOVEUP_KEYUP:
            break;
            case EInputType.MOVEDOWN_KEYUP:
            break;
            case EInputType.MOVERIGHT_KEYUP:
            break;
            case EInputType.MOVELEFT_KEYUP:
            break;
            case EInputType.POSSESS_KEYUP:
            break;
            case EInputType.PAUSE_KEYDOWN:
            break;
            case EInputType.PAUSE_KEYUP:
            break;
            case EInputType.CAMERAMODE_KEYDOWN:
            break;
            case EInputType.CAMRAMODE_KEYUP:
            break;
            case EInputType.USE_KEYDOWN:
            Debug.Log("press Space");
            Debug.Log(liftableCandidate);
            if (liftableCandidate) {
                if (liftableCandidate.GetIsInUse()) {
                    StartLerp(false);
                } else {
                    Debug.Log("Start Interaction Liftable");
                    StartLerp(true);
                    liftableCandidate.StartInteraction(lifter);
                }
            }
            break;
            case EInputType.USE_KEYUP:
            break;
            default:
            break;
        }
    }

    private void StartLerp(bool up) {
        isLerping = true;
        interactionPause = true;
        timeStartedLerping = Time.time;
        if (up) {
            startPosOfLerp = lifter.position;
            EndPosOfLerp = startPosOfLerp;
            EndPosOfLerp.y += +heightOfLifter;
            isLerpUp = true;
        } else {
            startPosOfLerp = lifter.position;
            EndPosOfLerp = startPosOfLerp;
            EndPosOfLerp.y -= heightOfLifter;
            isLerpUp = false;
        }
    }

    private void LerpInteractableObject() {
        float timeSinceStarted = Time.time - timeStartedLerping;
        float percentageComplete = timeSinceStarted / animationTime;

        lifter.position = Vector3.Lerp(startPosOfLerp, EndPosOfLerp, percentageComplete);
        if (percentageComplete >= 1.0f) {
            isLerping = false;
            interactionPause = false;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (liftableCandidate == null) {
            if (other.GetComponent<Interactable_Liftable>())
                Debug.Log("collide liftableObject");
                liftableCandidate = other.GetComponent<Interactable_Liftable>();
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other == liftableCandidate) {
            liftableCandidate = null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poss_Worker : Poss_Mobile
{

    private Interactable_Liftable currentLiftable = null;
    [SerializeField]
    Transform lifter;
    [SerializeField]
    Transform lifterAttachMarker;
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
    bool liftingStationaryObject = false;
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
        rb.velocity = Vector3.zero;

        if (isLerping)
        {
            LerpInteractableObject();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentMovementSpeedMultiplier = 3;
        }
        else
        {
            currentMovementSpeedMultiplier = 1.5f;
        }

        if (disobeyingList.Count > 0)
        {
            isDisobeying = true;
        }
        else
        {
            isDisobeying = false;
        }
        if (!interactionPause)
        {
            if (canMove && !liftingStationaryObject)
            {
                if (isPossessed)
                {
                    //Movement by player
                    float moveZValue = 0;
                    float moveXValue = 0;

                    if (movingUp)
                    {
                        moveZValue++;
                    }
                    if (movingDown)
                    {
                        moveZValue--;
                    }

                    if (movingRight)
                    {
                        moveXValue++;
                    }
                    if (movingLeft)
                    {
                        moveXValue--;
                    }

                    Vector3 movementVelocity;
                    movementVelocity = (cameraRotatorTransform.forward * moveZValue
                        + cameraRotatorTransform.right * moveXValue).normalized;

                    movementVelocity *= defaultMovementSpeed * currentMovementSpeedMultiplier
                        * Time.fixedDeltaTime;

                    rb.velocity = movementVelocity;
                    if (rb.velocity != Vector3.zero)
                    {
                        lookDirection = Quaternion.LookRotation(rb.velocity, Vector3.up).eulerAngles;
                    }
                }
            }
        }

        if (rb.velocity != Vector3.zero)
        {
            if (spriteController)
            {
                spriteController.SetAnimationState(EAnimationState.WALK);
            }
        }
        else
        {
            if (spriteController)
            {
                spriteController.SetAnimationState(EAnimationState.IDLE);
            }
        }

        transform.rotation = Quaternion.Euler(lookDirection);
    }

    public override void GiveInput(EInputType newInput)
    {
        base.GiveInput(newInput);
        switch (newInput)
        {
            case EInputType.USE_KEYDOWN:
                if (liftingObject)
                {
                    if (overlappingObjects.Count == 0)
                    {
                        if (liftingStationaryObject)
                        {
                            liftingStationaryObject = false;
                        }

                        StartLerp(false);
                    }
                }
                else
                {
                    Interactable_Liftable closestLiftable = GetClosestLiftable();
                    if (closestLiftable != null)
                    {
                        float result = closestLiftable.StartInteraction(lifterAttachMarker);
                        if (result >= 0)
                        {
                            if (closestLiftable.GetIsStationaryInteractable())
                            {
                                liftingStationaryObject = true;
                            }
                            else
                            {
                                Vector2 liftableMeasures = closestLiftable.GetMeasures();
                                lifterAttachMarker.localPosition =
                                    new Vector3(0, -0.4f, liftableMeasures.x / 2 + 0.4f);
                            }

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

    private void OnTriggerExit(Collider other)
    {
        if (liftingObject)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
            {
                if (overlappingObjects.Contains(other.gameObject))
                {
                    overlappingObjects.Remove(other.gameObject);
                }
            }
        }
        else
        {
            if (liftableCandidates.Contains(other.GetComponent<Interactable_Liftable>()))
            {
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
                    liftableCandidates.Add(hitLiftable);
                }
            }
        }
        else
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
            {
                GameObject overlapper = other.gameObject;

                if (!overlappingObjects.Contains(overlapper))
                {
                    overlappingObjects.Add(overlapper);
                }
            }
        }
    }
}

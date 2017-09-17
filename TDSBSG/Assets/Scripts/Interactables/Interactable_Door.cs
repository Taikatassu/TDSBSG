using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Door : Interactable
{
    [SerializeField]
    Collider interactionTrigger;
    [SerializeField]
    GameObject doorObject;
    [SerializeField]
    GameObject doorObject_2;
    [SerializeField]
    Vector3 openPos;
    [SerializeField]
    Vector3 openRot;
    [SerializeField]
    Vector3 openPos_2;
    [SerializeField]
    Vector3 openRot_2;

    Vector3 closedPos;
    Vector3 closedRot;
    Vector3 closedPos_2;
    Vector3 closedRot_2;
    Vector3 startPos;
    Vector3 startPos_2;
    Vector3 startRot;
    Vector3 startRot_2;
    [SerializeField]
    float animationDuration = 1.0f;
    [SerializeField]
    bool autoClose = true;
    float timeStartedLerping = 0f;
    EDoorState state;
    [SerializeField]
    Transform accessLightMarker;
    [SerializeField]
    Transform accessLightMarker2;

    ParticleSystem lightEffect;
    ParticleSystem lightEffect2;

    private enum EDoorState
    {
        IS_WAITING,
        IS_OPENING,
        IS_CLOSING
    }

    private void Start()
    {
        if (doorObject != null)
        {
            closedPos = doorObject.transform.localPosition;
            closedRot = doorObject.transform.localEulerAngles;
        }

        if (doorObject_2 != null)
        {
            closedPos_2 = doorObject_2.transform.localPosition;
            closedRot_2 = doorObject_2.transform.localEulerAngles;
        }

        lightEffect = null;
    }

    private void FixedUpdate()
    {
        if (state == EDoorState.IS_OPENING)
        {
            Open();
        }
        else if (state == EDoorState.IS_CLOSING)
        {
            Close();
        }
    }

    protected override float InteractionStartDuration()
    {
        return startDurationTime;
    }

    protected override float InteractionEndDuration()
    {
        return endDurationTime;
    }

    private void Open()
    {
        float timeSinceStarted = Time.time - timeStartedLerping;
        float percentageCompleted = timeSinceStarted / animationDuration;

        if (doorObject != null)
        {
            doorObject.transform.localPosition = Vector3.Lerp(startPos, openPos, percentageCompleted);
            doorObject.transform.localEulerAngles = Vector3.Lerp(startRot, openRot, percentageCompleted);
        }

        if (doorObject_2 != null)
        {
            doorObject_2.transform.localPosition = Vector3.Lerp(startPos_2, openPos_2, percentageCompleted);
            doorObject_2.transform.localEulerAngles = Vector3.Lerp(startRot_2, openRot_2, percentageCompleted);
        }

        if (percentageCompleted >= 1)
        {
            state = EDoorState.IS_WAITING;
        }
    }

    private void Close()
    {
        float timeSinceStarted = Time.time - timeStartedLerping;
        float percentageCompleted = timeSinceStarted / animationDuration;

        if (doorObject != null)
        {
            doorObject.transform.localPosition = Vector3.Lerp(startPos, closedPos, percentageCompleted);
            doorObject.transform.localEulerAngles = Vector3.Lerp(startRot, closedRot, percentageCompleted);
        }

        if (doorObject_2 != null)
        {
            doorObject_2.transform.localPosition = Vector3.Lerp(startPos_2, closedPos_2, percentageCompleted);
            doorObject_2.transform.localEulerAngles = Vector3.Lerp(startRot_2, closedRot_2, percentageCompleted);
        }

        if (percentageCompleted >= 1)
        {
            state = EDoorState.IS_WAITING;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent(typeof(IPossessable)))
        {
            IPossessable hitUser = other.GetComponent<IPossessable>();
            if (hitUser.GetIsPossessed())
            {
                if (ContainPermissionList(hitUser.GetRobotType()))
                {
                    timeStartedLerping = Time.time;
                    if (doorObject != null)
                    {
                        startPos = doorObject.transform.localPosition;
                        startRot = doorObject.transform.localEulerAngles;
                    }
                    if (doorObject_2 != null)
                    {
                        startPos_2 = doorObject_2.transform.localPosition;
                        startRot_2 = doorObject_2.transform.localEulerAngles;
                    }
                    state = EDoorState.IS_OPENING;
                    CreateGreenLightEffect();
                    return;
                }
                else
                {
                    CreateRedLightEffect();
                    return;
                }
            }
        }
        else if (other.GetComponent<EnemyBase>())
        {
            timeStartedLerping = Time.time;
            if (doorObject != null)
            {
                startPos = doorObject.transform.localPosition;
                startRot = doorObject.transform.localEulerAngles;
            }
            if (doorObject_2 != null)
            {
                startPos_2 = doorObject_2.transform.localPosition;
                startRot_2 = doorObject_2.transform.localEulerAngles;
            }
            state = EDoorState.IS_OPENING;
            CreateGreenLightEffect();
            return;
        }

    }

    private void OnTriggerExit(Collider other)
    {

        if (autoClose)
        {
            if (other.GetComponent(typeof(IPossessable)))
            {
                IPossessable hitUser = other.GetComponent<IPossessable>();
                if (hitUser.GetIsPossessed())
                {
                    if (ContainPermissionList(hitUser.GetRobotType()))
                    {
                        timeStartedLerping = Time.time;
                        if (doorObject != null)
                        {
                            startPos = doorObject.transform.localPosition;
                            startRot = doorObject.transform.localEulerAngles;
                        }
                        if (doorObject_2 != null)
                        {
                            startPos_2 = doorObject_2.transform.localPosition;
                            startRot_2 = doorObject_2.transform.localEulerAngles;
                        }
                        state = EDoorState.IS_CLOSING;
                    }
                }
                OffLightEffect();
            }
            else if (other.GetComponent<EnemyBase>())
            {
                timeStartedLerping = Time.time;
                if (doorObject != null)
                {
                    startPos = doorObject.transform.localPosition;
                    startRot = doorObject.transform.localEulerAngles;
                }
                if (doorObject_2 != null)
                {
                    startPos_2 = doorObject_2.transform.localPosition;
                    startRot_2 = doorObject_2.transform.localEulerAngles;
                }
                state = EDoorState.IS_CLOSING;
                OffLightEffect();
            }
        }
    }

    void CreateGreenLightEffect()
    {
        if (lightEffect != null)
        {
            OffLightEffect();
        }
        if (accessLightMarker != null)
        {
            lightEffect = Instantiate(Resources.Load<ParticleSystem>("ParticleEffect/GreenLight"),
                accessLightMarker);
            if (accessLightMarker2 != null)
            {
                lightEffect2 = Instantiate(Resources.Load<ParticleSystem>("ParticleEffect/GreenLight"),
                    accessLightMarker);
            }
        }
        else
        {
            lightEffect = Instantiate(Resources.Load<ParticleSystem>("ParticleEffect/GreenLight"),
                gameObject.transform.position + new Vector3(0, 3, 0), Quaternion.identity);
        }
        lightEffect.Play();
    }
    void CreateRedLightEffect()
    {
        if (lightEffect != null)
        {
            OffLightEffect();
        }
        if (accessLightMarker != null)
        {
            lightEffect = Instantiate(Resources.Load<ParticleSystem>("ParticleEffect/RedLight"),
            accessLightMarker);
            if (accessLightMarker2 != null)
            {
                lightEffect2 = Instantiate(Resources.Load<ParticleSystem>("ParticleEffect/RedLight"),
                    accessLightMarker);
            }
        }
        else
        {
            lightEffect = Instantiate(Resources.Load<ParticleSystem>("ParticleEffect/RedLight"),
            gameObject.transform.position + new Vector3(0, 3, 0), Quaternion.identity);
        }
        lightEffect.Play();
    }

    void OffLightEffect()
    {
        if (lightEffect != null)
        {
            Destroy(lightEffect.gameObject);
        }
        if (lightEffect2 != null)
        {
            Destroy(lightEffect2.gameObject);
        }
    }
}

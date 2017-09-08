using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FieldOfView))]
public class EnemyBase : MonoBehaviour
{
    #region References & variables
    protected Toolbox toolbox;
    protected EventManager em;
    protected Player player;
    protected FieldOfView myFoV;
    protected List<PatrolPoint> patrolPoints = new List<PatrolPoint>();
    protected bool initialized = false;
    protected bool isAlerted = false;
    protected int chaseState = 0; //0 = not chasing, 1 = target acquired, 2 = searching for target (target just got away)
    protected ERobotType wantedRobot = ERobotType.NONE;
    protected bool isTrackingTarget = false;
    protected Transform currentTarget;
    readonly bool canPatrol = true;
    float visionTickInterval = 0.1f;
    float visionTickTimer = 0;
    [SerializeField]
    protected float defaultVisionRange = 10f;
    protected float currentVisionRange = 0f;
    [SerializeField]
    protected float defaultVisionAngle = 30f;
    protected float currentVisionAngle = 0f;
    protected float visionRangeMultiplier = 1f;
    protected float visionAngleMultiplier = 1f;
    protected float eyeLevel = 2f;
    [SerializeField]
    LayerMask obstacleMask;
    protected int currentSecurityTier = 0;
    int visionAngleIncreasePerSecurityTier = 15;
    #endregion

    private void Awake()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();
    }

    private void OnEnable()
    {
        em.OnAlertStateChange += OnAlartStateChange;
        em.OnSecurityTierChange += OnSecurityTierChange;
    }

    private void OnDisable()
    {
        em.OnAlertStateChange -= OnAlartStateChange;
        em.OnSecurityTierChange -= OnSecurityTierChange;
    }

    private void OnAlartStateChange(int newState, ERobotType newWantedRobot)
    {
        switch (newState)
        {
            case 0:
                if (!isAlerted)
                {
                    return;
                }

                SetIsAlerted(false);
                //TODO: Return to original patrol route, reset fov angle and range
                visionRangeMultiplier = visionAngleMultiplier = 1;
                break;
            case 1:
                if (isAlerted)
                {
                    return;
                }

                SetIsAlerted(true);
                //TODO: Set fov angle and range to alerted values
                visionRangeMultiplier = 1.5f;
                visionAngleMultiplier = 2f;
                break;
            case 2:
                if (isAlerted)
                {
                    return;
                }

                isAlerted = true;
                //TODO: Set fov angle and range to alerted values
                visionRangeMultiplier = 1.5f;
                visionAngleMultiplier = 2f;
                break;
            default:
                break;
        }

    }

    private void OnSecurityTierChange(int newTier)
    {
        currentSecurityTier = newTier;
    }

    public virtual void InitializeEnemy()
    {
        player = em.BroadcastRequestPlayerReference().GetComponent<Player>();
        myFoV = GetComponent<FieldOfView>();

        if (patrolPoints.Count <= 1)
        {
            Debug.LogWarning("Patroller has only one patrol point, so it will remain stationary!");
        }

        initialized = true;
    }

    public bool GetCanPatrol()
    {
        return canPatrol;
    }

    public int GetChaseState()
    {
        return chaseState;
    }

    public bool GetIsTrackingTarget()
    {
        return isTrackingTarget;
    }

    public float GetVisionRange()
    {
        return currentVisionRange;
    }

    public float GetVisionAngle()
    {
        return currentVisionAngle;
    }

    public void SetPatrolPoints(List<PatrolPoint> newPatrolPoints)
    {
        patrolPoints = newPatrolPoints;
    }

    protected virtual void SetIsAlerted(bool newState)
    {
        if (isAlerted != newState)
        {
            isAlerted = newState;
        }
    }

    protected virtual void StartChase()
    {
        Debug.Log("StartChase");
        chaseState = 1;
    }

    protected virtual void EndChase()
    {
        Debug.Log("EndChase");
        chaseState = 0;
    }

    protected bool TargetInSight()
    {
        //Is the target within the vision range?
        Vector3 targetVector = player.transform.position - transform.position;
        if (targetVector.magnitude <= currentVisionRange)
        {
            //Is the target within the vision angle?
            float angleToTarget = Vector3.Angle(targetVector, transform.forward);
            if (angleToTarget <= currentVisionAngle / 2)
            {
                Vector3 raycastOrigin = transform.position;
                raycastOrigin.y += eyeLevel;
                Vector3 raycastDirection = player.transform.position - raycastOrigin;

                RaycastHit hit;
                if (Physics.Raycast(raycastOrigin, raycastDirection, out hit, currentVisionRange, obstacleMask))
                {
                    if (hit.collider.gameObject == currentTarget.gameObject)
                    {
                        return true;
                    }
                }
            }
        }
        
        return false;
    }

    #region FixedUpdate & LateUpdate
    protected virtual void FixedUpdate()
    {
        if (initialized)
        {
            
            currentVisionRange = defaultVisionRange * visionRangeMultiplier;
            currentVisionAngle = defaultVisionAngle * visionAngleMultiplier 
                + (currentSecurityTier * visionAngleIncreasePerSecurityTier);
            myFoV.SetViewRange(currentVisionRange);
            myFoV.SetViewAngle(currentVisionAngle);

            if(chaseState == 0)
            {
                visionTickTimer += Time.fixedDeltaTime;
                if (visionTickTimer >= visionTickInterval)
                {
                    //Debug.Log("Vision tick");
                    //Is the player within the vision range?
                    Vector3 targetVector = player.transform.position - transform.position;
                    if (targetVector.magnitude <= currentVisionRange)
                    {
                        //Is the player within the vision angle?
                        float angleToTarget = Vector3.Angle(targetVector, transform.forward);
                        if (angleToTarget <= currentVisionAngle / 2)
                        {
                            Vector3 raycastOrigin = transform.position;
                            raycastOrigin.y += eyeLevel;
                            Vector3 raycastDirection = player.transform.position - raycastOrigin;

                            RaycastHit hit;
                            if (Physics.Raycast(raycastOrigin, raycastDirection, out hit, currentVisionRange, obstacleMask))
                            {
                                //Debug.Log("hit.distance: " + hit.distance);
                                if (hit.collider.CompareTag("Possessable"))
                                {
                                    IPossessable detectedRobot = hit.collider.GetComponent<IPossessable>();

                                    Debug.Log("hit.collider.CompareTag('Possessable')");
                                    Debug.Log("isAlerted: " + isAlerted 
                                        + ", detectedRobot.GetRobotType(): " + detectedRobot.GetRobotType() 
                                        + ", wantedRobot " + wantedRobot);
                                    if (isAlerted && detectedRobot.GetRobotType() == wantedRobot)
                                    {
                                        Debug.Log("isAlerted && detectedRobot.GetRobotType() == wantedRobot");
                                        Debug.DrawRay(raycastOrigin, raycastDirection.normalized * hit.distance, Color.red);
                                        Debug.Log("targetVector.magnitude: " + targetVector.magnitude
                                            + " , angleToTarget: " + angleToTarget);

                                        em.BroadcastDisobeyingDetected(detectedRobot.GetRobotType());
                                        currentTarget = detectedRobot.GetGameObject().transform;
                                        StartChase();
                                    }
                                    else if (detectedRobot.GetIsDisobeying())
                                    {
                                        em.BroadcastDisobeyingDetected(detectedRobot.GetRobotType());
                                        currentTarget = detectedRobot.GetGameObject().transform;
                                        wantedRobot = detectedRobot.GetRobotType();
                                        StartChase();
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }
    }

    protected virtual void LateUpdate()
    {
        myFoV.DrawFieldOfView();
    }
    #endregion
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FieldOfView))]
public class EnemyBase : MonoBehaviour {
    #region References & variables
    protected Toolbox toolbox;
    protected EventManager em;
    protected Player player;
    protected FieldOfView myFoV;
    protected List<PatrolPoint> patrolPoints = new List<PatrolPoint>();
    [SerializeField]
    protected SpriteAnimationController spriteController;

    protected bool isHostile = true; //If false, the enemy's vision cone is toggled off, and the enemy will ignore the player
    IPossessable stoppedPossessable = null;
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
    protected float defaultVisionRangeMultiplier = 1f;
    protected float alertedVisionRangeMultiplier = 1.5f;
    protected float defaultVisionAngleMultiplier = 1f;
    protected float alertedVisionAngleMultiplier = 2f;
    protected float eyeLevel = 2f;
    [SerializeField]
    LayerMask obstacleMask;
    protected int currentSecurityTier = 0;
    int visionAngleIncreasePerSecurityTier = 15;
    protected bool isPaused = false; //Separate from full game pause. Used for example during cutscenes if necessary to pause all movement
    //[SerializeField]
    float knockOutDuration = 5f;
    float knockOutTimer;
    protected bool isKnockedOut;
    #endregion

    private void Awake()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();
    }

    private void OnEnable()
    {
        if (isHostile)
        {
            em.OnAlertStateChange += OnAlartStateChange;
            em.OnSecurityTierChange += OnSecurityTierChange;
        }
        em.OnPauseActorsStateChange += OnPauseActorsStateChange;
    }

    private void OnDisable()
    {
        if (isHostile)
        {
            em.OnAlertStateChange -= OnAlartStateChange;
            em.OnSecurityTierChange -= OnSecurityTierChange;
        }
        em.OnPauseActorsStateChange -= OnPauseActorsStateChange;
    }

    public void SetIsHostile(bool newIsHostile)
    {
        isHostile = newIsHostile;
    }

    protected virtual void OnAlartStateChange(int newState, ERobotType newWantedRobot)
    {
        if (!isHostile)
        {
            switch (newState)
            {
                case 0:
                    if (!isAlerted)
                    {
                        return;
                    }

                    SetIsAlerted(false);
                    //TODO: Return to original patrol route
                    visionRangeMultiplier = defaultVisionRangeMultiplier;
                    visionAngleMultiplier = defaultVisionAngleMultiplier;
                    break;
                case 1:
                    if (isAlerted)
                    {
                        return;
                    }

                    SetIsAlerted(true);
                    visionRangeMultiplier = alertedVisionRangeMultiplier;
                    visionAngleMultiplier = alertedVisionAngleMultiplier;
                    break;
                case 2:
                    if (isAlerted)
                    {
                        return;
                    }

                    isAlerted = true;
                    visionRangeMultiplier = alertedVisionRangeMultiplier;
                    visionAngleMultiplier = alertedVisionAngleMultiplier;
                    break;
                default:
                    break;
            }
        }
    }

    protected void StopPossessable(IPossessable newStoppedPossessable)
    {
        if(stoppedPossessable != null)
        {
            stoppedPossessable.ChangeCanMoveState(true);
        }

        stoppedPossessable = newStoppedPossessable;
        stoppedPossessable.ChangeCanMoveState(false);
    }

    private void OnSecurityTierChange(int newTier)
    {
        if (isHostile)
        {
            currentSecurityTier = newTier;
        }
    }

    private void OnPauseActorsStateChange(bool newState)
    {
        isPaused = newState;
    }

    public virtual void InitializeEnemy()
    {
        if (isHostile)
        {
            player = em.BroadcastRequestPlayerReference().GetComponent<Player>();
            myFoV = GetComponent<FieldOfView>();
        }

        //if (patrolPoints.Count <= 1)
        //{
        //    Debug.LogWarning("Patroller has only one patrol point, so it will remain stationary!");
        //}

        if (spriteController)
        {
            spriteController.SetAnimationState(EAnimationState.IDLE);
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
        stoppedPossessable.ChangeCanMoveState(true);
    }

    protected bool TargetInSight()
    {
        //If the target is within vision range
        Vector3 targetVector = player.transform.position - transform.position;
        if (targetVector.magnitude <= currentVisionRange)
        {
            //If the target is within the vision angle
            float angleToTarget = Vector3.Angle(targetVector, transform.forward);
            if (angleToTarget <= currentVisionAngle / 2)
            {
                Vector3 raycastOrigin = transform.position;
                raycastOrigin.y += eyeLevel;
                Vector3 raycastDirection = player.transform.position - raycastOrigin;
                RaycastHit hit;
                //If we can hit anything with a raycast towards the target
                if (Physics.Raycast(raycastOrigin, raycastDirection, out hit, currentVisionRange, obstacleMask))
                {
                    //If the hit object is our target
                    if (hit.collider.gameObject == currentTarget.gameObject)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public virtual void KnockOut() {
        isKnockedOut = true;
        knockOutTimer = knockOutDuration;
        spriteController.StartKnockout();
        myFoV.ChangeConeState(false);
    }

    protected virtual void EndKnockOut() {
        isKnockedOut = false;
        myFoV.ChangeConeState(true);
    }

    #region FixedUpdate & LateUpdate
    protected virtual void FixedUpdate()
    {
        //TODO: change vision checks so that enemies can detect any possessable (not just player)
        if (initialized)
        {
            if (isHostile)
            {
                //Calculate and send current vision variables to our FieldOfView controller
                currentVisionRange = defaultVisionRange * visionRangeMultiplier;
                currentVisionAngle = defaultVisionAngle * visionAngleMultiplier
                    + (currentSecurityTier * visionAngleIncreasePerSecurityTier);
                myFoV.SetViewRange(currentVisionRange);
                myFoV.SetViewAngle(currentVisionAngle);

                if (isKnockedOut)
                {
                    knockOutTimer -= Time.fixedDeltaTime;
                    if (knockOutTimer <= 0.0f)
                    {
                        EndKnockOut();
                    }
                }

                //If we are not chasing a target
                else if (chaseState == 0)
                {
                    visionTickTimer += Time.fixedDeltaTime;
                    //If we should check and update our vision
                    if (visionTickTimer >= visionTickInterval)
                    {
                        //If the player is within the vision range
                        Vector3 targetVector = player.transform.position - transform.position;
                        if (targetVector.magnitude <= currentVisionRange)
                        {
                            //If the player is within the vision angle
                            float angleToTarget = Vector3.Angle(targetVector, transform.forward);
                            if (angleToTarget <= currentVisionAngle / 2)
                            {
                                Vector3 raycastOrigin = transform.position;
                                raycastOrigin.y += eyeLevel;
                                Vector3 raycastDirection = player.transform.position - raycastOrigin;

                                RaycastHit hit;
                                //If we can hit anything with a raycast in the players direction
                                if (Physics.Raycast(raycastOrigin, raycastDirection, out hit, currentVisionRange, obstacleMask))
                                {
                                    //If the hit object has the tag "Possessable"
                                    if (hit.collider.CompareTag("Possessable"))
                                    {
                                        IPossessable detectedRobot = hit.collider.GetComponent<IPossessable>();
                                        //If we are alerted and the hit possessable is a robot of the type we're looking for
                                        if (isAlerted && detectedRobot.GetRobotType() == wantedRobot)
                                        {
                                            //Send an event about detecting the target and start chasing it
                                            em.BroadcastDisobeyingDetected(detectedRobot.GetRobotType(), detectedRobot);
                                            StopPossessable(detectedRobot);
                                            currentTarget = detectedRobot.GetGameObject().transform;
                                            StartChase();
                                        }
                                        //If the hit possessable is currently disobeying
                                        else if (detectedRobot.GetIsDisobeying())
                                        {
                                            //Send an event about detecting the target and start chasing it
                                            em.BroadcastDisobeyingDetected(detectedRobot.GetRobotType(), detectedRobot);
                                            StopPossessable(detectedRobot);
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
    }

    protected virtual void LateUpdate()
    {
        if (isHostile)
        {
            myFoV.DrawFieldOfView();
        }
    }
    #endregion
}

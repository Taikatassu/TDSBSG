using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy_Patroller : EnemyBase
{
    #region References & variables
    NavMeshAgent navAgent;
    float defaultMovementSpeed = 3f;
    float currentMovementSpeed = 0f;
    float movementSpeedMultiplier = 1f;
    int currentPatrolPointIndex = -1;
    float navTickInterval = 0.1f;
    float navTickTimer = 0;
    float patrolPointCompleteRadius = 0.5f; //At which distance is the enemy considered to have reached the point
    float guardPointRadius = 0.1f; //The distance from the guard point will the enemy aim to move before starting to guard it
    float catchDistance = 1f; //At which distance is the enemy considered to have catched it's target
    bool isPatrolling = false;
    bool isGuarding = false;
    float guardingTimer = 0;
    bool rotatingToGuardPosition = false;
    bool atGuardRotation = false;
    Quaternion guardingSlerpStartRotation = Quaternion.identity;
    Quaternion guardingSlerpEndRotation = Quaternion.identity;
    float guardingSlerpDuration = 0.5f;
    float guardinSlerpStartTime = 0;
    bool lookingAround = false;
    Vector3 lookAroundCenterRotation = Vector3.zero;
    Vector3 lookAroundStartRotation = Vector3.zero;
    float lookAroundAngle = 30f;
    float lookAroundDuration = 1f;
    float lookAroundTime = 0f;
    Vector3 lastKnownTargetPos = Vector3.zero;
    #endregion

    public override void InitializeEnemy()
    {
        base.InitializeEnemy();
        navAgent = GetComponent<NavMeshAgent>();
        InitializePatrolling();
    }

    private void OnEnable()
    {
        em.OnPauseActorsStateChange += OnPauseActorsStateChange;
    }

    private void OnDisable()
    {
        em.OnPauseActorsStateChange -= OnPauseActorsStateChange;
    }

    private void OnPauseActorsStateChange(bool newState)
    {
        isPaused = newState;
    }

    #region Alerts
    protected override void SetIsAlerted(bool newState)
    {
        base.SetIsAlerted(newState);

        //if (isAlerted)
        //{
        //    movementSpeedMultiplier = 1.5f;
        //}
        //else
        //{
        //    movementSpeedMultiplier = 1f;
        //}
    }

    protected override void OnAlartStateChange(int newState, ERobotType newWantedRobot)
    {
        base.OnAlartStateChange(newState, newWantedRobot);

        switch (newState)
        {
            case 0:
                ResumePatrolling();
                break;
            case 1:
                break;
            case 2:
                break;
            default:
                break;
        }
    }
    #endregion

    #region Patrolling
    private void InitializePatrolling()
    {
        isPatrolling = true;

        //Teleport ourselves to the first patrol point
        transform.position = patrolPoints[0].transform.position;

        //If there are more than one patrol points
        if (patrolPoints.Count > 1)
        {
            //Set the second one as our current destination index
            currentPatrolPointIndex = 1;
        }
        //If there is only one patrol point
        else
        {
            //Set it as our current destination index (effectively standing in place)
            currentPatrolPointIndex = 0;
        }

        //Set the patrol point with the current destination index as our new destination
        navAgent.SetDestination(patrolPoints[currentPatrolPointIndex].transform.position);
    }

    private void StopPatrolling()
    {
        Debug.Log("StopPatrolling");
        isPatrolling = false;
    }

    private void ResumePatrolling()
    {
        Debug.Log("ResumePatrolling");
        isPatrolling = true;

        //If there are any patrol points
        if (patrolPoints.Count > 0)
        {
            float distanceToClosest = 0;
            int closestPointIndex = 0;
            bool first = true;

            //Find the closest one
            for (int i = 0; i < patrolPoints.Count; i++)
            {
                float distanceToThis = (transform.position
                    - patrolPoints[i].transform.position).magnitude;

                if (first || distanceToThis < distanceToClosest)
                {
                    first = false;
                    closestPointIndex = i;
                    distanceToClosest = distanceToThis;
                }
            }
            //Set the closest patrol point as our current destination
            currentPatrolPointIndex = closestPointIndex;
            navAgent.SetDestination(patrolPoints[currentPatrolPointIndex].transform.position);
        }
    }

    private void SetNextPatrolPoint()
    {
        //Debug.Log("SetNextPatrolPoint");
        if (currentPatrolPointIndex < patrolPoints.Count - 1)
        {
            currentPatrolPointIndex++;
            navAgent.SetDestination(patrolPoints[currentPatrolPointIndex].transform.position);
        }
        else
        {
            currentPatrolPointIndex = 0;
            navAgent.SetDestination(patrolPoints[currentPatrolPointIndex].transform.position);
        }

        atGuardRotation = false;
    }
    #endregion

    #region KnockOut
    public override void KnockOut()
    {
        base.KnockOut();
        navAgent.isStopped = true;
    }

    protected override void EndKnockOut()
    {
        base.EndKnockOut();
        navAgent.isStopped = false;
    }
    #endregion

    #region Start & EndChase
    protected override void StartChase()
    {
        base.StartChase();
        StopPatrolling();
        movementSpeedMultiplier = 1.5f;
        lastKnownTargetPos = currentTarget.position;
        navAgent.SetDestination(lastKnownTargetPos);
    }

    protected override void EndChase()
    {
        base.EndChase();
        movementSpeedMultiplier = 1f;
        ResumePatrolling();
    }
    #endregion

    #region Start & EndLookAround
    private void StartLookAround()
    {
        navAgent.ResetPath();
        lookAroundAngle = patrolPoints[currentPatrolPointIndex].GetLookAroundAngle();
        lookAroundDuration = patrolPoints[currentPatrolPointIndex].GetLookAroundDuration();
        lookAroundCenterRotation = patrolPoints[currentPatrolPointIndex].transform.eulerAngles;
        lookAroundStartRotation = transform.eulerAngles;
        float startPhase = (lookAroundStartRotation.y - lookAroundCenterRotation.y) / lookAroundAngle;
        lookAroundTime = (Mathf.Asin(startPhase)) * lookAroundDuration;
        lookingAround = true;
    }

    private void EndLookAround()
    {
        navAgent.ResetPath();
        lookingAround = false;
    }
    #endregion

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!isPaused)
        {
            if (initialized)
            {
                if (!isKnockedOut)
                {//Calculate and send currentMovementSpeed to navAgent
                    currentMovementSpeed = defaultMovementSpeed * movementSpeedMultiplier;
                    navAgent.speed = currentMovementSpeed;

                    //If we are currently patrolling
                    if (isPatrolling)
                    {
                        #region Patrolling
                        #region Rotating to guarding position
                        //If we are currently trying to align ourselves with a guarding point
                        if (rotatingToGuardPosition)
                        {
                            //Turn to the proper alignment slowly
                            float guardingSlerpTimeSinceStarted = Time.time - guardinSlerpStartTime;
                            float guardingSlerpPercentage = guardingSlerpTimeSinceStarted / guardingSlerpDuration;

                            transform.rotation = Quaternion.Slerp(guardingSlerpStartRotation,
                                guardingSlerpEndRotation, guardingSlerpPercentage);

                            //If we are aligned
                            if (guardingSlerpPercentage >= 1)
                            {
                                //Stop aligning and start looking around
                                atGuardRotation = true;
                                rotatingToGuardPosition = false;
                                if (patrolPoints[currentPatrolPointIndex].GetShouldLookAround())
                                {
                                    StartLookAround();
                                }
                            }
                        }
                        #endregion

                        #region Looking around
                        //If we are currently looking around
                        if (lookingAround)
                        {
                            //Calculate and update current look around rotation
                            float phase = Mathf.Sin(lookAroundTime / lookAroundDuration);
                            transform.eulerAngles = new Vector3(lookAroundCenterRotation.x,
                                lookAroundCenterRotation.y + phase * lookAroundAngle, lookAroundCenterRotation.z);

                            lookAroundTime += Time.fixedDeltaTime;
                        }
                        #endregion

                        navTickTimer += Time.fixedDeltaTime;

                        //If we should check and update navigation
                        if (navTickTimer >= navTickInterval)
                        {
                            navTickTimer = 0;

                            //If we are currently guarding a patrol point
                            if (isGuarding)
                            {
                                #region Patrol point guarding
                                //If were close enough to our current destination point
                                if (navAgent.remainingDistance < guardPointRadius)
                                {
                                    //If we should look around when guarding the patrol point (and aren't already doing that)
                                    if (!rotatingToGuardPosition && !atGuardRotation && !lookingAround
                                        && patrolPoints[currentPatrolPointIndex].GetShouldLookAround())
                                    {
                                        lookAroundCenterRotation = patrolPoints[currentPatrolPointIndex].transform.eulerAngles;

                                        //If our arrival rotation is outside of the look around zone
                                        if (Mathf.Abs(transform.eulerAngles.y - lookAroundCenterRotation.y)
                                            > patrolPoints[currentPatrolPointIndex].GetLookAroundAngle())
                                        {
                                            //If we are not already rotating to be within the look around zone
                                            if (!rotatingToGuardPosition && !atGuardRotation)
                                            {
                                                //If it's closer for us to rotate at the positive side of the look around zone
                                                if (transform.eulerAngles.y - (patrolPoints[currentPatrolPointIndex].transform.eulerAngles.y
                                                    + patrolPoints[currentPatrolPointIndex].GetLookAroundAngle())
                                                    < transform.eulerAngles.y - (patrolPoints[currentPatrolPointIndex].transform.eulerAngles.y
                                                    - patrolPoints[currentPatrolPointIndex].GetLookAroundAngle()))
                                                {
                                                    //Set the look around zone's positive end as the end rotation
                                                    guardingSlerpEndRotation = Quaternion.Euler(new Vector3(0,
                                                        (patrolPoints[currentPatrolPointIndex].transform.eulerAngles.y
                                                        + patrolPoints[currentPatrolPointIndex].GetLookAroundAngle()) - 1, 0));
                                                }
                                                //If it's closer for us to rotate at the negative side of the look around zone
                                                else
                                                {
                                                    //Set the look around zone's negative end as the end rotation
                                                    guardingSlerpEndRotation = Quaternion.Euler(new Vector3(0,
                                                        (patrolPoints[currentPatrolPointIndex].transform.eulerAngles.y
                                                        - patrolPoints[currentPatrolPointIndex].GetLookAroundAngle()) + 1, 0));
                                                }

                                                //Initialize other parameters required to rotate to the closest edge of the look around zone
                                                guardingSlerpStartRotation = transform.rotation;
                                                guardinSlerpStartTime = Time.time;
                                                rotatingToGuardPosition = true;
                                            }
                                        }
                                        //If our arrival rotatin is within the look around zone
                                        else
                                        {
                                            //Start looking around
                                            atGuardRotation = true;
                                            StartLookAround();
                                        }
                                    }
                                    //If we should not look around, but are not yet properly aligned with the patrol point
                                    else if (!rotatingToGuardPosition && !atGuardRotation)
                                    {
                                        //Intialize parameters and start turning to the proper alignment
                                        guardingSlerpStartRotation = transform.rotation;
                                        guardingSlerpEndRotation = patrolPoints[currentPatrolPointIndex].transform.rotation;
                                        guardinSlerpStartTime = Time.time;
                                        rotatingToGuardPosition = true;
                                    }

                                    guardingTimer -= navTickInterval;
                                    //If the guarding timer is at zero
                                    if (guardingTimer <= 0)
                                    {
                                        //Stop guarding and move to the next point
                                        isGuarding = false;
                                        EndLookAround();
                                        SetNextPatrolPoint();
                                    }
                                }
                                #endregion
                            }
                            //If we are within the acceptable complete radius of the patrol point
                            else if (navAgent.remainingDistance < patrolPointCompleteRadius)
                            {
                                #region Setting next patrol point if current reached, and guarding not neccessary
                                float newGuardingDuration = patrolPoints[currentPatrolPointIndex].GetGuardingDuration();
                                //If we should be guarding the patrol point
                                if (newGuardingDuration > 0)
                                {
                                    //Start guarding the patrol point
                                    guardingTimer = newGuardingDuration;
                                    isGuarding = true;
                                }
                                //If not, move to the next destination
                                else
                                {
                                    SetNextPatrolPoint();
                                }
                                #endregion
                            }
                        }
                        #endregion
                    }
                    //TODO: DONE While target is visible and outside of catching distance, chase target
                    //If AI (player) swaps body within enemy vision cone while being chased, enemy target 
                    //changes to the newly possessed unit
                    //If target gets within catching distance, check if player
                    //If catched target was player, game over
                    //If not, return to your original task
                    //DONE If target gets outside of the enemy's vision, go to the last known location
                    //DONE If target (or any other robot of the same type) re-enters enemys vision, restart chase
                    //DONE If target is still not visible, (look around and then) return to your original duties

                    //If we are currently chasing a target
                    else if (chaseState == 1)
                    {
                        #region Chasing
                        navTickTimer += Time.fixedDeltaTime;

                        //If we should check and update navigation
                        if (navTickTimer >= navTickInterval)
                        {
                            navTickTimer = 0;

                            Debug.Log("ChaseState == 1, navTick");
                            float distanceToTarget = navAgent.remainingDistance;
                            //If we are not yet close enough to catch the target
                            if (distanceToTarget > catchDistance)
                            {
                                Debug.Log("distanceToTarget > catchDistance");
                                //If the target is in sight
                                if (TargetInSight())
                                {
                                    Debug.Log("Target is in sight, updating position");
                                    //Set the target's current position as our destination
                                    lastKnownTargetPos = currentTarget.position;
                                    navAgent.SetDestination(lastKnownTargetPos);
                                }
                                else
                                {
                                    navAgent.SetDestination(lastKnownTargetPos);
                                }

                                ////If the target is within catching distance
                                //if (distanceToTarget < catchDistance)
                                //{
                                //    if (TargetInSight())
                                //    {
                                //        Debug.Log("Target catched");
                                //        if (currentTarget.GetComponent(typeof(IPossessable)))
                                //        {
                                //            if (currentTarget.GetComponent<IPossessable>().GetIsPossessed())
                                //            {
                                //                Debug.Log("Catched target was player, broadcasting PlayerCatched");
                                //                em.BroadcastPlayerCatched();
                                //            }
                                //        }
                                //    }

                                //    EndChase();
                                //    ResumePatrolling();
                                //    //TODO: Check if player
                                //    //If yes, game over
                                //    //If not, look around
                                //    //If another robot of the wanted type sighted, start chasing it
                                //    //If not, restart patrolling
                                //}

                            }
                            //If the target is within catching distance
                            else if (distanceToTarget <= catchDistance)
                            {
                                Debug.Log("distanceToTarget <= catchDistance");
                                //TODO: Implement a "LookAround" method, and call it here, before returning to patrolling
                                if (TargetInSight())
                                {
                                    Debug.Log("Target catched");
                                    if (currentTarget.GetComponent(typeof(IPossessable)))
                                    {
                                        if (currentTarget.GetComponent<IPossessable>().GetIsPossessed())
                                        {
                                            Debug.Log("Catched target was player, broadcasting PlayerCatched");
                                            em.BroadcastPlayerCatched();
                                        }
                                    }
                                }

                                Debug.Log("Ending chase, resuming patrolling");
                                EndChase();
                                ResumePatrolling();
                            }
                        }
                        #endregion
                    }

                    if (isGuarding)
                    {
                        if (spriteController)
                        {
                            spriteController.SetAnimationState(EAnimationState.IDLE);
                        }
                    }
                    else if (isPatrolling)
                    {
                        if (spriteController)
                        {
                            spriteController.SetAnimationState(EAnimationState.WALK);
                        }
                    }
                    else if (chaseState != 0)
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
                }

            }
        }
    }


}

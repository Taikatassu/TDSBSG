using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy_Patroller : EnemyBase
{
    NavMeshAgent navAgent;
    float defaultMovementSpeed = 1.5f;
    float currentMovementSpeed = 0f;
    float movementSpeedMultiplier = 1f;
    int currentPatrolPointIndex = -1;
    float navTickInterval = 0.1f;
    float navTickTimer = 0;
    float patrolPointCompleteRadius = 0.5f;
    float catchDistance = 1f; //The distance at which the enemy is considered to have catched it's target
    bool isPatrolling = false;

    public override void InitializeEnemy()
    {
        base.InitializeEnemy();
        navAgent = GetComponent<NavMeshAgent>();
        InitializePatrolling();
    }

    private void InitializePatrolling()
    {
        isPatrolling = true;

        transform.position = patrolPoints[0].position;

        if (patrolPoints.Count > 0)
        {
            currentPatrolPointIndex = 1;
        }
        else
        {
            currentPatrolPointIndex = 0;
        }

        navAgent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
    }

    private void StopPatrolling()
    {
        Debug.Log("StopPatrolling");
        isPatrolling = false;
    }

    private void StartPatrolling()
    {
        Debug.Log("StartPatrolling");
        isPatrolling = true;

        if (patrolPoints.Count > 0)
        {
            float distanceToClosest = 0;
            int closestPointIndex = 0;
            bool first = true;

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

            currentPatrolPointIndex = closestPointIndex;
            navAgent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
        }
    }

    private void SetNextPatrolPoint()
    {
        //Debug.Log("SetNextPatrolPoint");
        if (currentPatrolPointIndex < patrolPoints.Count - 1)
        {
            currentPatrolPointIndex++;
            navAgent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
        }
        else
        {
            currentPatrolPointIndex = 0;
            navAgent.SetDestination(patrolPoints[currentPatrolPointIndex].position);
        }
    }

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

    protected override void StartChase()
    {
        base.StartChase();
        StopPatrolling();
        movementSpeedMultiplier = 1.5f;
        navAgent.SetDestination(currentTarget.position);
    }

    protected override void EndChase()
    {
        base.EndChase();
        movementSpeedMultiplier = 1f;
        StartPatrolling();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (initialized)
        {
            currentMovementSpeed = defaultMovementSpeed * movementSpeedMultiplier;

            navAgent.speed = currentMovementSpeed;

            if (isPatrolling)
            {
                navTickTimer += Time.fixedDeltaTime;

                if (navTickTimer >= navTickInterval)
                {
                    navTickTimer = 0;

                    if (navAgent.remainingDistance < patrolPointCompleteRadius)
                    {
                        SetNextPatrolPoint();
                    }
                }
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
            else if(chaseState == 1)
            {
                navTickTimer += Time.fixedDeltaTime;

                if (navTickTimer >= navTickInterval)
                {
                    //Debug.Log("NavTick");
                    navTickTimer = 0;

                    float distanceToTarget = navAgent.remainingDistance;
                    if (TargetInSight() && distanceToTarget > catchDistance)
                    {
                        navAgent.SetDestination(currentTarget.position);

                        if (distanceToTarget < catchDistance)
                        {
                            Debug.Log("Target catched");
                            StartPatrolling();
                            //TODO: Check if player
                            //If yes, game over
                            //If not, look around
                            //If another robot of the wanted type sighted, start chasing it
                            //If not, restart patrolling
                        }

                    }
                    else if(distanceToTarget < catchDistance)
                    {
                        Debug.Log("Last known target location reached, ending chase and returning to patrolling");
                        //TODO: Implement a "LookAround" method, and call it here, before returning to patrolling
                        EndChase();
                    }
                }
            }
        }
    }


}

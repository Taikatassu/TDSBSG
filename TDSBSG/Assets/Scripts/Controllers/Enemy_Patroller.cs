using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy_Patroller : EnemyBase
{
    NavMeshAgent navAgent;
    float movementSpeedMultiplier = 1f;
    int currentPatrolPointIndex = -1;
    float navTickInterval = 0.1f;
    float navTickTimer = 0;
    float patrolPointCompleteRadius = 0.5f;
    bool isPatrolling = false;

    public override void InitializeEnemy()
    {
        base.InitializeEnemy();
        navAgent = GetComponent<NavMeshAgent>();
        StartPatrolling();
    }

    private void StopPatrolling()
    {
        isPatrolling = false;
    }

    private void StartPatrolling()
    {
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

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (initialized)
        {
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
        }
    }


}

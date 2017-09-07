using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    #region References & variables
    protected Toolbox toolbox;
    protected EventManager em;
    protected Player player;
    protected List<Transform> patrolPoints = new List<Transform>();
    protected bool initialized = false;
    protected bool isAlerted = false;
    protected bool isTrackingTarget = false;
    readonly bool canPatrol = true;
    float visionTickInterval = 0.1f;
    float visionTickTimer = 0;
    float visionRange = 10f;
    float visionAngle = 30f;
    float visionAngleMultiplier = 1f;

    #endregion

    private void Awake()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();
    }

    //private void OnEnable()
    //{

    //}

    //private void OnDisable()
    //{

    //}

    public virtual void InitializeEnemy()
    {
        player = em.BroadcastRequestPlayerReference().GetComponent<Player>();
        initialized = true;
    }

    public bool GetIsAlerted()
    {
        return isAlerted;
    }

    public bool GetIsTrackingTarget()
    {
        return isTrackingTarget;
    }

    public void SetPatrolPoints(List<Transform> newPatrolPoints)
    {
        patrolPoints = newPatrolPoints;
    }

    private void FixedUpdate()
    {
        if (initialized)
        {
            visionTickTimer += Time.fixedDeltaTime;
            if (visionTickTimer >= visionTickInterval)
            {
                //Is the player within the vision range?
                Vector3 targetVector = player.transform.position - transform.position;
                if (targetVector.magnitude <= visionRange)
                {
                    //Is the player within the vision angle?
                    float angleToTarget = Vector3.Angle(targetVector, transform.forward);
                    if(angleToTarget <= visionAngle)
                    {
                        Debug.Log("targetVector.magnitude: " + targetVector.magnitude 
                            + " , angleToTarget: " + angleToTarget);
                    }
                }
            }
        }
    }

}

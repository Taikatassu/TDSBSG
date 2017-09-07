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
    protected List<Transform> patrolPoints = new List<Transform>();
    protected bool initialized = false;
    protected bool isAlerted = false;
    protected bool isTrackingTarget = false;
    readonly bool canPatrol = true;
    float visionTickInterval = 0.1f;
    float visionTickTimer = 0;
    [SerializeField]
    protected float defaultVisionRange = 10f;
    protected float currentVisionRange = 0f;
    [SerializeField]
    protected float defaultVisionAngle = 30f;
    protected float currentVisionAngle = 0f;
    protected float visionAngleMultiplier = 1f;
    protected float eyeLevel = 2f;

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
        myFoV = GetComponent<FieldOfView>();
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

    public float GetVisionRange()
    {
        return currentVisionRange;
    }

    public float GetVisionAngle()
    {
        return currentVisionAngle;
    }

    public void SetPatrolPoints(List<Transform> newPatrolPoints)
    {
        patrolPoints = newPatrolPoints;
    }

    //protected Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    //{
    //    if (!angleIsGlobal)
    //    {
    //        angleInDegrees += transform.eulerAngles.y;
    //    }

    //    return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0,
    //        Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    //}

    //private void DrawVisionCone()
    //{
    //    int stepCount = Mathf.RoundToInt(currentVisionAngle * visionMeshResolution);
    //    float stepAngleSize = currentVisionAngle / stepCount;

    //    for (int i = 0; i <= stepCount; i++)
    //    {
    //        float angle = transform.eulerAngles.y - currentVisionAngle / 2 + stepAngleSize * i;
    //        Debug.Log("angle: " + angle);
    //        Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true)
    //            * currentVisionRange, Color.red);
    //    }
    //}

    //ViewCastInfo ViewCast(float globalAngle)
    //{
    //    Vector3 dir = DirFromAngle(globalAngle, true);
    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, dir, out hit, currentVisionRange, obstacleMask))
    //    {
    //        return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
    //    }
    //    else
    //    {
    //        return new ViewCastInfo(false, transform.position + dir * currentVisionRange, 
    //            hit.distance, globalAngle);
    //    }
    //}

    //public struct ViewCastInfo
    //{
    //    bool hit;
    //    Vector3 point;
    //    float dist;
    //    float angle;

    //    public ViewCastInfo(bool _hit, Vector3 _point, float _dist, float _angle)
    //    {
    //        hit = _hit;
    //        point = _point;
    //        dist = _dist;
    //        angle = _angle;
    //    }

    //}


    protected virtual void FixedUpdate()
    {
        if (initialized)
        {
            currentVisionRange = defaultVisionRange;
            currentVisionAngle = defaultVisionAngle;


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
                        if (Physics.Raycast(raycastOrigin, raycastDirection, out hit, currentVisionRange))
                        {
                            //Debug.Log("hit.distance: " + hit.distance);
                            if (hit.collider.CompareTag("Possessable"))
                            {
                                if (hit.collider.GetComponent<IPossessable>().GetIsPossessed())
                                {
                                    Debug.DrawRay(raycastOrigin, raycastDirection.normalized * hit.distance, Color.red);
                                    Debug.Log("targetVector.magnitude: " + targetVector.magnitude
                                        + " , angleToTarget: " + angleToTarget);
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

}

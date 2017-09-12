using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //TODO: RayCast from camera to player, and hide any environmental objects if hit by the raycast
    //http://answers.unity3d.com/questions/44815/make-object-transparent-when-between-camera-and-pl.html
    //TODO: Add a parent to the camera, and implement rotation functionality
    //TODO: Implement zooming functionality to mouse wheel(lerp between mode 0 and 1 by the zoom percentage)


    #region References & variables
    Toolbox toolbox;
    EventManager em;
    Transform target;

    [SerializeField]
    float xOffsetMode0 = 0;
    [SerializeField]
    float yOffsetMode0 = 0;
    [SerializeField]
    float zOffsetMode0 = 0;
    [SerializeField]
    float xRotMode0 = 0;
    [SerializeField]
    float yRotMode0 = 0;
    [SerializeField]
    float zRotMode0 = 0;
    [SerializeField]
    float xOffsetMode1 = 0;
    [SerializeField]
    float yOffsetMode1 = 0;
    [SerializeField]
    float zOffsetMode1 = 0;
    [SerializeField]
    float xRotMode1 = 0;
    [SerializeField]
    float yRotMode1 = 0;
    [SerializeField]
    float zRotMode1 = 0;
    float smoothTime = 0.25f;
    Vector3 velocity = Vector3.zero;
    bool isFollowing = false;
    int cameraMode = 0; //0 = tilted & close, 1 = top down & high up (strategic / map view)
    bool isRotatingToDesiredRotation = false;
    Quaternion desiredRotation = Quaternion.identity;
    Quaternion startingRotation = Quaternion.identity;
    float rotationSlerpDuration = 0.75f;
    float rotationSlerpStartTime = 0f;
    float obscuringObjectsCheckTickInterval = 0.25f;
    float obscuringObjectsCheckTimer = 0f;
    float obscuringObjectsCheckSpherecastRadius = 3f;
    [SerializeField]
    LayerMask obscuringObjectsCheckMask;
    #endregion

    private void Awake()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();
    }

    private void OnEnable()
    {
        em.OnInitializeGame += OnInitializeGame;
        em.OnInputEvent += OnInputEvent;
    }

    private void OnDisable()
    {
        em.OnInitializeGame -= OnInitializeGame;
        em.OnInputEvent += OnInputEvent;
    }

    void OnInitializeGame()
    {
        target = em.BroadcastRequestPlayerReference().transform;

        cameraMode = 0;
        Vector3 targetPosition = target.position;
        Vector3 desiredPosition = new Vector3(targetPosition.x + xOffsetMode0,
            targetPosition.y + yOffsetMode0, targetPosition.z + zOffsetMode0);
        transform.position = desiredPosition;
        desiredRotation = Quaternion.Euler(new Vector3(xRotMode0, yRotMode0, zRotMode0));
        transform.rotation = desiredRotation;
        isFollowing = true;
    }

    private void OnInputEvent(EInputType newInput)
    {
        //TODO: Implement input detection and event broadcasting for new button input, "toggle cameraMode"
        //TODO: Toggle cameraMode when cameraMode button is pressed

        switch (newInput)
        {
            case EInputType.CAMERAMODE_KEYDOWN:
                ToggleCameraMode();
                break;
            case EInputType.CAMRAMODE_KEYUP:
                break;
            default:
                break;
        }
    }

    private void ToggleCameraMode()
    {
        if (cameraMode == 0)
        {
            cameraMode = 1;
            startingRotation = transform.rotation;
            desiredRotation = Quaternion.Euler(new Vector3(xRotMode1, yRotMode1, zRotMode1));
            rotationSlerpStartTime = Time.time;
            isRotatingToDesiredRotation = true;
        }
        else if (cameraMode == 1)
        {
            cameraMode = 0;
            startingRotation = transform.rotation;
            desiredRotation = Quaternion.Euler(new Vector3(xRotMode0, yRotMode0, zRotMode0));
            rotationSlerpStartTime = Time.time;
            isRotatingToDesiredRotation = true;
        }
    }

    private void FixedUpdate()
    {

        //TODO: Instead of trying to find objects with "HideableObject" component, add the component to all hit objects
        //TODO: Find out why this isn't working (hideable object found, but it will not turn transparent)
        obscuringObjectsCheckTimer -= Time.fixedDeltaTime;

        if(obscuringObjectsCheckTimer <= 0)
        {
            Debug.Log("obscuringObjectsCheck tick");
            obscuringObjectsCheckTimer = obscuringObjectsCheckTickInterval;

            RaycastHit[] hits;
            Vector3 spherecastDirection = target.position - transform.position;
            hits = Physics.SphereCastAll(transform.position, obscuringObjectsCheckSpherecastRadius,
                spherecastDirection, spherecastDirection.magnitude, obscuringObjectsCheckMask);
            
            if(hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.GetComponent<HideableObject>())
                    {
                        Debug.Log("Hideable object found, hiding with timer");
                        HideableObject hitHideable = hits[i].collider.GetComponent<HideableObject>();
                        hitHideable.HideObjectWithTimer();
                    }
                }
            }

        }
    }

    private void LateUpdate()
    {
        if (isFollowing)
        {
            Vector3 desiredPosition = Vector3.zero;
            if (cameraMode == 0)
            {
                //Calculate desired position with camera mode 0
                Vector3 targetPosition = target.position;
                desiredPosition = new Vector3(targetPosition.x + xOffsetMode0,
                   targetPosition.y + yOffsetMode0, targetPosition.z + zOffsetMode0);
            }
            else if (cameraMode == 1)
            {
                //Calculate desired position with camera mode 1
                Vector3 targetPosition = target.position;
                desiredPosition = new Vector3(targetPosition.x + xOffsetMode1,
                   targetPosition.y + yOffsetMode1, targetPosition.z + zOffsetMode1);
            }

            //Smoothly move to the desired position
            transform.position = Vector3.SmoothDamp(transform.position,
                desiredPosition, ref velocity, smoothTime);

            //transform.LookAt(target);
            //transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, 0);
        }

        if (isRotatingToDesiredRotation)
        {
            float timeSinceStarted = Time.time - rotationSlerpStartTime;
            float percentageCompleted = timeSinceStarted / rotationSlerpDuration;

            transform.rotation = Quaternion.Slerp(startingRotation, desiredRotation, percentageCompleted);

            if (percentageCompleted >= 1)
            {
                isRotatingToDesiredRotation = false;
            }
        }
    }

}

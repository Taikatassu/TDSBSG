using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region References & variables
    Toolbox toolbox;
    EventManager em;
    Transform rotatorTransform;
    Transform target;
    Transform cameraZoomerTransform;

    [SerializeField]
    Vector3 posOffsetMode0 = Vector3.zero;
    [SerializeField]
    Vector3 posOffsetMode1 = Vector3.zero;
    float smoothTime = 0.25f;
    Vector3 rotatorRefVelocity = Vector3.zero;
    Vector3 cameraRefVeloity = Vector3.zero;
    bool isFollowing = false;
    int cameraMode = 0; //0 = tilted & close, 1 = top down & high up (strategic / map view)
    Vector3 rotationOffset = Vector3.zero;
    float rotationSpeed = 12f;
    Vector3 lastMousePosition = Vector3.zero;
    float cameraHeightPercentage = 0; //At what point between possOffsetMode0 and 
                                      //possOffsetMode1 do we want the camera to be (ranges from 0f to 1f)
    [SerializeField]
    float initialCameraHeightPercentage = 0.25f;
    [SerializeField]
    LayerMask cameraObscureMask;
    Vector3 cameraZoomerVelocity = Vector3.zero;
    [SerializeField]
    Camera cam;
    Vector3[] clipPoints;
    bool ignoreCameraCollision = false;
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
        em.OnRequestCameraReference += OnRequestCameraReference;
        em.OnMouseInputEvent += OnMouseInputEvent;
        em.OnPossessablePossessed += OnPossessablePossessed;
    }

    private void OnDisable()
    {
        em.OnInitializeGame -= OnInitializeGame;
        em.OnInputEvent -= OnInputEvent;
        em.OnRequestCameraReference -= OnRequestCameraReference;
        em.OnMouseInputEvent -= OnMouseInputEvent;
        em.OnPossessablePossessed -= OnPossessablePossessed;
    }

    void OnInitializeGame()
    {
        cameraZoomerTransform = transform.GetChild(0);
        rotatorTransform = transform.parent;
        target = em.BroadcastRequestPlayerReference().transform;

        cameraHeightPercentage = initialCameraHeightPercentage;
        cameraMode = 0;
        Vector3 targetPosition = target.position;
        rotatorTransform.position = target.position;
        rotatorTransform.eulerAngles = target.eulerAngles;
        transform.localPosition = Vector3.Lerp(posOffsetMode0, posOffsetMode1, cameraHeightPercentage);

        isFollowing = true;
    }

    private GameObject OnRequestCameraReference()
    {
        return gameObject;
    }

    private void OnInputEvent(EInputType newInput)
    {
        switch (newInput)
        {
            case EInputType.CAMERAMODE_KEYDOWN:
                ToggleCameraMode();
                break;
            default:
                break;
        }
    }

    private void OnMouseInputEvent(int button, bool down, Vector3 mousePosition)
    {
        if (button == 2)
        {
            lastMousePosition = mousePosition;
            if (down)
            {
                em.OnMousePositionChange += OnMousePositionChange;
            }
            else
            {
                em.OnMousePositionChange -= OnMousePositionChange;
            }
        }
    }

    private void OnMousePositionChange(Vector3 newPosition)
    {
        float mouseMoveDistance = lastMousePosition.x - newPosition.x;
        lastMousePosition = newPosition;

        rotationOffset.y = mouseMoveDistance * rotationSpeed * Time.fixedDeltaTime;

        rotatorTransform.eulerAngles += rotationOffset;
    }

    private void OnPossessablePossessed(bool wasStationary)
    {
        ignoreCameraCollision = wasStationary;
    }

    private void ToggleCameraMode()
    {
        if (cameraMode == 0)
        {
            cameraMode = 1;
        }
        else if (cameraMode == 1)
        {
            cameraMode = 0;
        }
    }

    private float CheckTargetVisibilityMultiCast()
    {
        float cameraZoomAdjustment = 0f;
        for (int i = 0; i < clipPoints.Length; i++)
        {
            Vector3 rayOrigin = target.position;
            rayOrigin.y++;
            Vector3 rayDirection = clipPoints[i] - target.position;
            float rayDistance = rayDirection.magnitude;

            Debug.DrawRay(rayOrigin, rayDirection, Color.blue, 1f);
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance, cameraObscureMask, QueryTriggerInteraction.Ignore))
            {

                Debug.DrawRay(rayOrigin, rayDirection.normalized * hit.distance, Color.blue, 1f);

                if (cameraZoomAdjustment == 0)
                {
                    cameraZoomAdjustment = (transform.position - hit.point).magnitude;
                }
                else if ((transform.position - hit.point).magnitude < cameraZoomAdjustment)
                {
                    cameraZoomAdjustment = (transform.position - hit.point).magnitude;
                }
            }
        }

        return cameraZoomAdjustment;
    }

    private float CheckTargetVisibilitySingleCast()
    {
        float cameraZoomAdjustment = 0f;

        Vector3 rayOrigin = target.position;
        rayOrigin.y++;
        Vector3 rayDirection = transform.position - target.position;
        float rayDistance = rayDirection.magnitude;

        Debug.DrawRay(rayOrigin, rayDirection, Color.blue, 1f);
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance, cameraObscureMask, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(rayOrigin, rayDirection.normalized * hit.distance, Color.blue, 1f);
            cameraZoomAdjustment = (transform.position - hit.point).magnitude;
        }

        return cameraZoomAdjustment;
    }

    private void UpdateClipPoints()
    {
        if (cam != null)
        {
            float z = cam.nearClipPlane;
            float x = Mathf.Tan(cam.fieldOfView / 3.41f) * z;
            float y = x / cam.aspect;
            Quaternion atRotation = cam.transform.rotation;
            Vector3 cameraPosition = transform.position;

            clipPoints = new Vector3[5];
            clipPoints[0] = (atRotation * new Vector3(-x, y, z)) + cameraPosition;
            Debug.DrawRay(cameraPosition, clipPoints[0] - cameraPosition, Color.green, 0.1f);
            clipPoints[1] = (atRotation * new Vector3(x, y, z)) + cameraPosition;
            Debug.DrawRay(cameraPosition, clipPoints[1] - cameraPosition, Color.green, 0.1f);
            clipPoints[2] = (atRotation * new Vector3(-x, -y, z)) + cameraPosition;
            Debug.DrawRay(cameraPosition, clipPoints[2] - cameraPosition, Color.green, 0.1f);
            clipPoints[3] = (atRotation * new Vector3(x, -y, z)) + cameraPosition;
            Debug.DrawRay(cameraPosition, clipPoints[3] - cameraPosition, Color.green, 0.1f);
            clipPoints[4] = cameraPosition/* - camera.transform.forward*/;
        }
    }

    private void FixedUpdate()
    {

        Vector3 targetPosition = target.position;
        Vector3 rotatorDesiredPosition = targetPosition;

        rotatorTransform.position = Vector3.SmoothDamp(rotatorTransform.position,
            rotatorDesiredPosition, ref rotatorRefVelocity, smoothTime);
    }

    private void LateUpdate()
    {
        if (isFollowing)
        {
            Vector3 desiredCameraPosition = Vector3.zero;

            //TODO: Move this to input manager?
            cameraHeightPercentage -= Input.GetAxis("Mouse ScrollWheel") * 0.2f;
            cameraHeightPercentage = Mathf.Clamp(cameraHeightPercentage, 0.0f, 1.0f);

            desiredCameraPosition = Vector3.Lerp(posOffsetMode0, posOffsetMode1, cameraHeightPercentage);
            //Smoothly move to the desired position
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition,
                desiredCameraPosition, ref cameraRefVeloity, smoothTime);

            Vector3 lookAtPos = rotatorTransform.position;
            lookAtPos.y += 1;
            cameraZoomerTransform.LookAt(lookAtPos);

            UpdateClipPoints();
            float obscureDistance = CheckTargetVisibilityMultiCast();
            //float obscureDistance = CheckTargetVisibilitySingleCast();
            
            if (ignoreCameraCollision || cameraHeightPercentage > 0.2f)
            {
                cameraZoomerTransform.localPosition = Vector3.SmoothDamp(cameraZoomerTransform.localPosition,
                    Vector3.zero, ref cameraZoomerVelocity, smoothTime / 2);
            }
            else
            {
                if (obscureDistance > 0)
                {
                    Vector3 cameraZoomerDesiredPosition = transform.position
                        + cameraZoomerTransform.forward * (Mathf.Clamp((obscureDistance + 1), 0,
                        ((transform.position - target.position).magnitude - 2)));

                    cameraZoomerTransform.position = Vector3.SmoothDamp(cameraZoomerTransform.position,
                        cameraZoomerDesiredPosition, ref cameraZoomerVelocity, smoothTime / 2);
                    //cameraZoomerTransform.position = cameraZoomerDesiredPosition;
                }
                else
                {
                    cameraZoomerTransform.localPosition = Vector3.SmoothDamp(cameraZoomerTransform.localPosition,
                        Vector3.zero, ref cameraZoomerVelocity, smoothTime / 2);
                    //cameraZoomerTransform.localPosition = Vector3.zero;
                }
            }
        }
    }

}
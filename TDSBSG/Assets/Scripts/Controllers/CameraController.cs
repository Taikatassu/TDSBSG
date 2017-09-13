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
    Transform rotatorTransform;
    Transform target;
    
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
    float cameraZoom = 0;
    [SerializeField]
    float initialCameraZoom = 0.25f;
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
    }

    private void OnDisable()
    {
        em.OnInitializeGame -= OnInitializeGame;
        em.OnInputEvent -= OnInputEvent;
        em.OnRequestCameraReference -= OnRequestCameraReference;
        em.OnMouseInputEvent -= OnMouseInputEvent;
    }

    void OnInitializeGame()
    {
        rotatorTransform = transform.parent;
        target = em.BroadcastRequestPlayerReference().transform;

        cameraZoom = initialCameraZoom;
        cameraMode = 0;
        Vector3 targetPosition = target.position;
        rotatorTransform.position = target.position;
        rotatorTransform.eulerAngles = Vector3.zero;
        transform.localPosition = Vector3.Lerp(posOffsetMode0, posOffsetMode1, cameraZoom);//posOffsetMode0;
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
        if(button == 2)
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
            //if (cameraMode == 0)
            //{
            //    //Calculate desired position with camera mode 0
            //    desiredCameraPosition = posOffsetMode0;
            //}
            //else if (cameraMode == 1)
            //{
            //    //Calculate desired position with camera mode 1
            //    desiredCameraPosition = posOffsetMode1;
            //}

            //TODO: Move this to input manager?
            cameraZoom -= Input.GetAxis("Mouse ScrollWheel") * 0.2f;
            cameraZoom = Mathf.Clamp(cameraZoom, 0.0f, 1.0f);

            desiredCameraPosition = Vector3.Lerp(posOffsetMode0, posOffsetMode1, cameraZoom);
            
            //Smoothly move to the desired position
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition,
                desiredCameraPosition, ref cameraRefVeloity, smoothTime);

            Vector3 lookAtPos = rotatorTransform.position;
            lookAtPos.y += 2;
            transform.LookAt(lookAtPos);
        }
    }

}

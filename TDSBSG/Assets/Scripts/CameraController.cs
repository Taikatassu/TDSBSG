using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Toolbox toolbox;
    EventManager em;

    public Transform target;
    public float xOffset = 0;
    public float yOffset = 0;
    public float zOffset = 0;
    float smoothTime = 0.25f;
    Vector3 velocity = Vector3.zero;
    bool isFollowing = false;

    private void Awake()
    {
        
    }

    private void LateUpdate()
    {
        //Follow target
        Vector3 targetPosition = target.position;
        Vector3 desiredPosition = new Vector3(targetPosition.x + xOffset,
            targetPosition.y + yOffset, targetPosition.z + zOffset);
        //transform.position = desiredPosition;

        transform.position = Vector3.SmoothDamp(transform.position, 
            desiredPosition, ref velocity, smoothTime);
        
        //transform.LookAt(target);
        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, 0);
    }

}

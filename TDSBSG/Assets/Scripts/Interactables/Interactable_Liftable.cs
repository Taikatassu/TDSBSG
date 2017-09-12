using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Liftable : Interactable {

    float moveTime = 1.0f;     // Time taken to move
    float elapsedTime = 0.0f;  // elapsed time

    Vector3 startPosition;    // Initial position
    Vector3 endPosition;      // Position after movement

    // Use this for initialization
        void Start() {
            permissionList = new List<ERobotType>();
            permissionList.Add(ERobotType.DEFAULT);

            startPosition = transform.position;
            endPosition = transform.position;
            endPosition.y += 1.0f;
        }

    // Update is called once per frame
    void Update() { 

        // Use Lerp function for moving
        if (GetIsInUse())
        {
            InteractionStartDuration();
        }
        else
        {
            InteractionEndDuration();
        }
	}

    protected override float InteractionStartDuration()
    {
        // Clamp elapsedTime on maximum time
        if (startDurationTime >= moveTime) { startDurationTime = moveTime; }

        startDurationTime += moveTime * Time.deltaTime;
        endDurationTime = startDurationTime;
        transform.position = Vector3.Lerp(startPosition, endPosition, startDurationTime);

        return startDurationTime;
    }

    protected override float InteractionEndDuration()
    {
        // Clamp elapsedTime on minimum time
        if (endDurationTime <= 0) { endDurationTime = 0; }

        endDurationTime -= moveTime * Time.deltaTime;
        startDurationTime = endDurationTime;
        transform.position = Vector3.Lerp(startPosition, endPosition, endDurationTime);

        return endDurationTime;
    }

    public override float StartInteraction(IPossessable user)
    {
        if (base.StartInteraction(user) == -1.0f) { return -1.0f; }

        //transform.Translate(0, 1.0f, 0);
        return startDurationTime;
    }

    public override float EndInteraction(IPossessable user)
    {
        if (base.EndInteraction(user) == -1.0f) { return -1.0f; }
        

        return endDurationTime;
    }
}

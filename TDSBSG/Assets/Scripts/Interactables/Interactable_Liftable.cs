using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Liftable : Interactable {

    //float moveTime = 1.0f;     // Time taken to move

    //Vector3 startPosition;    // Initial position
    //Vector3 endPosition;      // Position after movement
    Transform currentLifter;

    // Use this for initialization
        void Start() {
            //permissionList = new List<ERobotType>();
            //permissionList.Add(ERobotType.DEFAULT);

            //startPosition = transform.position;
            //endPosition = transform.position;
            //endPosition.y += 1.0f;
        }

    private void FixedUpdate() {
        if (isInUse) {
            transform.position = currentLifter.position;
        }
    }

    protected override float InteractionStartDuration()
    {
        return startDurationTime;
    }

    protected override float InteractionEndDuration()
    {
        return endDurationTime;
    }

    public override float StartInteraction(IPossessable user) {
        if (base.StartInteraction(user) == -1.0f) { return -1.0f; }
        return startDurationTime;
    }

    public float StartInteraction(Transform newLifter) {
        currentLifter = newLifter;
        GetComponent<Collider>().enabled = false;
        isInUse = true;
        return 0.0f;
    }

    public override float EndInteraction(IPossessable user)
    {
        if (base.EndInteraction(user) == -1.0f) { return -1.0f; }
        GetComponent<Collider>().enabled = true;
        isInUse = false;
        return endDurationTime;
    }
}

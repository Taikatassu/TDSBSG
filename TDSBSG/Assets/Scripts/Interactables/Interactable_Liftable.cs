﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Liftable : Interactable {
    Transform currentLifter;

    private void FixedUpdate() {
        if (isInUse) {
            if (stationaryInteractable)
            {
                Vector3 newPos = transform.position;
                newPos.y = currentLifter.position.y;
                transform.position = newPos;
            }
            else
            {
                transform.position = currentLifter.position;
                transform.eulerAngles = currentLifter.eulerAngles;
            }
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
        if (!stationaryInteractable)
        {
            GetComponent<Collider>().enabled = false;
        }
        isInUse = true;
        return 0.0f;
    }

    public override float EndInteraction(IPossessable user)
    {
        if (base.EndInteraction(user) == -1.0f) { return -1.0f; }
        if (!stationaryInteractable)
        {
            GetComponent<Collider>().enabled = true;
        }
        isInUse = false;
        return endDurationTime;
    }
}

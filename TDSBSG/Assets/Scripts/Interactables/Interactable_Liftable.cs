using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Liftable : Interactable {

    float moveTime = 1.0f;     // Time taken to move
    float elapsedTime = 0.0f;  // elapsed time

    Vector3 startPosition;    // Initial position
    Vector3 endPosition;      // Position after movement

    // Use this for initialization
    void Start () {
        startPosition = transform.position;
        endPosition = transform.position;
        endPosition.y += 1.0f;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Z))
        {
            StartInteractable();
        }
        if (Input.GetKey(KeyCode.X))
        {
            EndInteractable();
        }

        // Use Lerp function for moving
        if (GetIsInUse())
        {
            // Clamp elapsedTime on maximum time
            if (elapsedTime >= moveTime) { elapsedTime = moveTime; }

            elapsedTime += moveTime * Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime);
        }
        else
        {
            // Clamp elapsedTime on minimum time
            if (elapsedTime <= 0) { elapsedTime = 0; }

            elapsedTime -= moveTime * Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime);
        }
	}

    public override bool StartInteractable()
    {
        if (!base.StartInteractable()) { return false; }

        //transform.Translate(0, 1.0f, 0);
        return false;
    }

    public override void EndInteractable()
    {
        base.EndInteractable();
    }
}

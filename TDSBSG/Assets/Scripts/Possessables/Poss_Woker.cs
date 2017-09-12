using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poss_Woker : Poss_Mobile {

    Transform lifter;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    protected override void FixedUpdate()
    {
        if (interactableObject != null)
        {
            interactableObject.transform.position = lifter.position;
            interactableObject.transform.rotation = lifter.rotation;
        }
    }
}

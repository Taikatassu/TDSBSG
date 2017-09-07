using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {
    [SerializeField]
    private bool isInUse;

    public bool GetIsInUse() { return isInUse; }

    public virtual bool StartInteractable()
    {
        if (isInUse) { return false; }

        isInUse = true;
        return true;
    }

    public virtual void EndInteractable()
    {
        isInUse = false;
    }
}

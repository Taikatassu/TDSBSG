using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTrigger : MonoBehaviour
{
    public delegate void GameObjectVoid(GameObject go);

    private void OnTriggerEnter(Collider other)
    {
        BroadcastTriggerEntered(other.gameObject);
    }

    public event GameObjectVoid OnTriggerEntered;
    public void  BroadcastTriggerEntered(GameObject enteringObject)
    {
        if (OnTriggerEntered != null)
        {
            OnTriggerEntered(enteringObject);
        }
    }
}

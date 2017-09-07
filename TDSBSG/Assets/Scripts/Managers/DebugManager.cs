using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField]
    bool showDebugIndicators = false;
    bool lastState = false;
    bool first = true;

    void Update()
    {
        if (showDebugIndicators != lastState || first)
        {
            first = false;
            GameObject[] debugIndicators = GameObject.FindGameObjectsWithTag("DebugIndicator");

            for (int i = 0; i < debugIndicators.Length; i++)
            {
                debugIndicators[i].GetComponent<MeshRenderer>().enabled = showDebugIndicators;
            }
        }

        lastState = showDebugIndicators;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poss_ConnectedMaster : Poss_Stationary
{
    [SerializeField]
    List<GameObject> connectedPossessablesMasterList = new List<GameObject>();
    [SerializeField]
    List<int> connectedPossessablePairs = new List<int>();

    protected override void OnInitializeGame()
    {
        base.OnInitializeGame();
        InitializeConnections();
    }

    private void InitializeConnections()
    {
        int count = connectedPossessablePairs.Count / 2;
        for (int i = 0; i < count; i++)
        {
            if (connectedPossessablePairs.Count >= i * 2 
                && connectedPossessablesMasterList.Count >= connectedPossessablePairs[i * 2])
            {
                IPossessable connectedFirst = connectedPossessablesMasterList[connectedPossessablePairs[i * 2]].GetComponent<IPossessable>();
                IPossessable connectedSecond = connectedPossessablesMasterList[connectedPossessablePairs[i * 2 + 1]].GetComponent<IPossessable>();
                connectedFirst.AddToConnectedPossessablesList(connectedSecond);
                connectedSecond.AddToConnectedPossessablesList(connectedFirst);

                Vector3 rayStart = connectedFirst.GetGameObject().transform.position;
                Vector3 rayDir = connectedSecond.GetGameObject().transform.position 
                    - connectedFirst.GetGameObject().transform.position;
                Debug.DrawRay(rayStart, rayDir, Color.red, 10f);
            }
        }


    }


}

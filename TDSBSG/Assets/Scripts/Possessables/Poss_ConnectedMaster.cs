using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poss_ConnectedMaster : Poss_Stationary
{
    [SerializeField]
    List<GameObject> connectedPossessablesMasterList = new List<GameObject>();
    [SerializeField]
    List<int> connectedPossessablePairs = new List<int>();
    List<ConnectionInfo> connectionInfoList = new List<ConnectionInfo>();
    bool connectionIndicatorsEnabled = false;
    float indicatorWidth = 0.3f;

    protected override void OnInitializeGame()
    {
        base.OnInitializeGame();
        connectionInfoList = new List<ConnectionInfo>();
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
                if (connectedFirst.GetGameObject().GetComponent<Poss_Stationary>())
                {
                    connectedFirst.GetGameObject().GetComponent<Poss_Stationary>().SetConnectionMaster(this);
                }
                if (connectedSecond.GetGameObject().GetComponent<Poss_Stationary>())
                {
                    connectedSecond.GetGameObject().GetComponent<Poss_Stationary>().SetConnectionMaster(this);
                }

                //Vector3 rayStart = connectedFirst.GetGameObject().transform.position;
                //Vector3 rayDir = connectedSecond.GetGameObject().transform.position
                //    - connectedFirst.GetGameObject().transform.position;
                //Debug.DrawRay(rayStart, rayDir, Color.red, 60f);

                GameObject newConnectionIndicator = new GameObject("ConnectionIndicator" + i);
                newConnectionIndicator.transform.SetParent(transform);
                newConnectionIndicator.transform.localPosition = Vector3.zero;
                newConnectionIndicator.layer = LayerMask.NameToLayer("Indicators");
                LineRenderer newLineRenderer = newConnectionIndicator.AddComponent<LineRenderer>();
                Material connectionIndicatorMaterial = Resources.Load("Materials/ConnectionIndicator_mat") as Material;
                newLineRenderer.material = connectionIndicatorMaterial;
                newLineRenderer.startWidth = indicatorWidth;
                newLineRenderer.endWidth = indicatorWidth;
                newLineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                newLineRenderer.receiveShadows = false;
                ConnectionInfo newConnection = new ConnectionInfo(newLineRenderer, connectedFirst.GetGameObject().transform, 
                    connectedSecond.GetGameObject().transform);
                connectionInfoList.Add(newConnection);
            }
        }

        connectionIndicatorsEnabled = true;
        UpdateConnectionIndicators();
        SetConnectionIndicatorState(false);
    }
    
    public void SetConnectionIndicatorState(bool newState)
    {
        if(connectionIndicatorsEnabled != newState)
        {
            connectionIndicatorsEnabled = newState;
            float count = connectionInfoList.Count;
            for (int i = 0; i < count; i++)
            {
                connectionInfoList[i].indicator.enabled = connectionIndicatorsEnabled;
            }

            UpdateConnectionIndicators();
        }
    }

    private void UpdateConnectionIndicators()
    {
        float count = connectionInfoList.Count;
        for (int i = 0; i < count; i++)
        {
            Vector3[] indicatorPosition = new Vector3[2];
            indicatorPosition[0] = connectionInfoList[i].startPoint.position;
            //indicatorPosition[0].y++;
            indicatorPosition[1] = connectionInfoList[i].endPoint.position;
            //indicatorPosition[1].y++;
            connectionInfoList[i].indicator.SetPositions(indicatorPosition);
        }
    }

    private void FixedUpdate()
    {
        if (connectionIndicatorsEnabled)
        {
            UpdateConnectionIndicators();
        }
    }

    public struct ConnectionInfo
    {
        public LineRenderer indicator;
        public Transform startPoint;
        public Transform endPoint;

        public ConnectionInfo(LineRenderer _lineRenderer, Transform start, Transform end)
        {
            indicator = _lineRenderer;
            startPoint = start;
            endPoint = end;
        }
    }
}

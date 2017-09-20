using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField]
    private float viewRange = 0f;
    [SerializeField]
    private float viewAngle = 0f;
    float visionMeshResolution = 0.5f;
    [SerializeField]
    LayerMask obstacleMask;
    public MeshFilter viewMeshFilter;
    Mesh viewMesh;
    MeshRenderer viewMeshRenderer = null;
    Material defaultMaterial = null;
    Material turnedOffMaterial = null;
    int edgeResolveIterations = 6;
    float edgeDistanceThreshold = 0.5f;

    private void Start()
    {
        GameObject viewMeshHolder = new GameObject("View Mesh Holder");
        viewMeshHolder.transform.SetParent(transform);
        Vector3 locPos = Vector3.zero;
        locPos.y = 0.5f;
        viewMeshHolder.transform.localPosition = locPos;
        viewMeshHolder.transform.localRotation = Quaternion.identity;
        viewMeshFilter = viewMeshHolder.AddComponent<MeshFilter>();
        viewMeshRenderer = viewMeshHolder.AddComponent<MeshRenderer>();
        viewMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        viewMeshRenderer.receiveShadows = false;
        defaultMaterial = Resources.Load("Materials/FovIndicatorOn_mat") as Material;
        turnedOffMaterial = Resources.Load("Materials/FovIndicatorOff_mat") as Material;
        viewMeshRenderer.material = defaultMaterial;
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

    }

    public void ChangeConeState(bool on)
    {
        if (on)
        {
            viewMeshRenderer.material = defaultMaterial;
        }
        else
        {
            viewMeshRenderer.material = turnedOffMaterial;
        }
    }

    public void SetViewRange(float newViewRange)
    {
        viewRange = newViewRange;
    }

    public void SetViewAngle(float newViewAngle)
    {
        viewAngle = newViewAngle;
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0,
            Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * visionMeshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDistanceThresholdExceeded
                    = Mathf.Abs(oldViewCast.dist - newViewCast.dist) > edgeDistanceThreshold;
                if (oldViewCast.hit != newViewCast.hit
                    || oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded)
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDistanceThresholdExceeded
                = Mathf.Abs(minViewCast.dist - newViewCast.dist) > edgeDistanceThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;
        Vector3 raycastOrigin = transform.position;
        raycastOrigin.y += 0.1f;
        if (Physics.Raycast(raycastOrigin, dir, out hit, viewRange, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, raycastOrigin + dir * viewRange,
                viewRange, globalAngle);
        }
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dist;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dist, float _angle)
        {
            hit = _hit;
            point = _point;
            dist = _dist;
            angle = _angle;
        }

    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }

}
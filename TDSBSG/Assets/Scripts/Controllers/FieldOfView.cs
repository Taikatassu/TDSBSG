using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{

    [SerializeField]
    private float viewRange = 0f;
    [SerializeField]
    private float viewAngle = 0f;
    [SerializeField]
    float visionMeshResolution = 1f;
    [SerializeField]
    LayerMask obstacleMask;
    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    private void Start()
    {
        GameObject viewMeshHolder = new GameObject("View Mesh Holder");
        viewMeshHolder.transform.SetParent(transform);
        Vector3 locPos = Vector3.zero;
        locPos.y = 0.5f;
        viewMeshHolder.transform.localPosition = locPos;
        viewMeshHolder.transform.localRotation = Quaternion.identity;
        viewMeshFilter = viewMeshHolder.AddComponent<MeshFilter>();
        MeshRenderer viewMeshRenderer = viewMeshHolder.AddComponent<MeshRenderer>();
        viewMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        viewMeshRenderer.receiveShadows = false;
        Material visionIndicatorMaterial 
            = Resources.Load("Materials/FovIndicator_mat") as Material;
        viewMeshRenderer.material = visionIndicatorMaterial;
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

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

    //TODO: Find out why the last two faces are not drawn
    public void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * visionMeshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            //Debug.Log("angle: " + angle);
            ViewCastInfo newViewCast = ViewCast(angle);
            viewPoints.Add(newViewCast.point);
        }

        int vertexCount = viewPoints.Count - 1;
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
        //Debug.Log("viewMesh created. vertexCount: " + vertexCount);
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, viewRange, obstacleMask))
        {
            //Debug.DrawRay(transform.position, hit.point - transform.position, Color.red);
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            //Debug.DrawRay(transform.position, dir * viewRange, Color.red);
            return new ViewCastInfo(false, transform.position + dir * viewRange,
                hit.distance, globalAngle);
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SphericalNavMesh : MonoBehaviour
{
    // Start is called before the first frame update
    public Mesh SphericalMesh;
    [SerializeField] private float enemyRadius = 1;
    [SerializeField] private float enemyHeight = 1;

    private Vector3[] vertices;
    private Dictionary<Vector3, List<Triangle>> nearbyTMap;
    private readonly Dictionary<Vector3, bool> traversableMap = new Dictionary<Vector3, bool>();
    private float updateTimer = 0;
    private bool updatedCorrectly = false;

    void Start()
    {
        ComputeMeshData();
    }

    void Update()
    {
        if (updateTimer == 0) UpdateTraversability();
        if (updateTimer > 5 && !updatedCorrectly)
        {
            updatedCorrectly = true;
            UpdateTraversability();
        }

        updateTimer += Time.deltaTime;
    }

    public Vector3 GetNearestVertex(GameObject other)
    {
        float? minDistance = null;
        Vector3 nearest = vertices[0];

        foreach (var vertex in vertices)
        {
            float currDist = Vector3.Distance(other.transform.position, vertex);
            if(minDistance == null || currDist < minDistance)
            {
                minDistance = currDist;
                nearest = vertex;
            }
        }

        return nearest;
    }

    public Vector3 GetVertex(int index)
    {
        return vertices[index];
    }

    public bool IsUpdatedCorrectly()
    {
        return this.updatedCorrectly;
    }

    private void UpdateTraversability()
    {
        foreach(var v in vertices)
        {
            if (!traversableMap.ContainsKey(v))
            {
                traversableMap.Add(v, CalculateTraversability(v));
            }
            else 
            {
                traversableMap[v] = CalculateTraversability(v);
            }
        }
    }

    private void ComputeMeshData()
    {
        // setup triangles array and NavPoint array sizes
        int[] triangles = SphericalMesh.triangles;
        Triangle[] faces = new Triangle[triangles.Length / 3];
        this.nearbyTMap = new Dictionary<Vector3, List<Triangle>>();

        // mutpliply each vertex to fit world size
        vertices = new Vector3[SphericalMesh.vertexCount];
        for(int i=0; i< SphericalMesh.vertexCount; i++)
        {
            vertices[i] = SphericalMesh.vertices[i] * (transform.parent.localScale.x/2);
        }

        // group by 3 the triangles and create a Triangle for each
        // meanwhile, create navPoints and assign them their father triangles
        for (int i = 0; i*3 < triangles.Length - 2; i++)
        {
            int currTriangleStart = i * 3;
            Vector3 v1 = vertices[triangles[currTriangleStart]];
            Vector3 v2 = vertices[triangles[currTriangleStart+1]];
            Vector3 v3 = vertices[triangles[currTriangleStart + 2]];

            faces[i] = new Triangle(triangles[currTriangleStart], triangles[currTriangleStart+1], triangles[currTriangleStart+2], this);

            // Add vertex and triangles to the map
            AddToNearby(v1, faces[i]);
            AddToNearby(v2, faces[i]);
            AddToNearby(v3, faces[i]);
            
        }
    }

    public bool IsTraversable(Vector3 vertex)
    {
        return traversableMap[vertex];
    }


    private bool CalculateTraversability(Vector3 vertex)
    {
        int layerMask = LayerMask.GetMask("Walls");
        Vector3 normal = (vertex - transform.position).normalized;
        Vector3 castStart = vertex - normal*5;
        float castLen = enemyHeight + 5;

        Ray ray = new Ray(castStart, normal);
        return !Physics.SphereCast(ray, enemyRadius, castLen, layerMask);
        
    }

    private void DrawTraversabilityGizmos(Vector3 vertex)
    {
        Gizmos.color = new Color(1,0.3f,0.3f,0.6f);
        Gizmos.DrawSphere(vertex, enemyRadius);
    }

    private void AddToNearby(Vector3 vector, Triangle t)
    {
        if (!nearbyTMap.ContainsKey(vector))
        {
            nearbyTMap.Add(vector, new List<Triangle>());
        }

        nearbyTMap[vector].Add(t);
    }

    public List<Vector3> GetNeighbors(Vector3 vertex)
    {
        List<Vector3> neighbors = new List<Vector3>();
        foreach(var v in vertices){
            if(vertex != null)
            {
                if (AreNeighbors(vertex, v) && vertex != v && !neighbors.Contains(v))
                {
                    neighbors.Add(v);
                }
            }
        }

        return neighbors;
    }

    public bool AreNeighbors(Vector3 v1, Vector3 v2)
    {
        // two vertex are neighbors if they have a triangle in common
        List<Triangle> nearby1 = nearbyTMap[v1];
        List<Triangle> nearby2 = nearbyTMap[v2];

        foreach(var t in nearby1)
        {
            if (nearby2.Contains(t))
                return true;
        }

        return false;
    }

    public static void DebugPath(Vector3[] vertices, Color c, float duration = 0)
    {
        for(int i=0; i<vertices.Length -1; i++)
        {
            Debug.DrawLine(vertices[i], vertices[i + 1], c, duration);
        }
    }

    public static void DebugVertexes(Triangle t, Color c, float duration)
    {
        Debug.DrawLine(t.GetVertex(0), t.GetVertex(1), c, duration);
        Debug.DrawLine(t.GetVertex(1), t.GetVertex(2), c, duration);
        Debug.DrawLine(t.GetVertex(2), t.GetVertex(0), c, duration);
        Debug.DrawRay(t.GetCenter(), Vector3.Cross(t.GetVertex(0) - t.GetVertex(1), t.GetVertex(0) - t.GetVertex(2)), c, duration);
    }

    private void OnDrawGizmos()
    {
        if(vertices != null)
        for (int i = 0; i < vertices.Length - 1; i++)
        {
            if(!IsTraversable(vertices[i]))
            DrawTraversabilityGizmos(vertices[i]);
        }
    }

}

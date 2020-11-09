using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SphericalNavMesh : MonoBehaviour
{
    // Start is called before the first frame update
    public float enemySize_TODO;
    public Mesh SphericalMesh;

    private Vector3[] vertices;
    private Dictionary<Vector3, List<Triangle>> nearbyTMap;


    void Start()
    {
        ComputeMeshData();
    }

    void Update()
    {
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
            addToNearby(v1, faces[i]);
            addToNearby(v2, faces[i]);
            addToNearby(v3, faces[i]);
            
        }
    }

    private void addToNearby(Vector3 vector, Triangle t)
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
                if (areNeighbors(vertex, v) && vertex != v && !neighbors.Contains(v))
                {
                    neighbors.Add(v);
                }
            }
        }

        return neighbors;
    }

    public bool areNeighbors(Vector3 v1, Vector3 v2)
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

    public static void DebugPath(Vector3[] vertices, Color c, float duration)
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

}

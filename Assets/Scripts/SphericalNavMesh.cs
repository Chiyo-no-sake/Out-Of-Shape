using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SphericalNavMesh : MonoBehaviour
{
    // Start is called before the first frame update
    public int detailLevel = 1;
    public Collider coll;
    public float enemySize_TODO;
    public Mesh SphericalMesh;

    private Triangle[] faces;
    private Vector3[] vertices;
    private int radius;

    void Start()
    {
        if (detailLevel < 0) detailLevel = 0;
        if (detailLevel > 6) detailLevel = 6;

        Collider coll = GetComponent<Collider>();
        if (coll != null) this.coll = coll;
        faces = ComputeTriangles();
    }

    void Update()
    {
    }

    // TODO: implement NavActor interface (extends MonoBehaviour) and GenericEnemy script ( implements NavActor )
    public Triangle GetCollidingTriangle(GameObject other)
    {
        Vector3 gravityVector = transform.localPosition - other.transform.localPosition;
        List<Triangle> intersecting = new List<Triangle>();

        foreach (var item in faces)
        {
            if (item != null)
            {
                List<Vector3> intersect = item.IntersectionsWith(other.transform.localPosition, gravityVector);
                intersect.ForEach(i => intersecting.Add(item));
            }
        }

        float? minDistance = null;
        Triangle nearest = null;

        intersecting.ForEach((t) => {
            float d = Vector3.Distance(other.transform.localPosition, t.GetCenter());
            if(minDistance==null || minDistance > d){
                minDistance = d;
                nearest = t;
            }
        });

        return nearest;
    }

    public Vector3 GetVertex(int index)
    {
        return vertices[index];
    }

    private Triangle[] ComputeTriangles()
    {
        // set up custom vertices snapped to world radius
        vertices = new Vector3[SphericalMesh.vertexCount];
        for(int i=0; i< SphericalMesh.vertexCount; i++)
        {
            vertices[i] = SphericalMesh.vertices[i] * (transform.parent.localScale.x/2);
        }

        // setup triangles pointers to vertices
        int[] triangles = SphericalMesh.triangles;
        Triangle[] faces = new Triangle[triangles.Length/3];

        // group by 3 the triangles and create a variable for each
        for (int i = 0; i*3 < triangles.Length - 2; i++)
        {
            int currTriangleStart = i * 3;
            faces[i] = new Triangle(triangles[currTriangleStart], triangles[currTriangleStart+1], triangles[currTriangleStart+2], this);
        }

        return faces;
    }

    public List<Triangle> GetNeighbors(Triangle t)
    {
        List<Triangle> neighbors = new List<Triangle>();
        foreach(var triangle in faces){
            if(triangle != null)
            {
                if (triangle.IsNeighbor(t) && !triangle.Equals(t) && !neighbors.Contains(triangle))
                {
                    neighbors.Add(triangle);
                }
            }
        }

        return neighbors;
    }

    private void OnCollisionStay(Collision other)
    {
        // dispatch the collision event on triangles
        //Triangle t = GetCollidingTriangle(other.gameObject);

        //DebugVertexes(t, Color.blue, 1);
        //foreach (var n in GetNeighbors(t))
        //{
        //    DebugVertexes(n, Color.green, 1);
        //}
    }

    public static void DebugVertexes(Triangle t, Color c, float duration)
    {
        Debug.DrawLine(t.GetVertex(0), t.GetVertex(1), c, duration);
        Debug.DrawLine(t.GetVertex(1), t.GetVertex(2), c, duration);
        Debug.DrawLine(t.GetVertex(2), t.GetVertex(0), c, duration);
        Debug.DrawRay(t.GetCenter(), Vector3.Cross(t.GetVertex(0) - t.GetVertex(1), t.GetVertex(0) - t.GetVertex(2)), c, duration);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SphericalNavMesh : MonoBehaviour
{
    // Start is called before the first frame update
    public int detailLevel = 1;
    public int radius = 1;
    public bool debug = false;
    public Collider coll;
    public float enemySize;

    private SphericalMesh navMesh;

    void Start()
    {
        if (detailLevel < 0) detailLevel = 0;
        if (detailLevel > 6) detailLevel = 6;

        navMesh = new SphericalMesh(detailLevel, radius);
        Collider coll = GetComponent<Collider>();
        if (coll != null) this.coll = coll;
    }

    void Update()
    {
    }

    // TODO: implement NavActor interface (extends MonoBehaviour and GenericEnemy script ( implements NavActor )
    public Triangle getCollidingTriangle(NavActor other)
    {
        Vector3 gravityVector = transform.localPosition - other.transform.localPosition;
        Triangle[] triangles = navMesh.getTriangles();
        List<Triangle> intersecting = new List<Triangle>();

        foreach (var item in triangles)
        {
            List<Vector3> intersect = item.IntersectionsWith(other.transform.localPosition, gravityVector);
            intersect.ForEach(i => intersecting.Add(item));
        }

        float? minDistance = null;
        Triangle nearest = null;

        intersecting.ForEach((t) => {
            float d = Vector3.Distance(transform.localPosition, t.getCenter());
            if(minDistance==null || minDistance > d){
                minDistance = d;
                nearest = t;
            }
        });

        return nearest;
    }

    public List<Triangle> getNeighbors(Triangle t)
    {
        List<Triangle> neighbors = new List<Triangle>();
        Triangle[] triangles = navMesh.getTriangles();
        foreach(var triangle in triangles){
            if (triangle.isNeighbor(t))
                neighbors.Add(triangle);
        }

        return neighbors;
    }

    private void OnCollisionEnter(Collision other)
    {
        // dispatch the collision event on triangles

    }
    //TODO set disable in case of obstacles
    /// From now on those are all math calc for mesh creation


    private Vector3 getLine(float t, Vector3 Point, Vector3 direction)
    {
        return Point + t * direction;
    }
}

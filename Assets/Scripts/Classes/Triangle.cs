using UnityEngine;
using System;
using System.Collections.Generic;

public class Triangle
{
    private Vector3[] vertices;
    private SphereCollider collider;
    private float collidingRadius;

    public float enemySize = 1;

    public Color color = Color.blue;
    public Triangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        vertices = new Vector3[3];
        vertices[0] = vertex1;
        vertices[1] = vertex2;
        vertices[2] = vertex3;
        collidingRadius = Vector3.Distance((vertices[1] + vertices[0]) / 2, getCenter());
    }

    public Vector3 getCenter()
    {
        return (vertices[0] + vertices[1] + vertices[2]) / 3f;
    }

    public Vector3[] getVertices()
    {
        return vertices;
    }

    public bool isNeighbor(Triangle oth)
    {
        int commonVertices = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (vertices[i] == oth.vertices[j])
                    //there is a common vertex
                    commonVertices++;
            }
        }

        return commonVertices == 2;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(getCenter(), collidingRadius);
    }

    public bool isColliding(Vector3 point)
    {
        return Vector3.Distance(point, getCenter()) - enemySize < collidingRadius;
    }

    public List<Vector3> IntersectionsWith(Vector3 point, Vector3 direction)
    {
        List<Vector3> i = new List<Vector3>();
        Vector3 edge1 = vertices[1] - vertices[0];
        Vector3 edge2 = vertices[2] - vertices[0];
        Vector3 normal = Vector3.Cross(edge1, edge2);
        float a = Vector3.Dot(vertices[0] - point, normal);
        float b = Vector3.Dot(direction, normal);
        if (b == 0) return i;

        Vector3 I = a / b * direction + point;
        if(PointInTriangle(I)) i.Add(I);

        return i;
    }

    // Just some math down there
    private static List<float> QuadraticSolve(float a, float b, float c)
    {
        // Local formula
        float formula(int plusOrMinus, float d) => (-b + (plusOrMinus * Mathf.Sqrt(d))) / (2 * a);
        float discriminant = b * b - 4 * a * c;

        List<float> result = new List<float>();
        if (discriminant >= 0)
        {
            result.Add(formula(1, discriminant));
            if (discriminant > 0)
                result.Add(formula(-1, discriminant));
        }
        return result;
    }

    private bool PointInTriangle(Vector3 pt)
    {
        float d1, d2, d3;
        bool has_neg, has_pos;

        float sign (Vector3 p1, Vector3 p2, Vector3 p3) => (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);

        d1 = sign(pt, vertices[0], vertices[1]);
        d2 = sign(pt, vertices[1], vertices[2]);
        d3 = sign(pt, vertices[2], vertices[0]);

        has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(has_neg && has_pos);
    }
}
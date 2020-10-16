using UnityEngine;
using System;
using System.Collections.Generic;

public class Triangle
{
    public readonly int[] vertexIndeces;
    private readonly float collidingRadius;
    private readonly SphericalNavMesh father;

    public float enemySize = 1;

    public Triangle(int vertexIndex1, int vertexIndex2, int vertexIndex3, SphericalNavMesh father)
    {
        this.father = father;
        vertexIndeces = new int[3];
        vertexIndeces[0] = vertexIndex1;
        vertexIndeces[1] = vertexIndex2;
        vertexIndeces[2] = vertexIndex3;
        collidingRadius = Vector3.Distance((GetVertex(1) + GetVertex(0)) / 2f, GetCenter());
    }

    public Vector3 GetCenter()
    {
        return (GetVertex(0) + GetVertex(1) + GetVertex(2)) / 3f;
    }

    public Vector3 GetVertex(int index)
    {
        return father.GetVertex(vertexIndeces[index]);
    }

    public bool IsNeighbor(Triangle oth)
    {
        int commonVertices = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j=0; j<3; j++)
            {
                if (GetVertex(i) == oth.GetVertex(j))
                    //there is a common vertex
                    commonVertices++;
            }
        }

        return commonVertices > 1;
    }

    public bool IsColliding(Vector3 point)
    {
        return Vector3.Distance(point, GetCenter()) - enemySize < collidingRadius;
    }

    public List<Vector3> IntersectionsWith(Vector3 point, Vector3 direction)
    {
        List<Vector3> i = new List<Vector3>();
        Vector3 edge1 = GetVertex(1) - GetVertex(0);
        Vector3 edge2 = GetVertex(2) - GetVertex(0);
        Vector3 normal = Vector3.Cross(edge1, edge2);
        float a = Vector3.Dot(GetVertex(0) - point, normal);
        float b = Vector3.Dot(direction, normal);
        if (b == 0) return i;

        Vector3 I = a / b * direction + point;
        if(PointInTriangle(I)) i.Add(I);

        return i;
    }

    private bool PointInTriangle(Vector3 pt)
    {
        float d1, d2, d3;
        bool has_neg, has_pos;

        float sign (Vector3 p1, Vector3 p2, Vector3 p3) => (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);

        d1 = sign(pt, GetVertex(0), GetVertex(1));
        d2 = sign(pt, GetVertex(1), GetVertex(2));
        d3 = sign(pt, GetVertex(2), GetVertex(0));

        has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(has_neg && has_pos);
    }

    public override bool Equals(object obj)
    {
        if(obj is Triangle triangle)
        {
            for(int i=0; i < 3; i++)
            {
                if (vertexIndeces[i] != triangle.vertexIndeces[i])
                    return false;
            }
            return true;
        }
        return false;
    }
}
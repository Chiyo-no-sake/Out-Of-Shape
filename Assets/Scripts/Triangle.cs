using UnityEngine;
using System;
using System.Collections.Generic;

public class Triangle
{
    public readonly int[] vertexIndeces;
    private readonly SphericalNavMesh father;
    private readonly Vector3 normal;

    public float enemySize = 1;

    public Triangle(int vertexIndex1, int vertexIndex2, int vertexIndex3, SphericalNavMesh father)
    {
        this.father = father;
        vertexIndeces = new int[3];
        vertexIndeces[0] = vertexIndex1;
        vertexIndeces[1] = vertexIndex2;
        vertexIndeces[2] = vertexIndex3;
        normal = Vector3.Cross(GetVertex(1) - GetVertex(0), GetVertex(2) - GetVertex(0)).normalized;
        //collidingRadius = Vector3.Distance((GetVertex(1) + GetVertex(0)) / 2f, GetCenter());
    }

    public Vector3 GetCenter()
    {
        return (GetVertex(0) + GetVertex(1) + GetVertex(2)) / 3f;
    }

    public Vector3 GetVertex(int index)
    {
        return father.GetVertex(vertexIndeces[index]);
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
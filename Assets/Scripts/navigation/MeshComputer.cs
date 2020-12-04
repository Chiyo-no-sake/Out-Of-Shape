using System.Collections;
using UnityEngine;
using static System.Math;

public static class MeshComputer
{
    private static Vector3[] vertices;


    public static void SetVertices(Vector3[] vertices) {
        MeshComputer.vertices = vertices;
    }

    public static Vector3? GetNearestVertexTo(Vector3 vertex)
    {
        if (vertices == null) return null;
        float? minDistance = null;
        Vector3 nearest = vertices[0];

        foreach (var v in vertices)
        {
            float currDist = Vector3.Distance(vertex, v);
            if (minDistance == null || currDist < minDistance)
            {
                minDistance = currDist;
                nearest = v;
            }
        }

        return nearest;
    }
}
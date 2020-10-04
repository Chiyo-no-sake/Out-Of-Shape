using UnityEngine;
public class SphericalMesh
{
    private int detailLevel;
    private int resolution;
    public float radius;
    private Vector3[] vertices;
    private int[] triangles;
    private Triangle[] faces;
    public SphericalMesh(int detailLevel, int radius)
    {
        this.detailLevel = detailLevel;
        this.radius = radius;

        generateMeshData();
        fillVertexesAndTriangles();

        faces = new Triangle[triangles.Length];
        for (int i = 0; i < triangles.Length - 2; i++)
        {
            faces[i] = new Triangle(vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]]);
            // TODO change
            faces[i].enemySize = 1;
        }
    }

    public Triangle[] getTriangles(){
        return faces;
    }

    private void generateMeshData()
    {
        resolution = 1 << detailLevel;
        vertices = new Vector3[(resolution + 1) * (resolution + 1) * 4 -
            (resolution * 2 - 1) * 3];
        triangles = new int[(1 << (detailLevel * 2 + 3)) * 3];
    }

    private static Vector3[] directions = {
        Vector3.left,
        Vector3.back,
        Vector3.right,
        Vector3.forward
    };

    private void fillVertexesAndTriangles()
    {
        int v = 0, vBottom = 0, t = 0;

        for (int i = 0; i < 4; i++)
        {
            vertices[v++] = Vector3.down;
        }

        for (int i = 1; i <= resolution; i++)
        {
            float progress = (float)i / resolution;
            Vector3 from, to;
            vertices[v++] = to = Vector3.Lerp(Vector3.down, Vector3.forward, progress);
            for (int d = 0; d < 4; d++)
            {
                from = to;
                to = Vector3.Lerp(Vector3.down, directions[d], progress);
                t = CreateLowerStrip(i, v, vBottom, t, triangles);
                v = CreateVertexLine(from, to, i, v, vertices);
                vBottom += i > 1 ? (i - 1) : 1;
            }
            vBottom = v - 1 - i * 4;
        }

        for (int i = resolution - 1; i >= 1; i--)
        {
            float progress = (float)i / resolution;
            Vector3 from, to;
            vertices[v++] = to = Vector3.Lerp(Vector3.up, Vector3.forward, progress);
            for (int d = 0; d < 4; d++)
            {
                from = to;
                to = Vector3.Lerp(Vector3.up, directions[d], progress);
                t = CreateUpperStrip(i, v, vBottom, t, triangles);
                v = CreateVertexLine(from, to, i, v, vertices);
                vBottom += i + 1;
            }
            vBottom = v - 1 - i * 4;
        }

        for (int i = 0; i < 4; i++)
        {
            triangles[t++] = vBottom;
            triangles[t++] = v;
            triangles[t++] = ++vBottom;
            vertices[v++] = Vector3.up;
        }

        Vector3[] normals = new Vector3[vertices.Length];
        Normalize(vertices, normals);

        // enlarge vertexes to reach sphere
        if (radius != 1f)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] *= radius;
            }
        }
    }

    private static int CreateVertexLine(
        Vector3 from, Vector3 to, int steps, int v, Vector3[] vertices
    )
    {
        for (int i = 1; i <= steps; i++)
        {
            vertices[v++] = Vector3.Lerp(from, to, (float)i / steps);
        }

        return v;
    }

    private static int CreateLowerStrip(int steps, int vTop, int vBottom, int t, int[] triangles)
    {
        for (int i = 1; i < steps; i++)
        {
            triangles[t++] = vBottom;
            triangles[t++] = vTop - 1;
            triangles[t++] = vTop;

            triangles[t++] = vBottom++;
            triangles[t++] = vTop++;
            triangles[t++] = vBottom;
        }
        triangles[t++] = vBottom;
        triangles[t++] = vTop - 1;
        triangles[t++] = vTop;
        return t;
    }

    private static int CreateUpperStrip(int steps, int vTop, int vBottom, int t, int[] triangles)
    {
        triangles[t++] = vBottom;
        triangles[t++] = vTop - 1;
        triangles[t++] = ++vBottom;
        for (int i = 1; i <= steps; i++)
        {
            triangles[t++] = vTop - 1;
            triangles[t++] = vTop;
            triangles[t++] = vBottom;

            triangles[t++] = vBottom;
            triangles[t++] = vTop++;
            triangles[t++] = ++vBottom;
        }
        return t;
    }

    private static void Normalize(Vector3[] vertices, Vector3[] normals)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            normals[i] = vertices[i] = vertices[i].normalized;
        }
    }

    public void drawGizmos() {
        if(triangles != null){
            for (int i = 0; i < triangles.Length - 1; i += 3)
            {
                Debug.DrawLine(vertices[triangles[i]], vertices[triangles[i + 1]], Color.red,1);
                if (i == triangles.Length - 2)
                {
                    Debug.DrawLine(vertices[triangles[i + 1]], vertices[triangles[0]], Color.red,1);
                }
                else
                {
                    Debug.DrawLine(vertices[triangles[i + 1]], vertices[triangles[i + 2]], Color.red,1);
                }
            }
        }
    }
}
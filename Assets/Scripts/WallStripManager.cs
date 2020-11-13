using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallStripManager : MonoBehaviour
{
    [SerializeField] private GameObject ownerWorld;
    [SerializeField] private GameObject stripPrefab;
    [SerializeField] private float wallHeight = 5;

    // Start is called before the first frame update
    void Start()
    {
        WallMeshNode[] children = GetComponentsInChildren<WallMeshNode>();

        foreach(var child in children)
        {
            initChildRotation(child.gameObject);
        }

        for(int i=0; i<children.Length -1; i++)
        {
            BuildStrip(children[i].gameObject.transform.localPosition, children[i + 1].gameObject.transform.localPosition);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void initChildRotation(GameObject child)
    {
        child.gameObject.transform.up = -GetGravityVectorFor(child.transform.position).normalized;
    }

    private void BuildStrip(Vector3 start, Vector3 end)
    {
        Vector3 tmp_start = start + (-GetGravityVectorFor(start).normalized * wallHeight);
        Vector3 tmp_end = end + (-GetGravityVectorFor(end).normalized * wallHeight);

        float length = Vector3.Distance(tmp_start, tmp_end) / 2.0f;
        Vector3 center = (start + end) / 2.0f;

        GameObject strip = Instantiate(stripPrefab);
        strip.transform.localScale = new Vector3(1, 1, length); // divide by 2 due to blender units conversion 
        strip.transform.localPosition = center;
        strip.transform.LookAt(end, -GetGravityVectorFor(strip.transform.position));
    }

    private void BuildGizmoStrip(Vector3 start, Vector3 end)
    {
        float length = Vector3.Distance(start, end) / 2.0f;

        Gizmos.DrawLine(start, end);
        Gizmos.DrawLine(start + start.normalized*wallHeight/2, end + end.normalized* wallHeight / 2);
        Gizmos.DrawLine(start + start.normalized * wallHeight, end + end.normalized * wallHeight);
    }

    private Vector3 GetGravityVectorFor(Vector3 obj)
    {
        return ownerWorld.transform.position - obj;
    }

    private void OnDrawGizmos()
    {
        WallMeshNode[] children = GetComponentsInChildren<WallMeshNode>();

        foreach (var child in children)
        {
            initChildRotation(child.gameObject);
        }

        for (int i = 0; i < children.Length - 1; i++)
        {
            BuildGizmoStrip(children[i].gameObject.transform.localPosition, children[i + 1].gameObject.transform.localPosition);
        }
    }
}

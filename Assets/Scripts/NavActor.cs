using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.PlayerLoop;
using UnityEditor.ShaderGraph.Internal;

public class Node : IComparable<Node>
{

    public Node parent;
    public Vector3 vertex;
    public Triangle[] fathers;
    public float g; // Distance from start to this
    public float h; // Distance from this to end (circa)

    public Node(Node parent, Vector3 thisVertex, float h)
    {
        this.parent = parent;
        this.vertex = thisVertex;
        this.h = h;
        if (parent == null)
            this.g = 0;
        else
            this.g = parent.g + 1;
    }

    public float f()
    {
        return g + h;
    }

    public int CompareTo(Node next)
    {
        return (int)(this.f() - next.f());
    }

    public override bool Equals(object obj)
    {

        if (obj != null && obj is Node node)
        {
            return node.vertex.Equals(this.vertex);
        }

        return false;
    }
}

[RequireComponent(typeof(Collider))]
public abstract class NavActor : MonoBehaviour
{

    [SerializeField] private float refreshDelay = 0.5f;
    [SerializeField] private GameObject target;
    private SphericalNavMesh navigationSurface;
    private bool updateNavigationPath = true;
    private List<Node> path;
    private int pathStepIndex;

    // to implement in child class
    protected abstract void MoveToTarget(Vector3 targetPoint, Vector3 gravityVector);

    public NavActor()
    {
        this.path = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (path == null) return;
        if (path.ElementAt(0) == null) return;

        Vector3 gravityVector = navigationSurface.transform.localPosition - transform.localPosition;

        Vector3 currVertex = navigationSurface.GetNearestVertex(this.gameObject);

        if(path.ElementAt(pathStepIndex).vertex == currVertex)
            ++pathStepIndex;
        
        this.MoveToTarget(path.ElementAt(pathStepIndex).vertex, gravityVector);
    }

    void FixedUpdate()
    {
        List<Vector3> vertices = new List<Vector3>();
        //only for debug
        path.ForEach(n =>
        {
            vertices.Add(n.vertex);
        });

        SphericalNavMesh.DebugPath(vertices.ToArray(), Color.cyan, this.refreshDelay);

    }


    //delay between fresh starts of the path-seeking algorithm
    IEnumerator delayedCollision(Collision other)
    {
        updateNavigationPath = false;
        navigationSurface = other.gameObject.GetComponent<SphericalNavMesh>();

        Vector3 first = navigationSurface.GetNearestVertex(this.gameObject);
        Vector3 last = navigationSurface.GetNearestVertex(target);

        if (first != null && last != null)
        {
            Vector3 initialPos = first;
            Vector3 endPos = last;
            float initDistance = Vector3.Distance(initialPos, endPos);

            Node start = new Node(null, first, initDistance);
            Node end = new Node(null, last, 0);

            Task.Factory.StartNew(() => {
                path = CreatePath(start, end);
                pathStepIndex = 0;
            });
        }

        
        yield return new WaitForSeconds(this.refreshDelay);
        updateNavigationPath = true;
        
    }

    // Take first and last node and return the path to reach the target
    private List<Node> CreatePath(Node first, Node last)
    {
        Node arrive = GetLast(first, last);
        List<Node> path = new List<Node>();
        List<Node> finalPath = new List<Node>();
        Node curr = arrive;
        while(curr.parent != null)
        {
            path.Add(curr);
            curr = curr.parent;
        };

        for(int i=path.Count-1; i >= 0; i--)
        {
            finalPath.Add(path.ElementAt(i));
        }

        return finalPath;
    }
    

    // Start A* and generate the resulting list of nodes
    // in it, you'll find the node 'last'.
    // this last element is returned with it's parent, so you can regenerate path
    // return null if no path exists
    private Node GetLast(Node first, Node last)
    {
        Vector3 endPos = last.vertex;

        //Debug.Log("A* start");
        List<Node> closedList = new List<Node>();
        List<Node> openList = new List<Node>();

        openList.Add(first);

        while (openList.Count > 0)
        {
            // get the cheapest node in the OL and move it to the closedList
            Node current = GetCheapest(openList);
            closedList.Add(current);
            openList.Remove(current);

            // if we added the destination to the closed list, return this last element
            if (current.Equals(last))
            {
                last.parent = current.parent;
                return last;
            }

            // get all current neighbors
            List<Vector3> neighbors = navigationSurface.GetNeighbors(current.vertex);

            // for each neighbor
            foreach(var t in neighbors)
            {
                // create the corresponding node
                // TODO set traversable accordingly
                Vector3 currPos = t;
                float distanceToEnd = Vector3.Distance(currPos, endPos);
                Node n = new Node(current, t, distanceToEnd);

                if (closedList.Contains(n))
                    continue;
                if (!openList.Contains(n))
                    openList.Add(n);
                else
                {
                    Node inList = openList.Find(node => node.vertex == n.vertex);
                    // t is already in the openList.
                    // check if using current as his parent makes it a better path member
                    // i.e. check:
                    if(current.g + 1 < inList.g)
                    {
                        // if it is, update parent and the g value
                        inList.parent = current;
                        inList.g = current.g + 1;
                    }
                }
            }
        }
        return null;
    }

    private Node GetCheapest(List<Node> openList)
    {
        Node cheapest = null;

        openList.ForEach(n =>
        {
            if (cheapest == null)
            {
                cheapest = n;
            }
            else if (n.f() < cheapest.f())
            {
                cheapest = n;
            }
            else if (n.f() == cheapest.f() && n.h < cheapest.h)
            {
                cheapest = n;
            }
        });

        return cheapest;
    }

    private void OnCollisionStay(Collision other)
    { 
        if (other.gameObject.CompareTag("Ground"))
        {
            if (updateNavigationPath)
            {
                StartCoroutine("delayedCollision", other);
            }
        }
        else
        {
            Debug.Log("Colliding with non ground");
        }
        

    }

}

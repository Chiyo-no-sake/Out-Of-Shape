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
    public Triangle triangle;
    public float g; // Distance from start to this
    public float h; // Distance from this to end (circa)

    public Node(Node parent, Triangle thisNode, float h)
    {
        this.parent = parent;
        this.triangle = thisNode;
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
            return node.triangle.Equals(this.triangle);
        }

        return false;
    }
}

[RequireComponent(typeof(Collider))]
public abstract class NavActor : MonoBehaviour
{

    [SerializeField] private GameObject target;
    private SphericalNavMesh navigationSurface;
    private bool updateNavigationPath = true;
    private List<Node> path;

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
        this.MoveToTarget(path.ElementAt(0).triangle.GetCenter(), gravityVector);
    }

    void FixedUpdate()
    {
        //only for debug
        path.ForEach(n =>
        {
            SphericalNavMesh.DebugVertexes(n.triangle, Color.cyan, 0.1f);
        });
    }


    //delay between fresh starts of the path-seeking algorithm
    IEnumerator delayedCollision(Collision other)
    {
        updateNavigationPath = false;
        navigationSurface = other.gameObject.GetComponent<SphericalNavMesh>();

        Triangle firstTriangle = navigationSurface.GetCollidingTriangle(this.gameObject);
        Triangle lastTriangle = navigationSurface.GetCollidingTriangle(target);

        if (firstTriangle != null && lastTriangle != null)
        {
            Vector3 initialPos = firstTriangle.GetCenter();
            Vector3 endPos = lastTriangle.GetCenter();
            float initDistance = Vector3.Distance(initialPos, endPos);

            Node start = new Node(null, firstTriangle, initDistance);
            Node end = new Node(null, lastTriangle, 0);

            Task.Factory.StartNew(() => path = CreatePath(start, end));
        }

        
        yield return new WaitForSeconds(0.1f);
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
        Vector3 endPos = last.triangle.GetCenter();

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
            List<Triangle> neighbors = navigationSurface.GetNeighbors(current.triangle);

            // for each neighbor
            foreach(var t in neighbors)
            {
                // create the corresponding node
                Vector3 currPos = t.GetCenter();
                float distanceToEnd = Vector3.Distance(currPos, endPos);
                Node n = new Node(current, t, distanceToEnd);

                if (closedList.Contains(n))
                    continue;
                if (!openList.Contains(n))
                    openList.Add(n);
                else
                {
                    Node inList = openList.Find(node => node.triangle.Equals(n.triangle));
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

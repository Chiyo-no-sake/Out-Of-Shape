using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Node : IComparable<Node>{

    final Node parent;
    final Triangle triangle;
    final int g = 1;
    int h = 0;
    int f = g + h;

    Node(Node parent, Triangle thisNode){
      this.parent = parent;
      this.thisNode = triangle;
      update();
    }

    Node(Node parent, Triangle thisNode, int h){
      this.parent = parent;
      this.thisNode = triangle;
      this.h = h;
      update();
    }

    void setH(int newH){
      h = newH;
      update();
    }

    void update(){
      f = g + h;
    }

    int CompareTo(Employee next)
    {
      return next.f.CompareTo(this.f);
    }

    public override bool Equals(object obj)
    {
        if (obj != null && obj is Node)
        {
            return ((Node)obj).ID.Equals(this.ID);
        }
        else return base.Equals(obj);
    }

}

[RequireComponent(typeof(Collider))]
public class NavActor : MonoBehaviour
{

    [SerializeField] private NavActor player;
    private List<Node> path = new List<Node>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //delay between fresh starts of the path-seeking algorithm
    IEnumerator pause(){
      yield return new WaitForSeconds(1);
    }

    //access method for A* algorithm
    private void seekPath(Node first, Node last){

      Debug.Log("A* start");

      List<Node> newPath = new List<Node>();
      List<Node> candidates = new List<Node>();

      candidates.Add(first);

      Node next = first;
      newPath.Add(next);

      while(!next.Equals(last)){

        next = seekNext(next, candidates);
        newPath.Add(next);

      }

      path = newPath;

    }

    private Node seekNext(Node current, List<Node> candidates){

      List<Triangle> neighbors = SphericalNavMesh.getNeighbors(current.triangle);

      //populate new candidates
      foreach(Triangle triangle in neighbors){
        candidates.Add(new Node(current, triangle, 1));
      }

      Node next = current;

      foreach(Node node in candidates){
        if(node.f < next.f)
          next = node;
      }

      return next;

    }

    private void OnCollisionEnter(Collision other)
    {
        // dispatch the collision event on triangles
        StartCoroutine(pause);

        if(other.tag == "Ground")
          seekPath(new Node(null, other.gameObject.getCollidingTriangle(this)),
           new Node(null, other.gameObject.getCollidingTriangle(player)));
        else
          Debug.Log("NavActor colliding with non-ground object");

    }

}

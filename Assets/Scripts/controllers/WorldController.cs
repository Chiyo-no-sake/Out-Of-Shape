using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WorldController : MonoBehaviour
{
    private Vector3 _position;
    private HashSet<GameObject> _bodies;

    public float terrainDensity = 5.5f;
    public float gravity = 9.81f;

    // Start is called before the first frame update
    void Start()
    {
        _position = transform.localPosition;
        _bodies = new HashSet<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        _bodies.RemoveWhere(body => body == null);
        foreach(var body in _bodies) ApplyGravityTo(body);
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.attachedRigidbody != null && !other.attachedRigidbody.isKinematic)
        {
            _bodies.Add(other.gameObject);
        }
    }

    private void ApplyGravityTo(GameObject target)
    {
        try
        {
            Rigidbody targetRb = target.GetComponent<Rigidbody>();

            Vector3 gravityVector = Vector3.Normalize(_position - target.transform.localPosition);

            Vector3 targetPos = target.transform.localPosition;

            gravityVector *= gravity;
            targetRb.AddForce(gravityVector, ForceMode.Acceleration);

        } catch(MissingComponentException)
        {
            return;
        }
    }
}

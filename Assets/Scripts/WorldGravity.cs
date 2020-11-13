using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGravity : MonoBehaviour
{

    private Rigidbody _rigidbody;
    private Collider _atmosphere;
    private Vector3 _position;

    public float terrainDensity = 5.5f;
    public float gravity = 9.81f;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponentsInChildren<Rigidbody>()[0];
        _atmosphere = GetComponent<Collider>();
        _position = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            ApplyGravityTo(other.gameObject);
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

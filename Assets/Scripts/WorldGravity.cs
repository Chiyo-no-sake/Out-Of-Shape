using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGravity : MonoBehaviour
{

    private Rigidbody _rigidbody;
    private Collider _atmosphere;
    private Vector3 _position;

    private float gravityConstant = 0.07f;
    public float terrainDensity = 5.5f;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponentsInChildren<Rigidbody>()[0];
        _atmosphere = GetComponent<Collider>();
        _position = transform.localPosition;

        _rigidbody.mass = 4 * Mathf.PI * Mathf.Pow(transform.GetChild(0).transform.localScale.x, 2) * terrainDensity;
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
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        Vector3 gravityVector = Vector3.Normalize(_position - target.transform.localPosition);

        float m1 = _rigidbody.mass;
        float m2 = targetRb.mass;
        Vector3 targetPos = target.transform.localPosition;
        float dist = Vector3.Distance(_position, targetPos);
        if (dist != 0)
        {
            float gForce = gravityConstant * m1 * m2 / Mathf.Pow(dist, 2);
            gravityVector *= gForce;
            targetRb.AddForce(gravityVector);
        }
    }
}

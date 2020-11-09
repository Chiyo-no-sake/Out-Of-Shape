using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEnemyBehaviour : NavActor
{
    [SerializeField] private float acceleration = 1;
    [SerializeField] private float maxSpeed = 1;
    private Rigidbody _rigidbody;
    private Collider _collider;
    private ParticleSystem _particles;
    CubeEnemyBehaviour()
    {
    }

    private void Start()
    {
        _collider = GetComponent<Collider>();
        _particles = GetComponent<ParticleSystem>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    protected override void MoveToTarget(Vector3 targetPoint, Vector3 gravityVector)
    {
        computeRotation(targetPoint, gravityVector);

        doStep(targetPoint, gravityVector);
    }

    private void computeRotation(Vector3 targetPoint, Vector3 gravityVector)
    {
        Vector3 pos = targetPoint - transform.position;
        var newRot = Quaternion.LookRotation(pos);
        //transform.rotation = Quaternion.Lerp(transform.rotation, newRot, 1 * Time.deltaTime);
    }

    private void doStep(Vector3 targetPoint, Vector3 gravityVector)
    {
        Vector3 fwd = (targetPoint - transform.position).normalized;
        Vector3 up = -gravityVector.normalized;

        Vector3 applyPoint = transform.position - fwd * transform.localScale.z + up * transform.localScale.y;

        _rigidbody.AddForceAtPosition(fwd * acceleration * Time.deltaTime * 10000, applyPoint, ForceMode.Force);
        preventExceedMaxSpeed();
    }

    private void preventExceedMaxSpeed()
    {
        _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, maxSpeed);
    }
}

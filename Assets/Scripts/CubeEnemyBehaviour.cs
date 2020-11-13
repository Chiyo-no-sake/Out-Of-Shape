using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CubeEnemyBehaviour : NavActor
{
    [SerializeField] private ParticleSystem collisionParticles;
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private float acceleration = 1;
    [SerializeField] private float maxSpeed = 1;
    private Rigidbody _rigidbody;
    private ParticleSystem _particles;

    void Start()
    {
        health = 5;
        _rigidbody = GetComponent<Rigidbody>();
        _particles = Instantiate(collisionParticles);
    }

    // Start is called before the first frame update

    protected void doStep(Vector3 targetPoint, Vector3 gravityVector)
    {
        Vector3 fwd = (targetPoint - transform.position).normalized;
        Vector3 up = -gravityVector.normalized;

        Vector3 applyPoint = transform.position - fwd * transform.localScale.z + up * transform.localScale.y;
        if (acceleration > maxSpeed) acceleration = maxSpeed;
        _rigidbody.AddForceAtPosition(fwd * acceleration * Time.deltaTime * 1000, applyPoint, ForceMode.Impulse);
        preventExceedMaxSpeed();
    }

    protected void preventExceedMaxSpeed()
    {
        _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, maxSpeed);
    }

    new protected void OnCollisionEnter(Collision collision)
    {

        base.OnCollisionEnter(collision);

        Vector3 contact = collision.contacts[0].point;
        _particles.transform.LookAt(transform.position);
        _particles.transform.position = contact;
        _particles.Play();
    }

    new protected void OnCollisionStay(Collision collision)
    {
        base.OnCollisionStay(collision);

        Vector3 contact = collision.contacts[0].point;
        _particles.transform.LookAt(transform.position);
        _particles.transform.position = contact;
    }

    new protected void OnCollisionExit(Collision collision)
    {
        base.OnCollisionExit(collision);
        _particles.Stop();
    }

    protected override void StepTowardsTarget()
    {
        if(getPath() == null) return;
        Node target = GetNextPathPoint();
        Vector3 gravityDir = (currentPlanet.transform.position - transform.position).normalized;
        doStep(target.vertex, gravityDir);
        UpdateNextPathPoint();
    }

    protected override bool IsTargetReached()
    {
        if (getPath() == null) return false;
        return false;
    }

    protected override void OnTargetReached()
    {
        return;
    }

    public override void DestroySelf(){

        Destroy(_particles);

        ParticleSystem deathParticles = Instantiate(this.deathParticles);
        deathParticles.transform.LookAt(transform.position);
        deathParticles.transform.position = transform.position;
        Destroy(deathParticles, 1);
        Destroy(transform.root.gameObject);

    }

}

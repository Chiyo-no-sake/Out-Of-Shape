using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CubeEnemy : AIEnemy, IAttacker
{
    [SerializeField] private ParticleSystem deathParticles = null;
    [SerializeField] private float speed = 1;
    private Rigidbody _rigidbody;

    void Start()
    {
        health = 5;
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void OnHit(LivingEntity other)
    {

    }

    public void OnKill(LivingEntity other)
    {

    }

    public void Attack() { }

    public bool IsHostileTo(WorldEntity other) {
        return GetTeamNumber() != other.GetTeamNumber();
    }

    protected void DoStep(Vector3 targetPoint, Vector3 gravityVector)
    {
        Vector3 fwd = (targetPoint - transform.position).normalized;
        Vector3 up = -gravityVector.normalized;

        Vector3 applyPoint = transform.position - fwd * transform.localScale.z + up * transform.localScale.y;

        float force = speed;

        _rigidbody.AddForceAtPosition(force*fwd / Time.fixedDeltaTime, applyPoint, ForceMode.Force);
    }

    protected override void StepTowardsTarget()
    {
        if(GetPath() == null) return;
        Node target = GetNextPathPoint();
        Vector3 gravityDir = (currentPlanet.transform.position - transform.position).normalized;
        DoStep(target.vertex, gravityDir);
        UpdateNextPathPoint();
    }

    protected override bool IsTargetReached()
    {
        if (GetPath() == null) return false;
        return false;
    }

    protected override void OnTargetReached()
    {
        return;
    }

    public override void DestroySelf(){
        ParticleSystem deathParticles = Instantiate(this.deathParticles);
        deathParticles.transform.LookAt(transform.position);
        deathParticles.transform.position = transform.position;


        Destroy(deathParticles, 3);
        Destroy(transform.root.gameObject);
    }

}

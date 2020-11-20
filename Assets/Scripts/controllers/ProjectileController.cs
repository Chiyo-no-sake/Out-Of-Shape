using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProjectileController : DamageSource
{

    [SerializeField] private ParticleSystem collisionParticles = null;
    private float _speed = 1;
    private Vector3 _center = new Vector3(0, 0, 0);
    private Vector3 _rotAxis = new Vector3(1,0,0);

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(_center, _rotAxis, Mathf.Lerp(0,360,(_speed*Time.deltaTime/10)));
    }

    public void SetRotAxis(Vector3 direction)
    {
        _rotAxis = direction;
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetCenter(Vector3 center)
    {
        _center = center;
    }

    public override void OnHit(LivingEntity other)
    {
        base.OnHit(other);
        CastHitParticles(transform.up);
        Destroy(gameObject);
    }

    public override void OnWallHit()
    {
        base.OnWallHit();
        CastHitParticles(-transform.up);
        Destroy(gameObject);
    }

    private void CastHitParticles(Vector3 lookDirection)
    {
        ParticleSystem particles = Instantiate(collisionParticles);
        particles.transform.position = transform.position - transform.up.normalized ;
        particles.transform.rotation = Quaternion.LookRotation(lookDirection);
        Destroy(particles, 2);
    }

    

}

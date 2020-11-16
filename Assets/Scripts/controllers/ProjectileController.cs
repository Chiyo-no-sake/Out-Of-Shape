using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{

    [SerializeField] private ParticleSystem collisionParticles = null;
    [SerializeField] private int damage = 1;

    private float _speed = 1;
    private Vector3 _center = new Vector3(0, 0, 0);
    private Vector3 _rotAxis = new Vector3(1,0,0);
    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(_center, _rotAxis, Mathf.Lerp(0,360,(_speed*Time.deltaTime/10)));
    }

    public void setRotAxis(Vector3 direction)
    {
        _rotAxis = direction;
    }

    public void setSpeed(float speed)
    {
        _speed = speed;
    }

    public void setCenter(Vector3 center)
    {
        _center = center;
    }

    private void castHitParticles(Vector3 lookDirection)
    {
        ParticleSystem particles = Instantiate(collisionParticles);
        particles.transform.position = transform.position - transform.up.normalized ;
        particles.transform.rotation = Quaternion.LookRotation(lookDirection);
        Destroy(particles, 2);
    }

    void OnTriggerEnter(Collider other){
        Debug.Log("colliding with " + other.gameObject);
        LivingEntity[] entities = other.gameObject.GetComponentsInChildren<LivingEntity>();

        foreach(var entity in entities){
            entity.TakeHit(damage);
            castHitParticles(transform.up);
        }

        if(entities.Length == 0)
        {
            castHitParticles(-transform.up);
        }

        Destroy(gameObject);

    }

}

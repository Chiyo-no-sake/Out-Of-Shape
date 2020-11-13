using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{

    [SerializeField] private ParticleSystem collisionParticles;
    [SerializeField] private Vector3 _center;
    [SerializeField] private float _speed = 1;
    [SerializeField] private Vector3 _rotAxis;
    [SerializeField] private int damage;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(_center, _rotAxis, _speed * Time.deltaTime);
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

    void OnCollisionEnter(Collision other){

        LivingEntity[] entities = other.gameObject.GetComponentsInChildren<LivingEntity>();

        foreach(var entity in entities){
          ParticleSystem particles = Instantiate(collisionParticles);
          particles.transform.position = transform.position;
          particles.transform.RotateAroundLocal(transform.right, 90);
          entity.TakeHit(damage);
          Destroy(particles, 1);
        }

        Debug.Log("colliding with " + other.gameObject);

        Destroy(gameObject);

    }

}

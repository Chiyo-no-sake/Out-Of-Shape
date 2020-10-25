using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{

    [SerializeField] private Vector3 _center;
    [SerializeField] private float _speed = 1;
    [SerializeField] private Vector3 _rotAxis;

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

}

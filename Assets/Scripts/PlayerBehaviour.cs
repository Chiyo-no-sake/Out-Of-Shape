using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    
    private Vector3 _position;
    private Rigidbody _rigidbody;
    public float acceleration = 2;
    public float maxSpeed = 10;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _position = transform.localPosition;
        handleInput();
        preventExceedMaxSpeed();
    }

    private void OnTriggerStay(Collider other) {
        if(other.tag == "Atmosphere"){
            applyRotation(other.gameObject);
        }
    }

    private void applyRotation(GameObject atmosphereOwner) {
        Vector3 gravityVector = _position - atmosphereOwner.transform.localPosition;
        transform.localRotation = Quaternion.FromToRotation(Vector3.up, gravityVector);
    }

    private void handleInput(){
        Vector3 accelVec = new Vector3();
        if(Input.GetKey(KeyCode.W)){
            accelVec.z += acceleration;
        }
        if(Input.GetKey(KeyCode.S)){
            accelVec.z -= acceleration;
        }
        if(Input.GetKey(KeyCode.A)){
            accelVec.x -= acceleration;
        }
        if(Input.GetKey(KeyCode.D)){
            accelVec.x += acceleration;
        }

        _rigidbody.AddRelativeForce(accelVec, ForceMode.Acceleration);
    }

    private void preventExceedMaxSpeed(){
        _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, maxSpeed);
    }
}

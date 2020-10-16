using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEntity : MonoBehaviour
{
    private Vector3 _position;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _position = transform.localPosition;  
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Atmosphere")
        {
            applyRotation(other.gameObject);
        }
    }

    private void applyRotation(GameObject atmosphereOwner)
    {
        Vector3 gravityVector = atmosphereOwner.transform.localPosition - _position;
        transform.localRotation = Quaternion.LookRotation(Vector3.Cross(gravityVector, transform.right), -gravityVector);
    }
}

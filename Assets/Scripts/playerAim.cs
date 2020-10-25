using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAim : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Camera cam = Camera.main;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Vector3.Distance(transform.position, cam.transform.localPosition);

        //NON FUNZIONA PORCO DIO
        Vector3 toLookPoint = cam.ScreenToWorldPoint(mousePos);
        Debug.DrawRay(transform.position, toLookPoint - transform.position);
        transform.localRotation = Quaternion.LookRotation(toLookPoint - transform.position);
    }
}

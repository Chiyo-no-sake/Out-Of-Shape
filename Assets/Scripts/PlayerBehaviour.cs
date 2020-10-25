using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviour : MonoBehaviour
{
    public GameObject playerMesh;
    public float acceleration = 2;
    public float maxSpeed = 10;
    public GameObject projectileType;
    public float projectileSpeed;
    public float projectileLife;
    public float fireRatio;
    
    private Camera cam;
    private GameObject currentPlanet;
    private Rigidbody _rigidbody;
    private Vector3 _position;
    private Vector3 _aimDirection;
    private bool isShooting = false;
    private float timer = 0;
    private float animationSpeed = 0;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Animator[] animators = GetComponentsInChildren<Animator>();

        foreach (var anim in animators)
        {
            if (anim.tag.Equals("PlayerRotationCenter"))
                animator = anim;
        }

        if(playerMesh == null)
        {
            Debug.LogError("Provide a mesh to the Player object");
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer = (timer + Time.deltaTime);
        _position = transform.localPosition;
        handleInput();
        preventExceedMaxSpeed();
        computeAim();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Atmosphere")
        {
            currentPlanet = other.gameObject;
            applyRotation(currentPlanet);
        }
    }

    private void computeAim()
    {
        cam = Camera.main;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Vector3.Distance(currentPlanet.transform.position, transform.position);

        Vector3 toLookPoint = cam.ScreenToWorldPoint(mousePos);

        Vector3 toLookVector = Vector3.ProjectOnPlane(toLookPoint, transform.up);
        //Debug.DrawRay(_position, toLookVector);
        playerMesh.transform.rotation = Quaternion.LookRotation(Vector3.Lerp(playerMesh.transform.forward, toLookVector, 2 * Time.deltaTime), transform.up);

        _aimDirection = toLookVector.normalized;
    }

    private void shoot()
    {
        GameObject p = Instantiate(projectileType);
        ProjectileBehaviour pb = p.GetComponent<ProjectileBehaviour>();

        p.transform.position = playerMesh.transform.position;
        p.transform.rotation = Quaternion.LookRotation(playerMesh.transform.up, playerMesh.transform.forward);
        pb.setCenter(currentPlanet.transform.localPosition);
        pb.setRotAxis(playerMesh.transform.right);
        pb.setSpeed(projectileSpeed);

        Destroy(p, projectileLife);
    }


    private void applyRotation(GameObject atmosphereOwner)
    {
        Vector3 gravityVector = _position - atmosphereOwner.transform.localPosition;
        transform.localRotation = Quaternion.LookRotation(Vector3.Cross(-gravityVector, transform.right), gravityVector);
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

        if (Input.GetMouseButton(0)) {
            isShooting = true;
        }
        else {
            isShooting = false;
        }

        if (isShooting)
        {
            if (timer > fireRatio)
            {
                timer = 0;
                shoot();
            }
        }

        _rigidbody.AddRelativeForce(accelVec * Time.deltaTime*10000, ForceMode.Force);
    }

    private void preventExceedMaxSpeed(){
        _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, maxSpeed);
    }
}

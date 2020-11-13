using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

enum WeaponState
{
    IDLE, CHARGING, SHOOTING, DISCHARGING
}

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerBehaviour : WorldEntity
{
    // ############## Serial #######################
    // Movement and phisics
    [SerializeField] private float acceleration = 2;
    [SerializeField] private float maxSpeed = 10;
    [SerializeField] private GameObject playerMesh;
    
    // Shooting and Animation
    [SerializeField] private GameObject projectileType;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileLife;
    [SerializeField] private float minShootingDelta = 0.2f;
    [SerializeField] private float maxShootingDelta = 1;
    [SerializeField] private float maxAnimationSpeed = 10;
    [SerializeField] private float chargingSpeed = 5;

    // ############### Private #####################
    //physics and movement
    private Camera cam;
    private Rigidbody _rigidbody;
    private Vector3 _position;

    // shooting
    private WeaponState weaponState = WeaponState.IDLE;
    private float timer;
    private float shootingDelta;
    
    // animations
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        shootingDelta = maxShootingDelta;
        timer = maxShootingDelta;
        _rigidbody = GetComponent<Rigidbody>();

        // search for animator for rotation
        animator = transform.Find("PlayerMesh/playerMesh/RotationCenter").GetComponent<Animator>();
        if(animator == null)
        {
            Debug.LogError("Cannot find animator");
        }
        else
        {
            animator.speed = 0;
        }

        if(playerMesh == null)
        {
            Debug.LogError("Provide a mesh to the Player object");
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        _position = transform.localPosition;
        handleInput();
        preventExceedMaxSpeed();
        computeAim();
        computeWeaponState();
    }

    private void computeWeaponState()
    {
        if (weaponState == WeaponState.IDLE)
            if (Input.GetMouseButton(0))
                weaponState = WeaponState.CHARGING;
        if (weaponState == WeaponState.CHARGING)
        {
            chargeWeapon();
            weaponShootWithDelta();

            if (!Input.GetMouseButton(0))
            {
                weaponState = WeaponState.DISCHARGING;
                return;
            }
        }
        if (weaponState == WeaponState.DISCHARGING)
        {
            if (Input.GetMouseButton(0))
            {
                weaponState = WeaponState.CHARGING;
                return;
            }

            dischargeWeapon();
        }
    }

    private void weaponShootWithDelta()
    {
        if(timer >= shootingDelta)
        {
            shoot();
            timer = 0;
        }
    }

    private void chargeWeapon()
    {
        animator.speed = Mathf.Lerp(animator.speed, maxAnimationSpeed, chargingSpeed/2 * Time.deltaTime);
        shootingDelta = Mathf.Lerp(shootingDelta, minShootingDelta, chargingSpeed * Time.deltaTime);
    }

    private void dischargeWeapon()
    {
        animator.speed = Mathf.Lerp(animator.speed, 0, chargingSpeed * Time.deltaTime);
        shootingDelta = Mathf.Lerp(shootingDelta, maxShootingDelta, chargingSpeed * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Atmosphere")
        {
            currentPlanet = other.gameObject;
            applyRotation();
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


    private void applyRotation()
    {
        Vector3 gravityVector = _position - currentPlanet.transform.localPosition;
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

        _rigidbody.AddRelativeForce(accelVec * Time.deltaTime*10000, ForceMode.Force);
    }

    private void preventExceedMaxSpeed(){
        _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, maxSpeed);
    }
}

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
public class PlayerController : LivingEntity, IAttacker
{
    // ############## Serial #######################
    // Movement and phisics
    [SerializeField] private float speed = 1;
    [SerializeField] private GameObject playerMesh;
    
    // Shooting and Animation
    [SerializeField] private GameObject projectileType = null;
    [SerializeField] private float projectileSpeed = 50.0f;
    [SerializeField] private float projectileLife = 1.0f;
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
    void LateUpdate()
    {
        timer += Time.deltaTime;
        _position = transform.localPosition;
        computeAim();
        computeWeaponState();
    }

    private void FixedUpdate()
    {
        handleInput();
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
            Attack();
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
        playerMesh.transform.rotation = Quaternion.LookRotation(Vector3.LerpUnclamped(playerMesh.transform.forward, toLookVector, 2 * Time.deltaTime), transform.up);

    }

    public void Attack()
    {
        GameObject projectile = Instantiate(projectileType);
        ProjectileController pc = projectile.GetComponent<ProjectileController>();

        projectile.transform.position = playerMesh.transform.position + playerMesh.transform.forward.normalized * 2;
        projectile.transform.rotation = Quaternion.LookRotation(playerMesh.transform.up, playerMesh.transform.forward);

        pc.SetCaster(this);
        pc.SetCenter(currentPlanet.transform.position);
        pc.SetRotAxis(playerMesh.transform.right);
        pc.SetSpeed(projectileSpeed);

        Destroy(projectile, projectileLife);
    }

    public void OnHit(LivingEntity other)
    {

    }

    public override void DestroySelf()
    {
        Debug.Log("TODO: sei morto");
    }

    public void OnKill(LivingEntity other)
    {
        GameObject.Find("CameraController").GetComponent<CameraController>().ShakeCamera();
    }

    public bool IsHostileTo(WorldEntity other)
    {
        return other.GetTeamNumber() != GetTeamNumber();
    }

    private void applyRotation()
    {
        Vector3 gravityVector = _position - currentPlanet.transform.localPosition;
        transform.localRotation = Quaternion.LookRotation(Vector3.Cross(-gravityVector, transform.right), gravityVector);
    }

    private void handleInput(){
        Vector3 forceDirection;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        forceDirection = Vector3.ClampMagnitude(new Vector3(h, 0, v), 1);

        _rigidbody.AddRelativeForce(speed * forceDirection / Time.fixedDeltaTime, ForceMode.Force);
    }
}

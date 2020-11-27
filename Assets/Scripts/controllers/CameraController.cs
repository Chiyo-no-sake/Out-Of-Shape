using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject boundGameObjectTP = null;
    [SerializeField] private GameObject boundGameObjectFP = null;
    [SerializeField] private ShakePreset enemyDeathShakePreset = null;
    [SerializeField] private ShakePreset playerHitShakePreset = null;
    [SerializeField] private Vector3 thirdPersonOffset = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 firstPersonOffset = new Vector3(0, 0, 0);
    [SerializeField] private float cameraDrag = 1;
    [SerializeField] private float cameraSensibility = 3;

    private Vector3 velocity = Vector3.zero;
    private float xRotation = 0;
    private float yRotation = 0;

    private GameObject mainCamera;
    private GameObject fpCamera;
    private GameObject tpCamera;



    // Start is called before the first frame update
    void Start()
    {
        LookForCameras();
        mainCamera = tpCamera;
    }

    void Update()
    {
        UpdateMainCamera();
        UpdateCamerasTransform();
    }

    private void UpdateMainCamera()
    {

        if (Input.GetKeyDown(KeyCode.Y))
        {

            mainCamera.SetActive(false);
            mainCamera = (mainCamera == tpCamera) ? fpCamera : tpCamera;
            Canvas hudCanvas = GameObject.Find("HUD").GetComponent<Canvas>();
            hudCanvas.worldCamera = GetMainCamera();
            hudCanvas.planeDistance = (mainCamera == tpCamera) ? 10 : 0.5f;

        }

        mainCamera.SetActive(true);

    }

    public void OnPlayerHit()
    {
        Shaker cameraShaker = GetMainCamera().GetComponent<Shaker>();

        cameraShaker.Shake(playerHitShakePreset);

    }

    public void OnEnemyHit()
    {

        Shaker cameraShaker = GetMainCamera().GetComponent<Shaker>();

        cameraShaker.Shake(enemyDeathShakePreset);

    }

    public void UpdateCamerasTransform()
    {
        //first person

        Transform fpCameraContainer = fpCamera.transform;   

        fpCameraContainer.position = boundGameObjectFP.transform.position;
        fpCameraContainer.rotation = boundGameObjectFP.transform.rotation;


        //third person
        Transform tpCameraContainer = tpCamera.transform;

        tpCameraContainer.position = Vector3.SmoothDamp(
            tpCameraContainer.position, 
            boundGameObjectTP.transform.TransformPoint(thirdPersonOffset), 
            ref velocity, 
            cameraDrag);
       
        tpCameraContainer.rotation = Quaternion.LookRotation(
                Vector3.LerpUnclamped(tpCameraContainer.forward, -boundGameObjectTP.transform.up, 0.25f),
                boundGameObjectTP.transform.forward);
    }

    private void LookForCameras()
    {

        fpCamera = transform.Find("CameraContainer_FP").gameObject;
        tpCamera = transform.Find("CameraContainer_TP").gameObject;
        fpCamera.SetActive(false);
        tpCamera.SetActive(false);
    }

    public Camera GetMainCamera()
    {
        return mainCamera.GetComponentInChildren<Camera>();
    }

    public void RotateTowardsMouse(GameObject go)
    {
        if (boundGameObjectTP.GetComponent<PlayerController>().GetCurrentPlanet() == null)
            return;


        if (mainCamera == tpCamera)
        {

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Vector3.Distance(boundGameObjectTP.GetComponent<PlayerController>().GetCurrentPlanet().transform.position, boundGameObjectTP.transform.position);
            
            Vector3 toLookPoint = GetMainCamera().ScreenToWorldPoint(mousePos);
            Vector3 toLookVector = Vector3.ProjectOnPlane(toLookPoint, boundGameObjectTP.transform.up);

            go.transform.rotation = Quaternion.LookRotation(Vector3.LerpUnclamped(go.transform.forward, toLookVector, 2 * Time.deltaTime), boundGameObjectTP.transform.up);
        }

        if (mainCamera == fpCamera)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            xRotation -= mouseY * cameraSensibility;
            xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);

            yRotation += mouseX * cameraSensibility;

            go.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0.0f);
        }
    }

    public Vector3 GetForwardDirection()
    {
        if(mainCamera == tpCamera)
        {
            return boundGameObjectTP.transform.forward.normalized;
        }
      
        else
        {
            return Vector3.ProjectOnPlane(boundGameObjectFP.transform.forward, boundGameObjectTP.transform.up).normalized;
        }

    }

    public Vector3 GetRightDirection()
    {
        if (mainCamera == tpCamera)
        {
            return boundGameObjectTP.transform.right.normalized;
        }

        else
        {
            return Vector3.ProjectOnPlane(boundGameObjectFP.transform.right, boundGameObjectTP.transform.up).normalized;
        }
    }

}

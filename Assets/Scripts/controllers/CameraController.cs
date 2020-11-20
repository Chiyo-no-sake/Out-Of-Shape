using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject boundGameObject = null;
    [SerializeField] private ShakePreset enemyDeathShakePreset = null;
    [SerializeField] private Vector3 thirdPersonOffset = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 firstPersonOffset = new Vector3(0, 0, 0);
    [SerializeField] private float cameraDrag = 1;

    private Vector3 velocity = Vector3.zero;

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
        UpdateCamerasPosition();
    }

    private void UpdateMainCamera()
    {

        if (Input.GetKeyDown(KeyCode.Y))
        {

            mainCamera.SetActive(false);
            mainCamera = (mainCamera == tpCamera) ? fpCamera : tpCamera;
            mainCamera.SetActive(true);

        }

    }
    public void ShakeCamera()
    {
        Shaker cameraShaker = GetComponentInChildren<Shaker>();
        cameraShaker.Shake(enemyDeathShakePreset);
    }

    public void UpdateCamerasPosition()
    {
        //first person

        Transform fpCameraContainer = fpCamera.transform;

        fpCameraContainer.position = boundGameObject.transform.position;
        fpCameraContainer.rotation = boundGameObject.transform.rotation;


        //third person
        Transform tpCameraContainer = tpCamera.transform;

        tpCameraContainer.position = Vector3.SmoothDamp(
            tpCameraContainer.position, 
            boundGameObject.transform.TransformPoint(thirdPersonOffset), 
            ref velocity, 
            cameraDrag);

        Debug.DrawLine(new Vector3(0, 0, 0), boundGameObject.transform.TransformPoint(thirdPersonOffset));
       
        tpCameraContainer.rotation = Quaternion.LookRotation(
                Vector3.LerpUnclamped(tpCameraContainer.forward, -boundGameObject.transform.up, 0.25f),
                boundGameObject.transform.forward);
    }

    private void LookForCameras()
    {

        fpCamera = transform.Find("CameraContainer_FP").gameObject;
        tpCamera = transform.Find("CameraContainer_TP").gameObject;

    }

    public Camera GetMainCamera()
    {
        return mainCamera.GetComponentInChildren<Camera>();
    }

    public void RotateTowardsMouse(GameObject go)
    {
        if (boundGameObject.GetComponent<PlayerController>().GetCurrentPlanet() == null)
            return;


        if (mainCamera == tpCamera)
        {

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Vector3.Distance(boundGameObject.GetComponent<PlayerController>().GetCurrentPlanet().transform.position, boundGameObject.transform.position);
            
            Vector3 toLookPoint = GetMainCamera().ScreenToWorldPoint(mousePos);
            Vector3 toLookVector = Vector3.ProjectOnPlane(toLookPoint, boundGameObject.transform.up);

            go.transform.rotation = Quaternion.LookRotation(Vector3.LerpUnclamped(go.transform.forward, toLookVector, 2 * Time.deltaTime), boundGameObject.transform.up);
        }

        if (mainCamera == fpCamera)
        {

            Quaternion playerTransform = boundGameObject.transform.rotation;

            float rotateX = Input.GetAxis("Mouse X") *  Time.deltaTime;
            float rotateY = Input.GetAxis("Mouse Y") * Time.deltaTime;

            go.transform.Rotate(new Vector3(rotateX, rotateY, 0), Space.Self);
            
        }
    }

}

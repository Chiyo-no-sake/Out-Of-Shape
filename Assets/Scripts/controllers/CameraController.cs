using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject boundGameObject = null;
    [SerializeField] private ShakePreset enemyDeathShakePreset = null;
    [SerializeField] private Vector3 thirdPersonOffset = new Vector3(0, 0, 0);
    [SerializeField] private float cameraDrag = 1;

    private Vector3 velocity = Vector3.zero;

    private GameObject mainCamera;
    // Start is called before the first frame update
    void Start()
    {
    }

    // LateUpdate is called once always after Render
    void FixedUpdate()
    {
        UpdateCamerasPosition();
    }

    public void ShakeCamera()
    {
        Shaker cameraShaker = GetComponentInChildren<Shaker>();
        cameraShaker.Shake(enemyDeathShakePreset);
    }

    public void UpdateCamerasPosition()
    {
        Transform cameraContainer = transform.Find("CameraContainer_TP");
        cameraContainer.position = Vector3.SmoothDamp(
            cameraContainer.position, 
            boundGameObject.transform.TransformPoint(thirdPersonOffset), 
            ref velocity, 
            cameraDrag);

        Debug.DrawLine(new Vector3(0, 0, 0), boundGameObject.transform.TransformPoint(thirdPersonOffset));
        //cameraContainer.LookAt(boundGameObject.transform, boundGameObject.transform.forward);
        cameraContainer.rotation = Quaternion.LookRotation(
                Vector3.LerpUnclamped(cameraContainer.forward, -boundGameObject.transform.up, 0.25f),
                boundGameObject.transform.forward);
    }
}

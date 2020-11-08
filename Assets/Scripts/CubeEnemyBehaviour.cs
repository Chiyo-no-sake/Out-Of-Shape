using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeEnemyBehaviour : NavActor
{
    [SerializeField] private float stepSpeed = 1;
    private Rigidbody rigidbody;
    CubeEnemyBehaviour()
    {
    }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    protected override void MoveToTarget(Vector3 targetPoint, Vector3 gravityVector)
    {
        computeRotation(targetPoint, gravityVector);

        doStep(targetPoint, gravityVector);
    }

    private void computeRotation(Vector3 targetPoint, Vector3 gravityVector)
    {
        Vector3 pos = targetPoint - transform.position;
        var newRot = Quaternion.LookRotation(pos);
        //transform.rotation = Quaternion.Lerp(transform.rotation, newRot, 1 * Time.deltaTime);
    }

    private void doStep(Vector3 targetPoint, Vector3 gravityVector)
    {
        Vector3 fwd = (targetPoint - transform.position).normalized;
        Vector3 up = -gravityVector.normalized;

        Vector3 applyPoint = transform.position - fwd * transform.localScale.z + up * transform.localScale.y;

        //rigidbody.AddTorque(Vector3.Cross(up, fwd) * stepSpeed, ForceMode.Impulse);
        rigidbody.AddForceAtPosition(fwd * stepSpeed, applyPoint, ForceMode.Impulse);
        //transform.RotateAround(pivot, Vector3.Cross(up, fwd), Mathf.LerpAngle(0,90, stepSpeed*Time.deltaTime));
    }

}

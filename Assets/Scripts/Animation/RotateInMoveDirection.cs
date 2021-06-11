using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateInMoveDirection : MonoBehaviour
{
    public TDInputMovement movement;
    [SerializeField] private float rotationRate;

    [SerializeField] private float maxAngle;
    private float targetAngle;
    private float currentAngle;

    private bool atTargetRot;
    private float smoothRot;


    private void Awake()
    {
        movement.OnNewMoveDirection += SetTargetAngle;
    }
    public void SetTargetAngle(Vector2 newDirection)
    {
        Vector2 resultant = newDirection - (Vector2)transform.parent.up;
        float newAngle = EssoUtility.GetAngleFromVector((resultant.normalized));
        if (newAngle < 0) newAngle += 360f;
        targetAngle = Mathf.Clamp(newAngle, -maxAngle, maxAngle);
        transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);//update angle
        //if (currentAngle != targetAngle) atTargetRot = false;
    }

    //public void Update()
    //{
    //    if (!atTargetRot)
    //    {
    //        currentAngle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle, ref smoothRot, rotationRate);//rotate player smoothly to target angle
    //        transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);//update angle
    //        if (currentAngle == targetAngle) atTargetRot = true;
    //    }
    //}

    public void OnDestroy()
    {
        movement.OnNewMoveDirection -= SetTargetAngle;
    }
}

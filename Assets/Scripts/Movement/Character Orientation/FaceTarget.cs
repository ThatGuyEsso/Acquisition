using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceTarget : MonoBehaviour
{
    [SerializeField] private float rotationRate;
    [SerializeField] private Transform target;

    private float smoothRot;

    public void SetTarget(Transform newTarget) { target = newTarget; }
    public void FaceCurrentTarget()
    {
        if (target != false)
        {
     
            Vector2 toVCursor = target.position - transform.position;
            float targetAngle = Mathf.Atan2(toVCursor.y, toVCursor.x) * Mathf.Rad2Deg;//get angle to rotate
          
                               //if (targetAngle < 0) targetAngle += 360f;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle, ref smoothRot, rotationRate);//rotate player smoothly to target angle
            transform.rotation = Quaternion.Euler(0f, 0f, angle);//update angle
        }
        
     

    }
    public void SetRotationRate(float rate) { rotationRate = rate; }

    public void FaceCurrentTarget(float offset)
    {
        if (target != false)
        {

            Vector2 toVCursor = target.position - transform.position;
            float targetAngle = Mathf.Atan2(toVCursor.y, toVCursor.x) * Mathf.Rad2Deg;//get angle to rotate

            //if (targetAngle < 0) targetAngle += 360f;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle+offset, ref smoothRot, rotationRate);//rotate player smoothly to target angle
            transform.rotation = Quaternion.Euler(0f, 0f, angle);//update angle
        }



    }

}

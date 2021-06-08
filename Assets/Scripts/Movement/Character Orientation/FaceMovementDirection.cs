using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceMovementDirection : MonoBehaviour
{
    [SerializeField] private float rotationRate;


    private float smoothRot;

  
    public void SmoothRotToMovement(Vector3 direction)
    {

        float targetAngle = EssoUtility.GetAngleFromVector((direction.normalized));
        /// turn offset -Due to converting between forward vector and up vector
        if (targetAngle < 0) targetAngle += 360f;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle, ref smoothRot, rotationRate);//rotate player smoothly to target angle
        transform.rotation = Quaternion.Euler(0f, 0f, angle);//update angle




    }

}

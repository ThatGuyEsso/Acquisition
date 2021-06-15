using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCursor : MonoBehaviour,ICharacterComponents
{
    [SerializeField] private Camera activeCamera;
    [SerializeField] private float rotationRate;
    [SerializeField] private Transform virtualCursor;
    [SerializeField] private bool useVirtualCursor;
    private float smoothRot;
    private bool isEnabled = true;

    public void FacePointer()
    {
        if (useVirtualCursor)
        {
            Vector2 toVCursor = virtualCursor.position - transform.position;
            float targetAngle = Mathf.Atan2(toVCursor.y, toVCursor.x) * Mathf.Rad2Deg;//get angle to rotate
            targetAngle -= 90f;// turn offset -Due to converting between forward vector and up vector
                               //if (targetAngle < 0) targetAngle += 360f;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle, ref smoothRot, rotationRate);//rotate player smoothly to target angle
            transform.rotation = Quaternion.Euler(0f, 0f, angle);//update angle
        }
        else
        {
            float targetAngle = Mathf.Atan2(EssoUtility.GetVectorToPointer(activeCamera, transform.position).y, EssoUtility.GetVectorToPointer(activeCamera, transform.position).x) * Mathf.Rad2Deg;//get angle to rotate
            targetAngle -= 90f;// turn offset -Due to converting between forward vector and up vector
                               //if (targetAngle < 0) targetAngle += 360f;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle, ref smoothRot, rotationRate);//rotate player smoothly to target angle
            transform.rotation = Quaternion.Euler(0f, 0f, angle);//update angle

        }

    }

    public void LateUpdate()
    {
        if (isEnabled)
        {
            FacePointer();
        }

    }

    public void ResetComponent()
    {
        //
    }
    public void DisableComponent()
    {
        isEnabled = false;
    }

    public void EnableComponent()
    {
        isEnabled = true;
    }

}

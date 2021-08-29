using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadMoveCursor : MonoBehaviour, Controls.IAimingActions, IInitialisable,ICharacterComponents
{
    private Controls input;
    private bool isInitialised = false;
    private bool isMoving;
    private Vector2 aimDirection;
    private float magnitude;

    private Vector2 lastScreenPosition;
    [SerializeField] private GameObject pointer;
    private float smoothRot;

    [Header("Rotation Settings")]
    [SerializeField] float rotationRate;
    [SerializeField] private bool inDebug;

    private void Awake()
    {
        if (inDebug) Init();
    }
    public void Init()
    {
        //Inputs
        input = new Controls();
        input.Aiming.SetCallbacks(this);
        input.Enable();

        input.Aiming.Aim.canceled += _ => StopMovement();
        isInitialised = true;

    }

    public void OnAim(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        if (dir != Vector2.zero && dir.magnitude>0.2f)
        {
            isMoving = true;
            magnitude = dir.magnitude;
            aimDirection = dir.normalized;
        
        }
    }

    public void LateUpdate()
    {
        if (isMoving) FaceAimDirection();


    }
    private void SetIsMoving(bool moving) { isMoving = moving; }
    public void FaceAimDirection()
    {
     
        Vector2 toAimDir = aimDirection - (Vector2)transform.up;
        float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;//get angle to rotate

        //if (targetAngle < 0) targetAngle += 360f;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.z, targetAngle-90f, ref smoothRot, rotationRate);//rotate player smoothly to target angle
        transform.rotation = Quaternion.Euler(0f, 0f, angle);//update angle
        

    }

    private void StopMovement()
    {
        isMoving = false;
    }
    private void OnDestroy()
    {
        if (isInitialised)
        {
            input.Disable();
        
            isMoving = false;
        }


    }

    private void OnEnable()
    {
        if (isInitialised && input != null)
        {
            input.Enable();
            if (pointer) pointer.gameObject.SetActive(true);
        }

    }
    private void OnDisable()
    {
        if (isInitialised && input != null)
        {

        
                input.Disable();
          
                isMoving = false;
            if (pointer) pointer.gameObject.SetActive(false);

        }
    }

    public void EnableComponent()
    {
        if (isInitialised && input != null)
         input.Enable();
      
    }

    public void DisableComponent()
    {
        if (input!=null)
        {

            input.Disable();
        }
        StopMovement();
    }

    public void ResetComponent()
    {
        //
    }

    public GameObject GetCursor()
    {
        return pointer;
    }

}

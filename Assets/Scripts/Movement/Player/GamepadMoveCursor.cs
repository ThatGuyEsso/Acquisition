using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadMoveCursor : MonoBehaviour, Controls.IAimingActions, IInitialisable,ICharacterComponents
{
    private Controls input;
    private bool isInitialised = false;
    private bool isMoving;
    private Vector2 movementDirection;
    private float magnitude;
    [SerializeField] private Transform vCursor;
    [SerializeField] private float sensitivity;
    [SerializeField] private Camera activeCamera;

    [SerializeField] private bool inDebug;
    private bool isCharMoving;
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
        input.Movement.Move.performed += _ => SetIsCharMoving(true);
        input.Movement.Move.canceled += _ => SetIsCharMoving(false);
        isInitialised = true;
    }
    public void SetIsCharMoving(bool moving) { isCharMoving = moving; }

    public void OnAim(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        if (dir != Vector2.zero && dir.magnitude>0.2f)
        {
            isMoving = true;
            magnitude = dir.magnitude;
            movementDirection = dir.normalized;
        
        }
    }

    public void LateUpdate()
    {
        if (isMoving||isCharMoving) MoveCursor();
    }

    public void MoveCursor()
    {
        if (isMoving)
        {
            if (activeCamera && vCursor)
            {
                vCursor.position += (Vector3)movementDirection * magnitude * Time.deltaTime * sensitivity;
                //clamp to camerea viewport
                Vector2 point = vCursor.position;

                Vector2 maxBounds = EssoUtility.MaxCamBounds(activeCamera);

                Vector2 minBounds = EssoUtility.MinCamBounds(activeCamera);

                point.x = Mathf.Clamp(point.x, minBounds.x, maxBounds.x);
                point.y = Mathf.Clamp(point.y, minBounds.y, maxBounds.y);
                vCursor.position = point;

            }
        }
        else if (isCharMoving)
        {
            if (activeCamera && vCursor)
            {
                Vector2 point = activeCamera.ScreenToWorldPoint(vCursor.transform.position);

                Vector2 maxBounds = EssoUtility.MaxCamBounds(activeCamera);

                Vector2 minBounds = EssoUtility.MinCamBounds(activeCamera);

                point.x = Mathf.Clamp(point.x, minBounds.x, maxBounds.x);
                point.y = Mathf.Clamp(point.y, minBounds.y, maxBounds.y);
                vCursor.position = point;

            }
        }

    }

    private void StopMovement()
    {
        isMoving = false;
        movementDirection =Vector2.zero;
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
        if (isInitialised)
        {
            input.Enable();
           
        }

    }
    private void OnDisable()
    {

        if (isInitialised)
        {
            input.Disable();
          
            isMoving = false;

        }
    }

    public void EnableComponent()
    {
        input.Enable();
    }

    public void DisableComponent()
    {
        input.Disable();
        StopMovement();
    }

    public void ResetComponent()
    {
        //
    }
}

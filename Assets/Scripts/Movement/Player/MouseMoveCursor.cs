using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class MouseMoveCursor : MonoBehaviour, Controls.IMouseActivityActions, IInitialisable,ICharacterComponents
{

    private Controls input;
    private bool isInitialised = false;
    private bool isMoving;

    private bool isCharMoving;
    [SerializeField] private Transform vCursor;
    [SerializeField] private Camera activeCamera;
    [SerializeField] private bool inDebug =false;

    private void Awake()
    {
        if (inDebug) Init();
    }
    public void Init()
    {
        //Inputs
        input = new Controls();
        input.MouseActivity.SetCallbacks(this);
        input.Enable();


        input.MouseActivity.Move.canceled += _ => StopMovement();
        input.Movement.Move.performed += _ => SetIsCharMoving(true);
        input.Movement.Move.canceled += _ => SetIsCharMoving(false);
        isInitialised = true;
    }
    private void SetIsCharMoving(bool moving) { isCharMoving = moving; }
    public void LateUpdate()
    {
        if(isMoving|| isCharMoving) MoveCursor();

    }

    private void MoveCursor()
    {
        if (activeCamera && vCursor)
        {
            //clamp to camerea viewport
            Vector2 point = activeCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            Vector2 maxBounds = EssoUtility.MaxCamBounds(activeCamera);

            Vector2 minBounds = EssoUtility.MinCamBounds(activeCamera);

            point.x = Mathf.Clamp(point.x, minBounds.x, maxBounds.x);
            point.y = Mathf.Clamp(point.y, minBounds.y, maxBounds.y);

            vCursor.position = (point);


        }
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
            input.Aiming.Aim.canceled -= _ => StopMovement();
            input.Movement.Move.performed -= _ => SetIsCharMoving(true);
            input.Movement.Move.canceled -= _ => SetIsCharMoving(false);
            isMoving = false;
        }


    }

    private void OnEnable()
    {
        if (isInitialised)
        {
            if (input != null)
                input.Enable();
            input.Aiming.Aim.canceled += _ => StopMovement();
            input.Movement.Move.performed += _ => SetIsCharMoving(true);
            input.Movement.Move.canceled += _ => SetIsCharMoving(false);

        }

    }
    private void OnDisable()
    {

        if (isInitialised)
        {
            if (input != null)
                input.Disable();
            input.Aiming.Aim.canceled -= _ => StopMovement();
            input.Movement.Move.performed -= _ => SetIsCharMoving(true);
            input.Movement.Move.canceled -= _ => SetIsCharMoving(false);
            isMoving = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isMoving = true;
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public Vector3 GetVCusorPosition()
    {
        if (vCursor)
            return vCursor.position;
        else
        {
            vCursor = GameObject.FindGameObjectWithTag("Cursor").transform;
            if (vCursor)
            {
                return vCursor.position;
            }
            return Mouse.current.position.ReadValue();
        }
          
            
        
    }

    public void EnableComponent()
    {
        input.Enable();
        vCursor.gameObject.SetActive(true);
    }

    public void DisableComponent()
    {
        input.Disable();
        StopMovement();
        vCursor.gameObject.SetActive(false);
    }

    public void ResetComponent()
    {
        //false
    }
}



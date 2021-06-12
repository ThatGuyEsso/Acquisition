using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class TDInputMovement : MonoBehaviour, Controls.IMovementActions, IInitialisable
{
    [SerializeField] private bool inDebug =false;
    [SerializeField] private bool isMoving;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;



    public Action<Vector2> OnNewMoveDirection;

    public Action OnWalk;
    public Action OnStop;
    Controls input;
    bool isInitialised;
    Vector2 movementDir;
    private float currentSpeed = 0f;
    private float magnitude = 0f;
    private bool isStopping;

    private Rigidbody2D rb;
    private void Awake()
    {
        if (inDebug) Init();
    }
    public void Init()
    {
        input = new Controls();

        input.Movement.SetCallbacks(this);
        input.Enable();
        input.Movement.Move.canceled += _ => BeginStop();
        isInitialised = true;

        rb = gameObject.GetComponent<Rigidbody2D>();




    }


    private void FixedUpdate()
    {
    
        if (isMoving)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, Time.fixedDeltaTime * acceleration);
            if (Mathf.Abs(maxSpeed - currentSpeed) <= 0.01f) currentSpeed = maxSpeed;

            Move();
        }
        else if (isStopping)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0.0f, Time.fixedDeltaTime * deceleration);


            Vector2 direction = movementDir * currentSpeed;
            rb.velocity = direction;
            if (currentSpeed <= 0.01f)
            {

                Stop();
            }
        }
    }


    private void OnEnable()
    {
        ToggleInputs(true);
    }
    private void Stop()
    {
        currentSpeed = 0f;

        isStopping = false;
        isMoving = false;
        currentSpeed = 0.0f;
        movementDir = Vector2.zero;
        rb.velocity = Vector2.zero;
  
    }
    private void BeginStop()
    {
        magnitude = 0.0f;
        isStopping = true;
        isMoving = false;
        OnStop?.Invoke();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        if (context.performed && dir != Vector2.zero)
        {
            magnitude = dir.magnitude;
            movementDir = dir.normalized;
            OnNewMoveDirection?.Invoke(movementDir);
            OnWalk?.Invoke();
            isMoving = true;
        }
    }
    public void ToggleInputs(bool isOn)
    {
        if (isInitialised)
        {
            if (isOn)
            {
                input.Enable();
                input.Movement.Move.canceled += _ => Stop();

            }
            else
            {
                input.Disable();
                input.Movement.Move.canceled -= _ => Stop();

            }
        }

    }


    public void Move()
    {

        Vector2 velocity = movementDir * currentSpeed * magnitude;

        rb.velocity = velocity;

    }




    private void OnDisable()
    {
        if (isInitialised)
        {
            input.Disable();
            input.Movement.Move.canceled -= _ => Stop();
        }
    }

    private void OnDestroy()
    {
        if (isInitialised)
        {
            input.Disable();
            input.Movement.Move.canceled -= _ => Stop();
        }
    }

    public float GetMaxSpeed() { return maxSpeed; }
    public float GetCurrentSpeed() { return currentSpeed; }

    public Vector2 GetMoveDirection() { return movementDir; }
    public void SetCurrentSpeed(float newSpeed) { currentSpeed = newSpeed; }
    public void SetIsMoving(bool moving ) { isMoving = moving; }
}


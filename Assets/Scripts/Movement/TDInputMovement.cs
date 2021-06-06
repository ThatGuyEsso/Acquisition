using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class TDInputMovement : MonoBehaviour, Controls.IMovementActions, IInitialisable
{
    [SerializeField] private bool inDebug =false;
    [SerializeField] private bool isMoving;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;

    private Rigidbody2D rb;
    Controls input;
    bool isInitialised;
    Vector2 movementDir;
    private float currentSpeed = 0f;
    private float magnitude = 0f;
    private bool isStopping;


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

        rb = GetComponent<Rigidbody2D>();

    }
    private void Update()
    {
        if (isMoving)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, Time.deltaTime * acceleration);
            if (Mathf.Abs(maxSpeed - currentSpeed) <= 0.01f) currentSpeed = maxSpeed;

            Move();
        }
        else if (isStopping)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0.0f, Time.deltaTime * deceleration);


            float direction = movementDir.x * currentSpeed;
            rb.velocity = new Vector2(direction, rb.velocity.y);
            if (currentSpeed <= 0.01f)
            {

                Stop();
            }
        }
    }
    //private void FixedUpdate()
    //{

   

    //}




    private void OnEnable()
    {
        ToggleInputs(true);
    }
    private void Stop()
    {
        currentSpeed = 0f;
        Debug.Log("Stop");
        isStopping = false;
        isMoving = false;
        currentSpeed = 0.0f;
    }
    private void BeginStop()
    {
        magnitude = 0.0f;
        isStopping = true;
        isMoving = false;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        if (context.performed && dir != Vector2.zero)
        {

            magnitude = dir.magnitude;
            movementDir = dir.normalized;
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

        Vector2 velocity = movementDir * currentSpeed * magnitude*Time.deltaTime;

        transform.position += new Vector3(velocity.x, velocity.y, 0.0f);

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


}


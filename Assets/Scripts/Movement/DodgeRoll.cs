using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DodgeRoll : MonoBehaviour, Controls.IDodgeRollActions,IInitialisable
{


    private Rigidbody2D rb;
    [SerializeField] private bool inDebug;

    [Header("Dodgeroll Settings")]
    [SerializeField] private float maxRollSpeed;
    [SerializeField] private float dodgeCooldown;
    [SerializeField] private bool canDodge;
    [SerializeField] private float rollTime;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [SerializeField] private List<Behaviour> inactiveComponentsDuringRoll =new List<Behaviour>();
    [SerializeField] private TDInputMovement tdMovement;
    private Controls input;
    private bool isInitialised;
    private bool isRolling;
    private bool isStopping;
    private float currentSpeed;
    private float rollEndSpeed;
    private Vector2 rollDirection;
    private void Awake()
    {
        if (inDebug) Init();
    }
    public void Init()
    {
        canDodge = true;
        rb = GetComponent<Rigidbody2D>();
        //Inputs
        input = new Controls();
        input.DodgeRoll.SetCallbacks(this);
        input.Enable();
        isInitialised = true;
    }
    public void OnRoll(InputAction.CallbackContext context)
    {
   
        if (context.performed && canDodge)
        {
            Debug.Log("Dodge");
            DoDodgeRoll();
        }
    }
    private void FixedUpdate()
    {

        if (isRolling)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxRollSpeed, Time.fixedDeltaTime * acceleration);
            if (Mathf.Abs(maxRollSpeed - currentSpeed) <= 0.01f) currentSpeed = maxRollSpeed;

            Roll();
        }
        else if (isStopping)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0.0f, Time.fixedDeltaTime * deceleration);


            Vector2 direction = rollDirection * currentSpeed;
            if (tdMovement.GetMoveDirection() == Vector2.zero)
                rb.velocity = direction;
           
            if (currentSpeed <= 0.01f)
            {

                StopRoll();
            }
        }
    }
    private void OnEnable()
    {
        if (isInitialised) input.Enable();
    }

    private void OnDisable()
    {
        if (isInitialised) input.Disable();
    }
    private void OnDestroy()
    {
        if (isInitialised) input.Disable();
    }


    public void DoDodgeRoll()
    {
        canDodge = false;
        if (tdMovement.GetMoveDirection() != Vector2.zero) rollDirection = tdMovement.GetMoveDirection().normalized;
        else rollDirection = transform.up;

        if (tdMovement.GetCurrentSpeed() > 0f) rollEndSpeed = tdMovement.GetMaxSpeed();
        else rollEndSpeed = 0f;
        ToggleComponents(false);
        isRolling = true;

        Invoke("EndRoll", rollTime);



    }
    public void Roll()
    {
        Vector2 velocity = rollDirection * currentSpeed;
        rb.velocity = velocity;
    }
    public void EndRoll()
    {

        ToggleComponents(true);
        tdMovement.SetCurrentSpeed(rollEndSpeed);
        isRolling = false;
        isStopping = true;
      

    }
    public void StopRoll()
    {
        isStopping = false;

        if(tdMovement.GetMoveDirection()==Vector2.zero)
            rb.velocity = Vector2.zero;

        Invoke("ResetDodge", dodgeCooldown);
    }

    public void ResetDodge()
    {

        canDodge = true;
   
    }

    public void ToggleComponents(bool isActive)
    {
        if(inactiveComponentsDuringRoll.Count > 0)
        {
            foreach(Behaviour comp in inactiveComponentsDuringRoll)
            {
                comp.enabled = isActive;
            }
        }
    }
}

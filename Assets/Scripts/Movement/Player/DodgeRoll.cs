using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DodgeRoll : MonoBehaviour, Controls.IDodgeRollActions,IInitialisable,ICharacterComponents
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

    [Header("Dodgeroll VFX")]
    [SerializeField] private AfterImageController VFXcontroller;


    [SerializeField] private int invisibilityLayer;
    [SerializeField] private int defaultLayer;
    [SerializeField] private Animator dodgeAnimator;
    [SerializeField] private GameObject topGFX;
    [SerializeField] private GameObject legsGFX;
    private Controls input;
    private bool isInitialised;
    private bool isRolling;
    private bool isStopping;
    private float currentSpeed;
    private float rollEndSpeed;
    private Vector2 rollDirection;

    public System.Action OnRollBegun;
    public System.Action OnRollEnd;
    private void Awake()
    {
        if (inDebug) Init();
    }
    public void Init()
    {
        dodgeAnimator.gameObject.SetActive(false);
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
        if (isInitialised)
        {
            if(input!=null)
                input.Enable();
            if (!canDodge) ResetDodge();
        }

    }

    private void OnDisable()
    {
        if (isInitialised)
        {
            if (input != null)
                input.Disable();
            StopAllCoroutines();
        }
    }
    private void OnDestroy()
    {
        if (isInitialised&&input != null) input.Disable();
    }

    public void PlayDodgeSFX()
    {
        if (AudioManager.instance)
        {
            AudioManager.instance.PlayThroughAudioPlayer("DodgeRoll", transform.position, true);
        }
    }
    public void DoDodgeRoll()
    {
        dodgeAnimator.gameObject.SetActive(true);
        topGFX.SetActive(false);
        legsGFX.SetActive(false);
        if (VFXcontroller) VFXcontroller.StartDrawing(0.05f);
        if (WeaponManager.instance)
        {
            WeaponManager.instance.ToggleWeapon(false);
        }
        if (dodgeAnimator)
        {

            dodgeAnimator.Play("DodgeRoll", 0, 0f);
            PlayDodgeSFX();
            OnRollBegun?.Invoke();
        }
        gameObject.layer = invisibilityLayer;
        canDodge = false;
        if (tdMovement.GetMoveDirection() != Vector2.zero) rollDirection = tdMovement.GetMoveDirection().normalized;
        else rollDirection = transform.up;

        if (tdMovement.GetCurrentSpeed() > 0f) rollEndSpeed = tdMovement.GetMaxSpeed();
        else rollEndSpeed = 0f;
        ToggleComponents(false);
        isRolling = true;
        OrientateToMovement();
        StartCoroutine(RollTimer());



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
        OnRollEnd?.Invoke();
        if (tdMovement.GetMoveDirection()==Vector2.zero)
            rb.velocity = Vector2.zero;
        if (VFXcontroller) VFXcontroller.StopDrawing();
        dodgeAnimator.gameObject.SetActive(false);
        topGFX.SetActive(true);
        legsGFX.SetActive(true);
        gameObject.layer = defaultLayer;

        if (WeaponManager.instance)
        {
            WeaponManager.instance.ToggleWeapon(true);
        }
        StartCoroutine(WaitToRefreshDodge());
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
    private IEnumerator RollTimer()
    {
        yield return new WaitForSeconds(rollTime);
        EndRoll();
    }

    private IEnumerator WaitToRefreshDodge()
    {
        yield return new WaitForSeconds(dodgeCooldown);
        ResetDodge();
    }
    public void EnableComponent()
    {
        input.Enable();
        canDodge = true;
        ToggleComponents(true);
    }

    public void DisableComponent()
    {
        input.Disable();
        tdMovement.SetCurrentSpeed(rollEndSpeed);
        isRolling = false;
        isStopping = false;
        dodgeAnimator.gameObject.SetActive(false);
        topGFX.SetActive(true);
        legsGFX.SetActive(true);
        gameObject.layer = defaultLayer;
        StopAllCoroutines();
    }

    public void ResetComponent()
    {
        StopAllCoroutines();
        isRolling = false;
        isStopping = false;
        currentSpeed =0f;
        canDodge = true;
    }
    public void OrientateToMovement()
    {

        float targetAngle = EssoUtility.GetAngleFromVector(rollDirection.normalized);
        /// turn offset -Due to converting between forward vector and up vector
        if (targetAngle < 0) targetAngle += 360f;
        transform.rotation = Quaternion.Euler(0.0f, 0f, targetAngle - 90f);
    }
 
}

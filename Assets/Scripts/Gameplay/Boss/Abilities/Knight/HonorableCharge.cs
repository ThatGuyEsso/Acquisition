using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HonorableCharge : BaseBossAbility
{


    public Rigidbody2D rb;
    [SerializeField] private bool inDebug;
    [SerializeField] private LayerMask stopChargeLayers;
    [Header("Charge Settings")]
    [SerializeField] private float maxChargeSpeed;

    [SerializeField] private float chargeTime;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [Header("Ability Settings")]
    [SerializeField] private GameObject attackColliderPrefab;


    [Header("VFX/SFX Settings")]
    [SerializeField] private GameObject dustVFx;
    [SerializeField] private float sfxPlayRate;
    private AfterImageController afterImageController;

    private float currTimeToSFX;
    private AttackVolume chargeCollider;
    protected bool isCharging;
    protected bool isStopping;
    protected float currrentChargeTime;

    protected float currentSpeed =0;
    protected Vector2 chargeDirection;
    

    public override void Init()
    {
        base.Init();
        afterImageController = GetComponent<AfterImageController>();

    }

    private void Update()
    {
        if (isCharging)
        {
            if (currrentChargeTime <= 0)
            {
                isCharging = false;
                isStopping = true;
            }
            else
            {
                currrentChargeTime -= Time.deltaTime;
            }

            if (currTimeToSFX <= 0) {

                if (AudioManager.instance)
                    AudioManager.instance.PlayGroupThroughAudioPlayer("KnightSteps", owner.transform.position,true);
                currTimeToSFX = sfxPlayRate;
                if (dustVFx) ObjectPoolManager.Spawn(dustVFx, owner.transform.position, owner.transform.rotation);
                if (CamShake.instance) CamShake.instance.DoScreenShake(0.1f, 2f, 0f, 0.05f, 1.5f);
            }
            else
            {
                currTimeToSFX -= Time.deltaTime;
            }
        }
  
    }
    public void FixedUpdate()
    {
     
        if (isCharging)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxChargeSpeed, Time.fixedDeltaTime * acceleration);
            if (Mathf.Abs(maxChargeSpeed - currentSpeed) <= 0.01f) currentSpeed = maxChargeSpeed;

            Charge();

         
        }
        else if (isStopping)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0.0f, Time.fixedDeltaTime * deceleration);


            Vector2 direction = chargeDirection * currentSpeed;
            rb.velocity = direction;

            if (currentSpeed <= 0.1f)
            {

                StopCharge();
            }
        }

 
    }

    public void BeginCharge()
    {
        canAttack = false;
        if ((rb = owner.GetComponent<IBoss>().GetRigidBody()) == true)
        {
            attacksLeft--;
            currrentChargeTime = chargeTime;
            owner.SetCanLockOn(false);
            chargeDirection = owner.GetFirePoint().up;
            owner.GetComponent<IBoss>().SetUseRigidBody(true);

            chargeCollider = ObjectPoolManager.Spawn(attackColliderPrefab, owner.GetFirePoint(), Vector3.zero).GetComponent<AttackVolume>();

            chargeCollider.WallHit += StopCharge;
            chargeCollider.OnPlayerHit += StopCharge;
            isCharging = true;

        }

        if (afterImageController) afterImageController.StartDrawing();
    }
    public void Charge()
    {
        Vector2 velocity = chargeDirection * currentSpeed;
        rb.velocity = velocity;

    }





    public void StopCharge()
    {
        isStopping = false;
        isCharging = false;
        rb.velocity = Vector2.zero;
        owner.GetComponent<IBoss>().SetUseRigidBody(false);

        if (afterImageController) afterImageController.StopDrawing();
        currentSpeed = 0;
        owner.GetComponent<IBoss>().PlayAnimation("Idle");
        owner.SetIsBusy(false);
        if (chargeCollider)
        {
            chargeCollider.WallHit -= StopCharge;
            chargeCollider.OnPlayerHit -= StopCharge;
            ObjectPoolManager.Recycle(chargeCollider.gameObject);
        }
        chargeCollider = null;

        if (attacksLeft <= 0)
        {
            StopAllCoroutines();
            owner.CycleToNextAttack();
            StartCoroutine(BeginResetAbility(coolDown));

        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(BeginRefreshAttack(attackRate));
        }
        
    }

    override public void DisableAbility()
    {
        isEnabled = false;
        if (eventListener)
            eventListener.OnShowAttackZone -= BeginCharge;

        if (afterImageController) afterImageController.StopDrawing();
        if(isCharging)
        {
            isStopping = false;
            isCharging = false;
            rb.velocity = Vector2.zero;
            owner.GetComponent<IBoss>().SetUseRigidBody(false);

            if (afterImageController) afterImageController.StopDrawing();
            currentSpeed = 0;
            owner.SetIsBusy(false);
            if (chargeCollider)
            {
                chargeCollider.WallHit -= StopCharge;
                chargeCollider.OnPlayerHit -= StopCharge;
                ObjectPoolManager.Recycle(chargeCollider.gameObject);
            }
            chargeCollider = null;

        }
    }

    override public void EnableAbility()
    {
        base.EnableAbility();
        if (eventListener)
        {
            eventListener.OnShowAttackZone += BeginCharge;
        }
        if (owner)
        {
            transform.rotation = owner.GetFirePoint().rotation;
            SpriteRenderer ownerRenderer = owner.GetRenderer();
            afterImageController = owner.GetAfterimageController();
            if (ownerRenderer && afterImageController) afterImageController.SetUpRenderer(ownerRenderer,0.1f,0.2f);
        }

    }

}

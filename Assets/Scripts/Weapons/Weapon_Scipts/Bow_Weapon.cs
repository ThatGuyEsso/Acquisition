using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow_Weapon : Base_Weapon
{
    [Header("Bow Settings")]
    [SerializeField] private float primaryShotSpeed;
    [SerializeField] private float primaryShotKnockBack;
    [SerializeField] private float primaryShotLifeTime;

    [SerializeField] private float secondaryShotSpeed;
    [SerializeField] private float secondaryShotLifeTime;



    [SerializeField] private GameObject weakCharge, midCharge, superCharge;

    private MouseMoveCursor vCursor;
    private int chargeCount=0;
    bool isCharging = false;

    public override void Init()
    {
        base.Init();
        vCursor = GameObject.FindGameObjectWithTag("Player").GetComponent<MouseMoveCursor>();
    }

    protected override void PrimaryAttack()
    {
        if (!isWeaponActive)
            return;

        if (!canPrimaryFire) return;

        if (isBusy) return;


        isBusy = true;
        isIdle = false;
        currTimeToIdle = timeToIdle;
        canPrimaryFire = false;

        OnPrimaryAttack?.Invoke();
        attackEvents.OnShootProjectile += OnFireProjectile;
        attackEvents.OnPlaySFX += PlayArrowDrawSFX;
        attackEvents.OnAnimEnd += ResetPrimaryFire;
        animSolver.PlayAnimationFromStart("Primary_Bow");

    }


    private void Update()
    {
        if (!isIdle&&!isBusy)
        {
            if (currTimeToIdle <= 0f)
            {
                isIdle = true;
                if (isRunning)
                {
                    animSolver.PlayAnimation("Run_Bow");
                    Debug.Log("run");
                }
                else
                {
                    animSolver.PlayAnimation("Idle_Bow");
                }
                currTimeToIdle = timeToIdle;
            }
            else
            {
                currTimeToIdle -= Time.deltaTime;
            }
        }
    }

    public override void OnFireProjectile()
    {
        Vector2 dir = (vCursor.GetVCusorPosition() - firePoint.position).normalized;
        GameObject go = ObjectPoolManager.Spawn(primaryProjectile, firePoint.transform.position, Quaternion.identity);
        IProjectile projectile = go.GetComponent<IProjectile>();
        if (projectile!=null)
        {
            projectile.SetUpProjectile(primaryAttackDamage, dir, primaryShotSpeed,primaryShotLifeTime, 0,playerTransform.gameObject);
            OnPrimaryAbility?.Invoke(go);

        }
        attackEvents.OnShootProjectile -= OnFireProjectile;
        PlayArrowShotSFX();
    }

    public override void Equip(Transform firePoint, AttackAnimEventListener eventListener, Transform player, TopPlayerGFXSolver solver)
    {
        if (!isInitialised)
        {
            Init();
            inputAction.Attack.PrimaryAttack.started += ctx => PrimaryAttack();
            inputAction.Attack.PrimaryAttack.started += ctx => OnPrimaryHeld();
            inputAction.Attack.SecondaryAttack.started += ctx => SecondaryAttack();
            inputAction.Attack.SecondaryAttack.canceled += ctx => EvaluateChargeShot();
            inputAction.Attack.PrimaryAttack.canceled += ctx => OnPrimaryReleased();
        }
        else
            inputAction.Enable();

        this.firePoint = firePoint;
        attackEvents = eventListener;
        playerTransform = player;
        SetCanFire(true);
        animSolver = solver;
        animSolver.movement.OnWalk += OnRun;
        animSolver.movement.OnStop += OnStop;
     
        Debug.Log("Equip");
    }
    public override void UnEquip()
    {
        base.UnEquip();
        chargeCount = 0;
        StopAllCoroutines();
        attackEvents.OnAnimEnd -= ResetSecondaryFire;
        attackEvents.OnPlaySFX -= PlayArrowDrawSFX;
        isBusy = false;
        isCharging = false;
        Debug.Log("unequip");
    }
    public override void DisableWeapon()
    {
        base.DisableWeapon();
        attackEvents.OnPlaySFX -= PlayArrowDrawSFX;
        attackEvents.OnShootProjectile -= OnFireProjectile;

        attackEvents.OnAnimEnd -= ResetPrimaryFire;
        if (chargeCount > 0)
        {
            attackEvents.OnChargeIncrease -= IncreaseCharge;

            Vector2 dir = (vCursor.GetVCusorPosition() - firePoint.position).normalized;
            if (chargeCount == 1)
            {

                IProjectile projectile = ObjectPoolManager.Spawn(weakCharge, firePoint.transform.position, Quaternion.identity)
                .GetComponent<IProjectile>();
                if (projectile != null)
                {
                    projectile.ShootProjectile(secondaryShotSpeed, dir, secondaryShotLifeTime);
                }
            }
            else if (chargeCount == 2)
            {
                IProjectile projectile = ObjectPoolManager.Spawn(midCharge, firePoint.transform.position, Quaternion.identity)
              .GetComponent<IProjectile>();
                if (projectile != null)
                {
                    projectile.ShootProjectile(secondaryShotSpeed, dir, secondaryShotLifeTime);
                }
            }
            else if (chargeCount >= 3)
            {
                IProjectile projectile = ObjectPoolManager.Spawn(superCharge, firePoint.transform.position, Quaternion.identity)
                .GetComponent<IProjectile>();
                if (projectile != null)
                {
                    projectile.ShootProjectile(secondaryShotSpeed, dir, secondaryShotLifeTime);
                }
            }
            isCharging = false;
        }
        else
        {
            isCharging = false;
        }
    }


    public override void EnableWeapon()
    {

        base.EnableWeapon();
        if (!canPrimaryFire) ResetPrimaryFire();
        if (!canSecondaryFire) ResetSecondaryFire();
    }
    protected override void SecondaryAttack()
    {

        if (isWeaponActive == false)
            return;

        if (!canSecondaryFire)
            return;


        if (isBusy) return;


        isBusy = true;
        isIdle = false;
        canSecondaryFire = false;

        currTimeToIdle = timeToIdle;
        attackEvents.OnChargeIncrease += IncreaseCharge;
        isCharging = true;
        OnSecondaryAttack?.Invoke();
        animSolver.PlayAnimationFromStart("Secondary_Bow");

    }

    public void PlayArrowShotSFX()
    {
        if (AudioManager.instance)
        {
            AudioManager.instance.PlayThroughAudioPlayer("BowDraw", playerTransform.position);
        }
    }

    public void PlayArrowDrawSFX()
    {
        attackEvents.OnPlaySFX -= PlayArrowDrawSFX;
        if (AudioManager.instance)
        {
            AudioManager.instance.PlayThroughAudioPlayer("BowShot", playerTransform.position);
        }
    }

    private void EvaluateChargeShot()
    {

        if (isCharging)
        {
            attackEvents.OnChargeIncrease -= IncreaseCharge;

            Vector2 dir = (vCursor.GetVCusorPosition() - firePoint.position).normalized;
            if (chargeCount == 1)
            {
                GameObject projObject = ObjectPoolManager.Spawn(weakCharge, firePoint.transform.position, Quaternion.identity);
                IProjectile projectile = projObject.GetComponent<IProjectile>();

                if (projectile != null)
                {
                    projectile.ShootProjectile(secondaryShotSpeed, dir, secondaryShotLifeTime);
                    OnSecondaryAbility?.Invoke(projObject);
                }
            }
            else if (chargeCount == 2)
            {
                GameObject projObject = ObjectPoolManager.Spawn(midCharge, firePoint.transform.position, Quaternion.identity);
                IProjectile projectile = projObject.GetComponent<IProjectile>();
                if (projectile != null)
                {
                    projectile.ShootProjectile(secondaryShotSpeed, dir, secondaryShotLifeTime);
                    OnSecondaryAbility?.Invoke(projObject);
                }
            }
            else if (chargeCount >= 3)
            {
                GameObject projObject = ObjectPoolManager.Spawn(superCharge, firePoint.transform.position, Quaternion.identity);
                IProjectile projectile = projObject.GetComponent<IProjectile>();
                if (projectile != null)
                {
                    projectile.ShootProjectile(secondaryShotSpeed, dir, secondaryShotLifeTime);
                    OnSecondaryAbility?.Invoke(projObject);
                }
            }

            attackEvents.OnAnimEnd += ResetSecondaryFire;
            animSolver.PlayAnimationFromStart("ReleaseCharge");
           
        }
       

    }
    private void IncreaseCharge()
    {

        chargeCount++;

    }

    public override void OnStop()
    {
        base.OnStop();
        if (isIdle)
        {
            animSolver.PlayAnimation("Idle_Bow");
        }
    }

    public override void OnRun()
    {
        base.OnRun();
        if (isIdle)
        {
            animSolver.PlayAnimation("Run_Bow");
        }
    }


    override public void ResetSecondaryFire()
    {
        attackEvents.OnAnimEnd -= ResetSecondaryFire;
        isBusy = false;
        isCharging = false;

        if(chargeCount>0)
             StartCoroutine(WaitForFireSecondaryRate(secondaryFireRate));
        else
        {
            canSecondaryFire = true;
        }
        chargeCount = 0;
    }


    public override void ResetPrimaryFire()
    {
        base.ResetPrimaryFire();
        if (primaryHeld) PrimaryAttack();
    }
}

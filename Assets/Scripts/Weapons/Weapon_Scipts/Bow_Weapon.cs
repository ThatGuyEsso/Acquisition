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


    private int chargeCount=0;
    bool isCharging = false;
    bool primaryHeld =false;
    public override void Init()
    {
        base.Init();

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
        primaryHeld = true;
        attackEvents.OnShootProjectile += OnFireProjectile;
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
        GameObject go = ObjectPoolManager.Spawn(primaryProjectile, firePoint.transform.position, Quaternion.identity);
        go.GetComponent<IProjectile>().SetUpProjectile(primaryAttackDamage, firePoint.transform.up.normalized, primaryShotSpeed,primaryShotLifeTime, 0,playerTransform.gameObject);
        attackEvents.OnShootProjectile -= OnFireProjectile;
        Debug.Log("Fire");
    }

    public override void Equip(Transform firePoint, AttackAnimEventListener eventListener, Transform player, TopPlayerGFXSolver solver)
    {
        base.Equip(firePoint, eventListener, player, solver);
        inputAction.Attack.SecondaryAttack.canceled += ctx => EvaluateChargeShot();
        inputAction.Attack.PrimaryAttack.canceled += ctx => OnPrimaryReleased();
    }
    public override void UnEquip()
    {
        base.UnEquip();
        inputAction.Attack.SecondaryAttack.canceled -= ctx => EvaluateChargeShot();
        inputAction.Attack.PrimaryAttack.canceled -= ctx => OnPrimaryReleased();
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
        animSolver.PlayAnimationFromStart("Secondary_Bow");

    }



    private void EvaluateChargeShot()
    {

        if (isCharging)
        {
            attackEvents.OnChargeIncrease -= IncreaseCharge;


            if (chargeCount == 1)
            {
                IProjectile projectile = ObjectPoolManager.Spawn(weakCharge, firePoint.transform.position, Quaternion.identity)
                .GetComponent<IProjectile>();
                if (projectile != null)
                {
                    projectile.ShootProjectile(secondaryShotSpeed, firePoint.up, secondaryShotLifeTime);
                }
            }
            else if (chargeCount == 2)
            {
                IProjectile projectile = ObjectPoolManager.Spawn(midCharge, firePoint.transform.position, Quaternion.identity)
              .GetComponent<IProjectile>();
                if (projectile != null)
                {
                    projectile.ShootProjectile(secondaryShotSpeed, firePoint.up, secondaryShotLifeTime);
                }
            }
            else if (chargeCount >= 3)
            {
                IProjectile projectile = ObjectPoolManager.Spawn(superCharge, firePoint.transform.position, Quaternion.identity)
                .GetComponent<IProjectile>();
                if (projectile != null)
                {
                    projectile.ShootProjectile(secondaryShotSpeed, firePoint.up, secondaryShotLifeTime);
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

    private void OnPrimaryHeld() {
        primaryHeld = true;
    }

    private void OnPrimaryReleased()
    {
        primaryHeld = false;
    }

    public override void ResetPrimaryFire()
    {
        base.ResetPrimaryFire();
        if (primaryHeld) PrimaryAttack();
    }
}

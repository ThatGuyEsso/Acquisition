using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Weapon : Base_Weapon
{
    [Header("Sword Settings")]
    [SerializeField] private DynamicConeCollider swordCollider;

    [SerializeField] private float primaryCollisionRadius = 1.5f;
    [SerializeField] private float secondaryCollisionRadius = 2.5f;

    [Header("Sword Slash Porjectiles Settings")]
    [SerializeField] private float primarySlashSpeed = 5f;
    [SerializeField] private float primarySlashLifeTIme = 2f;

    [SerializeField] private float SecondarySlashSpeed = 10f;
    [SerializeField] private float SecondarySlashLifeTIme = 3f;
    [SerializeField] private Vector2 secondaryProjectileOffset;
    private DynamicConeCollider sCollider;
    bool isLeftSwing;


    
    protected override void PrimaryAttack()
    {
        if (!isWeaponActive)
            return;

        if (!canPrimaryFire)
            return;

        if (isBusy) return;
        isBusy = true;
        attackEvents.OnShowAttackZone += CreateAttackCollider;
        attackEvents.OnHideAttackZone+= DestroyAttackZone;
        attackEvents.OnShootProjectile += OnFireProjectile;
        attackEvents.OnAnimEnd += ResetPrimaryFire;
        if (isIdle || !isLeftSwing)
        {
            isIdle = false;
            isLeftSwing = true;
            currTimeToIdle = timeToIdle;
            animSolver.PlayAnimation("Primary");
        }
        else
        {
            isIdle = false;
            isLeftSwing = false;
            currTimeToIdle = timeToIdle;
            animSolver.PlayAnimation("Primary_2");
        }

        canPrimaryFire = false;

    }
    private void Update()
    {
        if (!isIdle)
        {
            if (currTimeToIdle <=0f)
            {
                isIdle = true;
                if (isRunning)
                {
                    animSolver.PlayAnimation("Run_Sword");

                }
                else
                {
                    animSolver.PlayAnimation("Idle_Sword");
                }
                currTimeToIdle = timeToIdle;
            }
            else
            {
                currTimeToIdle -= Time.deltaTime;
            }
        }
    }
    public void CreateAttackCollider()
    {
        sCollider = ObjectPoolManager.Spawn(swordCollider.gameObject, Vector3.zero, Quaternion.identity).GetComponent<DynamicConeCollider>();
        sCollider.SetColliderShape(firePoint.transform.up, primaryCollisionRadius, 90f, playerTransform.position);
        if (sCollider.GetComponent<IVolumes>() != null) {
            IVolumes volume = sCollider.GetComponent<IVolumes>();
            volume.SetIsPlayerZone(true);
            volume.SetUpDamageVolume(primaryAttackDamage, 10f, firePoint.up, playerTransform.gameObject);
        }
        attackEvents.OnShowAttackZone -= CreateAttackCollider;
     
    }
    public void CreateSecondaryAttackCollider()
    {
        sCollider = ObjectPoolManager.Spawn(swordCollider.gameObject, Vector3.zero, Quaternion.identity).GetComponent<DynamicConeCollider>();
        sCollider.SetColliderShape(firePoint.transform.up, secondaryCollisionRadius, 60f, playerTransform.position);
        if (sCollider.GetComponent<IVolumes>() != null)
        {
            IVolumes volume = sCollider.GetComponent<IVolumes>();
            volume.SetIsPlayerZone(true);
            volume.SetUpDamageVolume(primaryAttackDamage, 10f, firePoint.up, playerTransform.gameObject);
        }
        attackEvents.OnShowAttackZone -= CreateSecondaryAttackCollider;

    }

    public void DestroyAttackZone()
    {
        if(sCollider)
            Destroy(sCollider.gameObject);
        sCollider = null;
        attackEvents.OnHideAttackZone -= DestroyAttackZone;



    }

    public override void OnFireProjectile()
    {
        IProjectile projectile = ObjectPoolManager.Spawn(primaryProjectile, firePoint.transform.position, Quaternion.identity)
              .GetComponent<IProjectile>();
        if (projectile!=null)
        {
            projectile.SetUpProjectile(primaryProjectileDamage, firePoint.up, primarySlashSpeed, primarySlashLifeTIme, 0, playerTransform.gameObject);
        }
        attackEvents.OnShootProjectile -= OnFireProjectile;
    }


    public  void OnFireSecondaryProjectile()
    {
        IProjectile projectile = ObjectPoolManager.Spawn(secondaryProjectile, firePoint.transform.position+ (Vector3)secondaryProjectileOffset, Quaternion.identity)
              .GetComponent<IProjectile>();
        if (projectile != null)
        {
            projectile.SetUpProjectile(secondaryProjectileDamage, firePoint.up,SecondarySlashSpeed, SecondarySlashLifeTIme,3, playerTransform.gameObject);
        }
        attackEvents.OnShootProjectile -= OnFireSecondaryProjectile;
    }



    protected override void SecondaryAttack()
    {
        if (isBusy)
        {
          
            Debug.Log("BUSY");
           
            return;
        }
        if (!isWeaponActive)
        {

            Debug.Log("INACTIVE");

            return;
        }

        if (!canSecondaryFire)
        {
            Debug.Log("can't attack");
            return;
        }

        isIdle = false;
        isBusy = true;
        canSecondaryFire = false;
        isLeftSwing = true;
        attackEvents.OnShowAttackZone += CreateSecondaryAttackCollider;
        attackEvents.OnHideAttackZone += DestroyAttackZone;
        attackEvents.OnShootProjectile += OnFireSecondaryProjectile;
        attackEvents.OnAnimEnd += ResetSecondaryFire;
        currTimeToIdle = timeToIdle;
        animSolver.PlayAnimationFromStart("Secondary");
 
  
        Debug.Log("Second Attack");


    }


    public override void Equip(Transform firePoint, AttackAnimEventListener eventListener, Transform player,TopPlayerGFXSolver solver)
    {
        base.Equip(firePoint, eventListener, player,solver);


    }

    public override void UnEquip()
    {
        base.UnEquip();

    }

    public override void OnStop()
    {
        base.OnStop();
        if (isIdle)
        {
            animSolver.PlayAnimation("Idle_Sword");
        }
    }

    public override void OnRun()
    {
        base.OnRun();
        if (isIdle)
        {
            animSolver.PlayAnimation("Run_Sword");
        }
    }
    override public void ResetPrimaryFire()
    {
        canPrimaryFire = true;
        isBusy = false;
        attackEvents.OnAnimEnd -= ResetPrimaryFire;
    }

    override public void ResetSecondaryFire()
    {
        attackEvents.OnAnimEnd -= ResetSecondaryFire;
        isBusy = false;
        StartCoroutine(WaitForFireSecondaryRate(secondaryFireRate));
        Debug.Log("Not busy");
    }
}

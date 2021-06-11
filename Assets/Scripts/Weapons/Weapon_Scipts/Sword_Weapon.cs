using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Weapon : Base_Weapon
{
    [Header("Sword Settings")]
    [SerializeField] private DynamicConeCollider swordCollider;
    [SerializeField] private float timeToIdle;
    [SerializeField] private float primaryCollisionRadius = 1.5f;
    [SerializeField] private float secondaryCollisionRadius = 2.5f;

    [Header("Sword Slash Porjectiles Settings")]
    [SerializeField] private float primarySlashSpeed = 5f;
    [SerializeField] private float primarySlashLifeTIme = 2f;

    [SerializeField] private float SecondarySlashSpeed = 10f;
    [SerializeField] private float SecondarySlashLifeTIme = 3f;

    private DynamicConeCollider sCollider;
    bool isLeftSwing;
    bool isIdle=true;
   float currTimeToIdle;
    protected override void PrimaryAttack()
    {
        if (!isWeaponActive)
            return;

        if (!canPrimaryFire)
            return;

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
                animSolver.PlayAnimation("Idle_Sword");
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
        sCollider = Instantiate(swordCollider);
        sCollider.SetColliderShape(firePoint.transform.up, primaryCollisionRadius, 90, transform.position);
        if (sCollider.GetComponent<IVolumes>() != null) sCollider.GetComponent<IVolumes>().SetIsPlayerZone(true);
        attackEvents.OnShowAttackZone -= CreateAttackCollider;
     
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

   


    protected override void SecondaryAttack()
    {
        if (!isWeaponActive)
            return;

        if (!canPrimaryFire)
            return;

        sCollider = Instantiate(swordCollider);
        sCollider.SetColliderShape(firePoint.transform.up, secondaryCollisionRadius, 90, transform.position);

   
           
    }


    public override void Equip(Transform firePoint, AttackAnimEventListener eventListener, Transform player,TopPlayerGFXSolver solver)
    {
        base.Equip(firePoint, eventListener, player,solver);


    }

    public override void UnEquip()
    {
        base.UnEquip();

    }

    public void ResetPrimaryFire()
    {
        canPrimaryFire = true;
        attackEvents.OnAnimEnd -= OnFireProjectile;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingSkillAttribute : Base_SkillAttribute
{
    [SerializeField] private GameObject swordHomingProjectilePrefab;
    [SerializeField] private float swordHomingSpeed;
    [SerializeField] private GameObject bowHomingProjectilePrefab;
    [SerializeField] private float bowHomingSpeed;

    [SerializeField] private GameObject ShieldHomingProjectilePrefab;
    [SerializeField] private float shieldHomingSpeed;
    [SerializeField] private float shieldProjDamage;
    [SerializeField] private int shieldHitPoints;
    [SerializeField] private float shieldHomingLifeTime;
    private BubbleShield currentShield;
    public override void SetUpAttribute(Base_Weapon weaponOwner)
    {
        base.SetUpAttribute(weaponOwner);

        switch (weaponOwner.GetWeaponType())
        {
            case WeaponType.Sword:
                owner.SetSecondaryProjectile(swordHomingProjectilePrefab);
                break;
            case WeaponType.Bow:

                owner.SetPrimaryProjectilePrefab(bowHomingProjectilePrefab);
                break;
        }
    }




    override public void EvaluatePrimaryAbilityAttribute(GameObject spawnedAbiliity)
    {
        switch (owner.GetWeaponType())
        { 
            case WeaponType.Bow:
                SetHomingSpeed(spawnedAbiliity, bowHomingSpeed);
            break;
          
        };
    }


    override public void EvaluateSecondaryAbilityAttribute(GameObject spawnedAbiliity)
    {
        switch (owner.GetWeaponType())
        {
            case WeaponType.Sword:
                SetHomingSpeed(spawnedAbiliity, swordHomingSpeed);
                break;
           
            case WeaponType.Staff:
                SpawnShieldProjectile();
                break;
        };
    }

    public void SpawnShieldProjectile()
    {
        Debug.Log("spawn homing" + gameObject);
        GameObject projObj = ObjectPoolManager.Spawn(ShieldHomingProjectilePrefab, owner.GetFirePoint().position, Quaternion.identity);
        IProjectile proj = projObj.GetComponent<IProjectile>();

        if (proj != null)
        {
            Debug.Log("begin homing set up" + gameObject);
            projObj.GetComponent<BubbleShield>().owner = owner.GetPlayerTransform().gameObject;
            proj.SetUpProjectile(shieldProjDamage, owner.GetFirePoint().up, shieldHomingSpeed, shieldHomingLifeTime,
                shieldHitPoints, owner.GetPlayerTransform().gameObject);
         
        }
    }

    public void SetHomingSpeed(GameObject projectile,float speed)
    {
        IProjectile proj = projectile.GetComponent<IProjectile>();

        if(proj != null)
        {
            proj.SetSpeed(speed);
        }
    }


    public void AddShield(GameObject shieldObj)
    {
        currentShield = shieldObj.GetComponent<BubbleShield>();
        if (currentShield)
        {
            currentShield.OnDestroy += ClearShield;
           
        }
    }

    public void ClearShield()
    {

        if (currentShield)
        {
            currentShield.OnDestroy -= ClearShield;
            currentShield = null;
        }

    }

    public float GetSwordSpeed() { return swordHomingSpeed; }

    public float GetBowSpeed() { return bowHomingSpeed; }
    protected override void OnDisable()
    {
        base.OnDisable();
        ClearShield();
    }

}
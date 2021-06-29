using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoenixAbsorbAttribute : Base_SkillAttribute
{
    [SerializeField] private GameObject swordAbsorbProjectilePrefab;
    [SerializeField] private float sword;
    [SerializeField] private GameObject bowAbsorbProjectilePrefab;
    [SerializeField] private float swordHomingSpeed;

    [SerializeField] private GameObject swordAbsorbandHomingProjectilePrefab;
    [SerializeField] private GameObject bowAbsorbandHomingProjectilePrefab;
     private float bowHomingSpeed;


    [SerializeField] private GameObject absorbsiveShieldPrefab;
    bool isHoming;
    public override void SetUpAttribute(Base_Weapon weaponOwner)
    {
        base.SetUpAttribute(weaponOwner);
        HomingProjectile homingProj;
        switch (weaponOwner.GetWeaponType())
        {
            case WeaponType.Sword:
                homingProj = owner.GetPrimaryProjectilePrefab().GetComponent<HomingProjectile>();
          
                if (homingProj)
                {
                    isHoming = true;
                    UseHomingPrefabs();
                    HomingSkillAttribute homeSkill = owner.GetComponentInChildren<HomingSkillAttribute>();
                    if (homeSkill)
                        bowHomingSpeed = homeSkill.GetBowSpeed();
                }
                else
                {
                    UsDefaultPrefabs();
                }

                break;
            case WeaponType.Bow:
                homingProj = owner.GetPrimaryProjectilePrefab().GetComponent<HomingProjectile>();

                if (homingProj)
                {
                    isHoming = true;
                    UseHomingPrefabs();
                    HomingSkillAttribute homeSkill = owner.GetComponentInChildren<HomingSkillAttribute>();
                    if (homeSkill)
                        bowHomingSpeed = homeSkill.GetBowSpeed();
                }
                else
                {
                    UsDefaultPrefabs();
                }

                break;

            case WeaponType.Staff:

                owner.SetSecondaryProjectile(absorbsiveShieldPrefab);
                break;
        }
    }



    public override void EvaluatePrimaryAbilityAttribute(GameObject spawnedAbiliity)
    {
        if (owner.GetWeaponType() != WeaponType.Staff && owner.GetWeaponType() != WeaponType.none)
        {
            if (isHoming) SetUpHomingProjectile(owner.GetWeaponType(), spawnedAbiliity);
      
        }
       


    }

    public void UseHomingPrefabs()
    {
        HomingSkillAttribute homeSkill;
        switch (owner.GetWeaponType())
        {
            case WeaponType.Sword:
           
                owner.SetPrimaryProjectilePrefab(swordAbsorbandHomingProjectilePrefab);
                homeSkill = owner.GetComponentInChildren<HomingSkillAttribute>();
                if (homeSkill)
                {
           
                    isHoming = true;

                }
                break;
            case WeaponType.Bow:

                owner.SetPrimaryProjectilePrefab(bowAbsorbandHomingProjectilePrefab);
                homeSkill = owner.GetComponentInChildren<HomingSkillAttribute>();
                if (homeSkill)
                {
                    bowHomingSpeed = homeSkill.GetBowSpeed();
                    isHoming = true;

                }

                break;

        }
    }

    public void UsDefaultPrefabs()
    {
        switch (owner.GetWeaponType())
        {
            case WeaponType.Sword:

                owner.SetPrimaryProjectilePrefab(swordAbsorbProjectilePrefab);


                break;
            case WeaponType.Bow:

                owner.SetPrimaryProjectilePrefab(bowAbsorbProjectilePrefab);

                break;

        }
    }


    public void SetUpHomingProjectile(WeaponType type, GameObject ogProjectile)
    {
        IProjectile oldProj = ogProjectile.GetComponent<IProjectile>();

        OmniSlash slaskSkill = transform.parent.GetComponentInChildren<OmniSlash>();
        if (slaskSkill) return;
        if (oldProj != null)
        {
            ProjectileData data = oldProj.GetProjectileData();

         
            switch (type)
            {
                case WeaponType.Sword:



                    oldProj.SetUpProjectile(data.damage, data.dir, swordHomingSpeed, data.lifeTime, data.blockCount, data.owner);
             
                    break;
                case WeaponType.Bow:
                  

                    oldProj.SetUpProjectile(data.damage, data.dir, bowHomingSpeed, data.lifeTime, data.blockCount, data.owner);
            
                    break;
            
            }
        }



    }

 
}
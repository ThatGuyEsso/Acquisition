using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentSkill : Base_SkillAttribute
{
    [SerializeField] private GameObject swordFragPrefab;
    [SerializeField] private GameObject bowFragPrefab;
    [SerializeField] private GameObject shieldFragPrefab;

    [SerializeField] private int fragmentCount;
    [SerializeField] private float fragentSpeed;
    [SerializeField] private float fragmentLifeTime;


    private ProjectileFragmentSpawner staffSpawner;
    override public void SetUpAttribute(Base_Weapon weaponOwner)
    {
        base.SetUpAttribute(weaponOwner);

        if (owner.GetWeaponType()==WeaponType.Staff)
        {
            staffSpawner = owner.GetPlayerTransform().gameObject.AddComponent<ProjectileFragmentSpawner>();
            staffSpawner.SetUpFragments(shieldFragPrefab, fragmentCount, fragentSpeed, fragmentLifeTime,
             360f, false, owner.GetPlayerTransform().gameObject);
        }
    }

    override public void EvaluatePrimaryAbilityAttribute(GameObject spawnedAbiliity)
    {
        if (owner.GetWeaponType()==WeaponType.Bow)
        {
            AttatchFragmentController(owner.GetWeaponType(), spawnedAbiliity);
        }
    }
    override public void EvaluateSecondaryAbilityAttribute(GameObject spawnedAbiliity)
    {
        switch (owner.GetWeaponType())
        {
            case WeaponType.Sword:
                AttatchFragmentController(WeaponType.Sword, spawnedAbiliity);
                break;
            case WeaponType.Bow:
                break;
            case WeaponType.Staff:

                staffSpawner.SpawnProjectileFragment(owner.GetPlayerTransform().position,1.5f);
                break;
        };
    }

    public void AttatchFragmentController(WeaponType type,GameObject projectile)
    {
        switch (type)
        {
            case WeaponType.Sword:
                projectile.AddComponent<ProjectileFragmentSpawner>().SetUpFragments(swordFragPrefab, fragmentCount, fragentSpeed, fragmentLifeTime,
                   360f, true, owner.GetPlayerTransform().gameObject);

                break;
            case WeaponType.Bow:
                projectile.AddComponent<ProjectileFragmentSpawner>().SetUpFragments(bowFragPrefab, fragmentCount, fragentSpeed, fragmentLifeTime,
              360f, true, owner.GetPlayerTransform().gameObject);
                break;
          
        };
    }


    override protected  void OnDisable()
    {
        if (owner)
        {
            if(owner.GetWeaponType()== WeaponType.Staff){

                if (staffSpawner)
                    Destroy(staffSpawner);
                staffSpawner = null;
           
            }
        }
    }
}

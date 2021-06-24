using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_SkillAttribute : MonoBehaviour
{
    protected Base_Weapon owner;

    virtual public void SetUpAttribute(Base_Weapon weaponOwner)
    {
        owner = weaponOwner;
        owner.OnPrimaryAttack += EvaluatePrimaryAttackAttribute;
        owner.OnPrimaryAbility += EvaluatePrimaryAbilityAttribute;

        owner.OnSecondaryAttack += EvaluateSecondaryAttackAttribute;
        owner.OnSecondaryAbility += EvaluateSecondaryAbilityAttribute;
    }


    virtual public void EvaluatePrimaryAttackAttribute()
    {
        switch (owner.GetWeaponType())
        {
            case WeaponType.Sword:
                break;
            case WeaponType.Bow:
                break;
            case WeaponType.Staff:
                break;
        };
    }

    virtual public void EvaluatePrimaryAbilityAttribute(GameObject spawnedAbiliity)
    {
        switch (owner.GetWeaponType())
        {
            case WeaponType.Sword:
                break;
            case WeaponType.Bow:
                break;
            case WeaponType.Staff:
                break;
        };
    }
    virtual public void EvaluateSecondaryAttackAttribute()
    {
        switch (owner.GetWeaponType())
        {
            case WeaponType.Sword:
                break;
            case WeaponType.Bow:
                break;
            case WeaponType.Staff:
                break;
        };
    }

    virtual public void EvaluateSecondaryAbilityAttribute(GameObject spawnedAbiliity)
    {
        switch (owner.GetWeaponType())
        {
            case WeaponType.Sword:
                break;
            case WeaponType.Bow:
                break;
            case WeaponType.Staff:
                break;
        };
    }

    virtual protected void OnEnable()
    {
        if (owner)
        {
            owner.OnPrimaryAttack += EvaluatePrimaryAttackAttribute;
            owner.OnPrimaryAbility += EvaluatePrimaryAbilityAttribute;

            owner.OnSecondaryAttack += EvaluateSecondaryAttackAttribute;
            owner.OnSecondaryAbility += EvaluateSecondaryAbilityAttribute;
        }
    }

    virtual protected void OnDisable()
    {
        if (owner)
        {
            owner.OnPrimaryAttack -= EvaluatePrimaryAttackAttribute;
            owner.OnPrimaryAbility -= EvaluatePrimaryAbilityAttribute;

            owner.OnSecondaryAttack -= EvaluateSecondaryAttackAttribute;
            owner.OnSecondaryAbility -= EvaluateSecondaryAbilityAttribute;
        }
    }
}

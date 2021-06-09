using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Weapon : Base_Weapon
{
    [Header("Sword Settings")]
    [SerializeField] private DynamicConeCollider swordCollider;
    private DynamicConeCollider sCollider;


    protected override void PrimaryAttack()
    {
        if (!isWeaponActive)
            return;

        sCollider = Instantiate(swordCollider);
        sCollider.SetColliderShape(firePoint.transform.up, 1.5f, 90, transform.position);
    }

    protected override void SecondaryAttack()
    {
        
    }

}

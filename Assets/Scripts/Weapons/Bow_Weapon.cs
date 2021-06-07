using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow_Weapon : Base_Weapon
{
    [SerializeField] private GameObject firePoint;

    protected override void PrimaryAttack()
    {
        GameObject go = Instantiate(Settings.primaryProjectile.gameObject, firePoint.transform.position, Quaternion.identity);
        go.GetComponent<Base_Projectile>().SetUp(firePoint.transform.up.normalized);
    }

    protected override void SecondaryAttack()
    {
        
    }
}

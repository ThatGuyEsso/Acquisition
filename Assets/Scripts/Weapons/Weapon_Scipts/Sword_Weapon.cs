using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Weapon : Base_Weapon
{
    [Header("Sword Settings")]
    [SerializeField] private DynamicConeCollider swordCollider;
    [SerializeField] private float swordProjectileSpawnTime = 0.2f;
    [SerializeField] private float primaryCollisionRadius = 1.5f;
    [SerializeField] private float secondaryCollisionRadius = 2.5f;
    private DynamicConeCollider sCollider;


    protected override void PrimaryAttack()
    {
        if (!isWeaponActive)
            return;

        if (!canPrimaryFire)
            return;

        sCollider = Instantiate(swordCollider);
        sCollider.SetColliderShape(firePoint.transform.up, primaryCollisionRadius, 90, transform.position);

        StartCoroutine(FireSwordProjectile(primaryProjectile, swordProjectileSpawnTime));
        StartCoroutine(WaitForFirePrimaryRate(primaryFireRate));
    }

    private IEnumerator FireSwordProjectile(Base_Projectile prefab, float time)
    {
        yield return new WaitForSeconds(time);
        ObjectPoolManager.Spawn(prefab, firePoint.transform.position, Quaternion.identity);
    }


    protected override void SecondaryAttack()
    {
        if (!isWeaponActive)
            return;

        if (!canPrimaryFire)
            return;

        sCollider = Instantiate(swordCollider);
        sCollider.SetColliderShape(firePoint.transform.up, secondaryCollisionRadius, 90, transform.position);

        StartCoroutine(FireSwordProjectile(secondaryProjectile, swordProjectileSpawnTime));
        StartCoroutine(WaitForFireSecondaryRate(secondaryFireRate));       
    }



}

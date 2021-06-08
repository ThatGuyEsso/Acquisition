using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow_Weapon : Base_Weapon
{
    [SerializeField] private float SecondaryCharge = 5;
    [SerializeField] private float SecondaryChargeAfterDuration = 2;
    [SerializeField] private float chargeDistance = 100;

    protected override void PrimaryAttack()
    {
        if (canFire == false)
            return;

        GameObject go = Instantiate(settings.primaryProjectile.gameObject, firePoint.transform.position, Quaternion.identity);
        go.GetComponent<Base_Projectile>().SetUp(firePoint.transform.up.normalized);

        base.PrimaryAttack();
    }

    protected override void SecondaryAttack()
    {
        if (canFire == false)
            return;

        StartCoroutine(ChargeDuration());
        canFire = false;

        base.SecondaryAttack();
    }

    
    private IEnumerator ChargeDuration()
    {
        yield return new WaitForSeconds(SecondaryCharge);
        ChargeShot();

        yield return new WaitForSeconds(SecondaryChargeAfterDuration);
        canFire = true;
    }

    private void ChargeShot()
    {
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(firePoint.transform.position, firePoint.transform.up, chargeDistance);

        Debug.DrawRay(firePoint.transform.position, firePoint.transform.up * chargeDistance, Color.green, 10);

        if (hits == null)
            return;

        foreach(RaycastHit2D hit in hits)
        {
            Debug.Log(hit.collider.gameObject);
        }

    }

}

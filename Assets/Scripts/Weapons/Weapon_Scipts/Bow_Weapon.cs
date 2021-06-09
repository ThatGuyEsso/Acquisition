using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow_Weapon : Base_Weapon
{
    [Header("Bow Settings")]
    [SerializeField] private float SecondaryChargeTime = 5;
    [SerializeField] private float SecondaryAfterDuration = 2;
    [SerializeField] private float chargeDistance = 100;

    private float nextTimeToFire = 0;
    private LineRenderer line;

    public override void Init()
    {
        base.Init();
        line = GetComponent<LineRenderer>();
        line.enabled = false;
    }

    protected override void PrimaryAttack()
    {
        if (!isWeaponActive)
            return;

        if(canPrimaryFire == true)
        {
            isFiringPrimary = true;
            canPrimaryFire = false;
            
        }
        else if(canPrimaryFire == false)
        {
            isFiringPrimary = false;
            canPrimaryFire = true;
            
        }


    }

    private void Update()
    {
        if(isFiringPrimary == true && Time.time >= nextTimeToFire)
        {
            //Spawn Projectile
            GameObject go = ObjectPoolManager.Spawn(primaryProjectile.gameObject, firePoint.transform.position, Quaternion.identity);
            go.GetComponent<IProjectile>().SetUpProjectile(0f, firePoint.transform.up.normalized, 10f, 10f, 0, transform.parent.gameObject);

            nextTimeToFire = Time.time + primaryFireRate; //Added Time onto firerate
        }
    }
    protected override void SecondaryAttack()
    {
        if (isWeaponActive == false)
            return;

        if (!canSecondaryFire)
            return;

        StartCoroutine(ChargeDuration());
        base.SecondaryAttack();
    }

    private IEnumerator ChargeDuration()
    {
        canSecondaryFire = false;
        yield return new WaitForSeconds(SecondaryChargeTime);
        ChargeShot();

        yield return new WaitForSeconds(SecondaryAfterDuration);
        line.enabled = false;
        canSecondaryFire = true;
    }

    private void ChargeShot()
    {
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(firePoint.transform.position, firePoint.transform.up, chargeDistance);

        Debug.DrawRay(firePoint.transform.position, firePoint.transform.up * chargeDistance, Color.green, 10);

        if (hits == null)
            return;

        //Enable Line and set origin and destination of line
        line.enabled = true;
        line.SetPosition(0, firePoint.transform.position);
        Vector3 dest = firePoint.transform.position + firePoint.transform.up * chargeDistance;
        line.SetPosition(1, dest);

        foreach(RaycastHit2D hit in hits)
        {
            Debug.Log(hit.collider.gameObject);
        }

    }

}

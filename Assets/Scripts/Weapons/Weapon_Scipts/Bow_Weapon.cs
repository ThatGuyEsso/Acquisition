using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow_Weapon : Base_Weapon
{
    [Header("Bow Settings")]
    [SerializeField] private float primaryShotSpeed;
    [SerializeField] private float primaryShotKnockBack;
    [SerializeField] private float primaryShotLifeTime;
    [SerializeField] private float SecondaryChargeTime = 5;
    [SerializeField] private float SecondaryAfterDuration = 2;
    [SerializeField] private float chargeDistance = 100;

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

        if (!canPrimaryFire) return;

        if (isBusy) return;


        isBusy = true;
        isIdle = false;
        attackEvents.OnShootProjectile += OnFireProjectile;
        attackEvents.OnAnimEnd += ResetPrimaryFire;
        animSolver.PlayAnimationFromStart("Primary_Bow");

    }


    private void Update()
    {
        if (!isIdle)
        {
            if (currTimeToIdle <= 0f)
            {
                isIdle = true;
                if (isRunning)
                {
                    animSolver.PlayAnimation("Run_Bow");
                    Debug.Log("run");
                }
                else
                {
                    animSolver.PlayAnimation("Idle_Bow");
                }
                currTimeToIdle = timeToIdle;
            }
            else
            {
                currTimeToIdle -= Time.deltaTime;
            }
        }
    }

    public override void OnFireProjectile()
    {
        GameObject go = ObjectPoolManager.Spawn(primaryProjectile, firePoint.transform.position, Quaternion.identity);
        go.GetComponent<IProjectile>().SetUpProjectile(primaryAttackDamage, firePoint.transform.up.normalized, primaryShotLifeTime, 10f, 0,playerTransform.gameObject);
        attackEvents.OnShootProjectile -= OnFireProjectile;
        Debug.Log("Fire");
    }

    public override void Equip(Transform firePoint, AttackAnimEventListener eventListener, Transform player, TopPlayerGFXSolver solver)
    {
        base.Equip(firePoint, eventListener, player, solver);
    }
    public override void UnEquip()
    {
        base.UnEquip();
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

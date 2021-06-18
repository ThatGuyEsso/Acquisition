using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeChaos : BaseBossAbility
{
 
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected float projectileLifeTime;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected int projectileBlockCount;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float attackDuration;
    [SerializeField] protected float rotationRate=250f;
    bool isAttacking;
    float currTimeToShoot=0;
    float curretAttackTime;
    public void Update()
    {
        if (isAttacking)
        {
            owner.transform.Rotate(new Vector3(0f, 0f, Time.deltaTime * rotationRate));
            if (curretAttackTime > 0)
            {
                curretAttackTime -= Time.deltaTime;
                if (currTimeToShoot <= 0)
                {
                    ShootProjectile();
                    currTimeToShoot = fireRate;

                }
                else
                {
                    currTimeToShoot -= Time.deltaTime;
                }
            }
            else
            {
                StopBladeCircus();
            }
        }
    }

    public void StartBladeCircus()
    {
        attacksLeft--;
        canAttack = false;
        isAttacking = true;
        owner.PlayAnimation("BladeChaos");
    }
    public void StopBladeCircus()
    {
        isAttacking = false;
        eventListener.OnAnimEnd += EvaluateEnd;
        owner.PlayAnimation("BladeChaosEnd");
        

    }

    public void EvaluateEnd()
    {
        eventListener.OnAnimEnd -= EvaluateEnd;
        if (attacksLeft <= 0)
        {
            StopAllCoroutines();
            owner.CycleToNextAttack();
            owner.SetIsBusy(false);
            StartCoroutine(BeginResetAbility(coolDown));

        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(BeginRefreshAttack(attackRate));
        }
    }

    public void ShootProjectile()
    {
        GameObject projectile = ObjectPoolManager.Spawn(projectilePrefab, owner.GetFirePoint().position, Quaternion.identity);
        projectile.GetComponent<IInitialisable>().Init();
        projectile.GetComponent<IProjectile>().SetUpProjectile(1.0f, owner.GetFirePoint().up,
            projectileSpeed, projectileLifeTime,  projectileBlockCount, owner.gameObject);
    }

    public override void EnableAbility()
    {
        base.EnableAbility();
        curretAttackTime = attackDuration;
        if (eventListener) eventListener.OnShowAttackZone += StartBladeCircus;

    }
}

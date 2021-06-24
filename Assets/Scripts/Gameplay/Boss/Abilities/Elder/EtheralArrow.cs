using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtheralArrow : BaseBossAbility,IInitialisable
{
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected float projectileLifeTime;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected int projectileBlockCount;


    public void ShootProjectile()
    {
   
        attacksLeft--;
        GameObject projectileObject = ObjectPoolManager.Spawn(projectilePrefab, owner.GetFirePoint().position, Quaternion.identity);
        IInitialisable init = projectileObject.GetComponent<IInitialisable>();
        IProjectile projectile = projectileObject.GetComponent<IProjectile>();
        if (init != null) init.Init();
        else
        {
            EvaluateRemainingAttacks();
            if (projectileObject) ObjectPoolManager.Recycle(projectileObject);
            return;
        }
        if (projectile != null)
        {
            projectile.SetUpProjectile(1.0f, owner.GetFirePoint().up, projectileSpeed, projectileLifeTime, projectileBlockCount, owner.gameObject);
            projectile.SetHomingTarget(owner.target);
        }
        else
        {
            EvaluateRemainingAttacks();
            if (projectileObject) ObjectPoolManager.Recycle(projectileObject);
            return;
        }

        EvaluateRemainingAttacks();


    }
    public void EvaluateRemainingAttacks()
    {
        if (attacksLeft <= 0)
        {
            eventListener.OnShootProjectile -= ShootProjectile;
            //Debug.Log("NO Attacks left");
            StopAllCoroutines();
            owner.CycleToNextAttack();
            StartCoroutine(BeginResetAbility(coolDown));

        }
        else
        {
            //Debug.Log("¨Prime next attack");
            StopAllCoroutines();
            StartCoroutine(BeginRefreshAttack(attackRate));
        }
    }
    override public void DisableAbility()
    {
        base.DisableAbility();

        eventListener.OnShootProjectile -= ShootProjectile;
 
    }

    public void Lockon()
    {
        eventListener.OnShowAttackZone -= Lockon;
        owner.SetCanLockOn(false);
    }

    override public void EnableAbility()
    {
        base.EnableAbility();
        if (eventListener)
        {
            eventListener.OnShowAttackZone += Lockon;
            eventListener.OnShootProjectile += ShootProjectile;
        }
    }
}

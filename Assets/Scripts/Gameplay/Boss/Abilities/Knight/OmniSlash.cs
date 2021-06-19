using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmniSlash : BaseBossAbility
{

    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected GameObject attackZonePrefab;
    [SerializeField] protected float projectileLifeTime;
    [SerializeField] protected int projectileBlockCount;


    protected AttackVolume attackZone;

    public override void Init()
    {
        base.Init();
        owner.ToggleCanAttack(true);

    }
    public void DoOmniSlash()
    {
        canAttack = false;
        attacksLeft--;
        eventListener.OnShowAttackZone -= DoOmniSlash;
        eventListener.OnShootProjectile += SpawnSlashProjectile;
        eventListener.OnHideAttackZone += RemoveAttackZone;
        CreateAttackZone();
        owner.PlayAnimation("OmniSlash");


    }

    public void CreateAttackZone()
    {
        if (attackZonePrefab)
        {

            attackZone = ObjectPoolManager.Spawn(attackZonePrefab, owner.transform.position, Quaternion.identity).GetComponent<AttackVolume>();
            if (attackZone)
            {
                attackZone.SetIsPlayerZone(false);
                attackZone.SetUpDamageVolume(1f, 10f, owner.transform.up, owner.gameObject);

            }
        }
    }


    public void RemoveAttackZone()
    {
        if (attackZone)
        {
            ObjectPoolManager.Recycle(attackZone.gameObject);
            attackZone = null;
        }
        eventListener.OnShootProjectile -= SpawnSlashProjectile;
        eventListener.OnHideAttackZone -= RemoveAttackZone;
        eventListener.OnAnimEnd += EvaluateAttack;
        owner.PlayAnimation("BladeChaosEnd");
    }
    public void EvaluateAttack()
    {
        eventListener.OnAnimEnd -= EvaluateAttack;
        if (attacksLeft <= 0)
        {
            StopAllCoroutines();
            owner.CycleToNextAttack();
            StartCoroutine(BeginResetAbility(coolDown));

        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(BeginRefreshAttack(attackRate));
        }
    }


    public void SpawnSlashProjectile()
    {
        if (ObjectPoolManager.instance)
        {
            GameObject projectile = ObjectPoolManager.Spawn(projectilePrefab, owner.GetFirePoint().position, Quaternion.identity);
            projectile.GetComponent<IInitialisable>().Init();
            projectile.GetComponent<IProjectile>().SetUpProjectile(1.0f, owner.GetFirePoint().up, 0.0f, projectileLifeTime, projectileBlockCount, owner.gameObject);
        }

    }
    public override void EnableAbility()
    {
        base.EnableAbility();
        if (!eventListener) return;
        if (isSuperCloseRange)
        {
            eventListener.OnAnimEnd += DisableAbility;
        }
        eventListener.OnShowAttackZone += DoOmniSlash;


    }

    public override void DisableAbility()
    {
        base.DisableAbility();
        if (!eventListener) return;
        if (isSuperCloseRange)
        {
            eventListener.OnAnimEnd -= DisableAbility;
        }
        eventListener.OnShootProjectile -= DoOmniSlash;

    }
}

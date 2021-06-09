using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmniSlash : BaseBossAbility
{

    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected float projectileLifeTime;
    [SerializeField] protected int projectileBlockCount;


    public override void Init()
    {
        base.Init();
        owner.ToggleCanAttack(true);

    }
    public void DoOmniSlash()
    {
        canAttack = false;
        attacksLeft--;
        GameObject projectile = ObjectPoolManager.Spawn(projectilePrefab, owner.GetFirePoint().position, Quaternion.identity);
        projectile.GetComponent<IInitialisable>().Init();
        projectile.GetComponent<IProjectile>().SetUpProjectile(1.0f, owner.GetFirePoint().up, 0.0f, projectileLifeTime, projectileBlockCount, owner.gameObject);

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
    public override void EnableAbility()
    {
        base.EnableAbility();
        if (!eventListener) return;
        if (isSuperCloseRange)
        {
            eventListener.OnAnimEnd += DisableAbility;
        }
        eventListener.OnShootProjectile += DoOmniSlash;


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

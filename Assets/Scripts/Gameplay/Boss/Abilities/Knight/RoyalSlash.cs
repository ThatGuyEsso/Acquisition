using UnityEngine;

public class RoyalSlash : BaseBossAbility,IInitialisable
{

    [SerializeField] protected GameObject attackAreaPrefab;
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected float attackRadius;
    [SerializeField] protected float maxAttackAngle;

    [SerializeField] protected float projectileLifeTime;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected int projectileBlockCount;
    public override void Init()
    {
        base.Init();
        canAttack = true;

    }
    public void ShootProjectile()
    {
        GameObject projectile = ObjectPoolManager.Spawn(projectilePrefab, owner.GetFirePoint().position, Quaternion.identity);
        projectile.GetComponent<IInitialisable>().Init();
        projectile.GetComponent<IProjectile>().SetUpProjectile(1.0f, owner.GetFirePoint().up, projectileSpeed, projectileLifeTime, projectileBlockCount,owner.gameObject);
    }
    public void CreateAttackZone()
    {
        canAttack = false;
        Debug.Log("Attack left before= "+attacksLeft);
        attacksLeft--;
        Debug.Log("Attack left after= " + attacksLeft);
        dynamicAttackZone = ObjectPoolManager.Spawn(attackAreaPrefab, Vector3.zero, Quaternion.identity).GetComponent<DynamicConeCollider>();
      
        dynamicAttackZone.SetColliderShape(owner.GetFirePoint().up, attackRadius, maxAttackAngle, owner.transform.position);
    }




    public void RemoveAttackZone()
    {
        if (dynamicAttackZone)
        {
            ObjectPoolManager.Recycle(dynamicAttackZone.gameObject);

            dynamicAttackZone = null;
        }

            //Debug.Log("Evaluate Attacks");
        if (attacksLeft <= 0)
        {
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
        eventListener.OnShowAttackZone -= CreateAttackZone;
        eventListener.OnHideAttackZone -= RemoveAttackZone;
        eventListener.OnShootProjectile -= ShootProjectile;
        if (dynamicAttackZone)
        {
            ObjectPoolManager.Recycle(dynamicAttackZone.gameObject);

            dynamicAttackZone = null;
        }
    }

    override public void EnableAbility()
    {
        base.EnableAbility();
        if (eventListener)
        {
            eventListener.OnShowAttackZone += CreateAttackZone;
            eventListener.OnHideAttackZone += RemoveAttackZone;
            eventListener.OnShootProjectile += ShootProjectile;
        }
    }


}

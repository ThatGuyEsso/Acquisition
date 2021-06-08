using UnityEngine;

public class RoyalSlash : BaseBossAbility
{

    [SerializeField] protected GameObject attackAreaPrefab;
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected float attackRadius;
    [SerializeField] protected float maxAttackAngle;
    [SerializeField] protected Transform fp;
    [SerializeField] protected float projectileLifeTime;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected int projectileBlockCount;
    public void ShootProjectile()
    {
        GameObject projectile = ObjectPoolManager.Spawn(projectilePrefab, fp.position, Quaternion.identity);
        projectile.GetComponent<IProjectile>().SetUpProjectile(1.0f, owner.transform.up, projectileSpeed, projectileLifeTime, projectileBlockCount,owner.gameObject);
    }
    public void CreateAttackZone()
    {
        canAttack = false;
        attacksLeft--;
        DynamicConeCollider attackZone = ObjectPoolManager.Spawn(attackAreaPrefab, Vector3.zero, Quaternion.identity).GetComponent<DynamicConeCollider>();
        attackZone.SetColliderShape(owner.transform.up, attackRadius, maxAttackAngle, owner.transform.position);
    }




    public void RemoveAttackZone()
    {
        if (attackZone)
        {
            ObjectPoolManager.Recycle(attackZone.gameObject);

            attackZone = null;
        }
          
        if (attacksLeft <= 0)
        {
            owner.BeginLongCooldown(coolDown);
        }
        else
        {
            Invoke("ResetAttack",attackRate);
        }
    }

    public void ResetAttack()
    {
        canAttack = true;
    }



    virtual protected void OnEnable()
    {
        if (eventListener)
        {
            eventListener.OnShowAttackZone += CreateAttackZone;
            eventListener.OnHideAttackZone += RemoveAttackZone;
            eventListener.OnShootProjectile += ShootProjectile;
        }
    }
 

    protected void OnDisable()
    {
        if (eventListener)
        {
            eventListener.OnShowAttackZone -= CreateAttackZone;
            eventListener.OnHideAttackZone -= RemoveAttackZone;
            eventListener.OnShootProjectile -= ShootProjectile;
        }
    }
}

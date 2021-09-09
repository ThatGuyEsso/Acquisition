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
    private AfterImageController afterImageController;
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
        OnProjectileSpawned?.Invoke(projectile);
    }
    public void CreateAttackZone()
    {
        if (afterImageController) afterImageController.StartDrawing();
        if (AudioManager.instance)
            AudioManager.instance.PlayThroughAudioPlayer("KnightSwing", owner.transform.position,true);
        canAttack = false;
        Debug.Log("Attack left before= "+attacksLeft);
        attacksLeft--;
        Debug.Log("Attack left after= " + attacksLeft);
        dynamicAttackZone = ObjectPoolManager.Spawn(attackAreaPrefab, Vector3.zero, Quaternion.identity).GetComponent<DynamicConeCollider>();
      
        dynamicAttackZone.SetColliderShape(owner.GetFirePoint().up, attackRadius, maxAttackAngle, owner.transform.position);
        AttackVolume attackVol = dynamicAttackZone.GetComponent<AttackVolume>();
        if (attackVol) attackVol.SetDespawnTime(1f);
        if (CamShake.instance) CamShake.instance.DoScreenShake(0.25f, 2f, 0f, 0.05f, 2f);
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
            if (eventListener)
            {
                eventListener.OnShowAttackZone -= CreateAttackZone;
                eventListener.OnHideAttackZone -= RemoveAttackZone;
                eventListener.OnShootProjectile -= ShootProjectile;
            }
            StartCoroutine(BeginResetAbility(coolDown));
          
        }
        else
        {
            //Debug.Log("¨Prime next attack");
            StopAllCoroutines();
            StartCoroutine(BeginRefreshAttack(attackRate));
        }

        if (afterImageController) afterImageController.StopDrawing();
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
        if (afterImageController) afterImageController.StopDrawing();
   
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
        if (owner)
        {
            transform.rotation = owner.GetFirePoint().rotation;
            SpriteRenderer ownerRenderer = owner.GetRenderer();
            afterImageController = owner.GetAfterimageController();
            if (ownerRenderer && afterImageController) afterImageController.SetUpRenderer(ownerRenderer, 0.05f, 0.2f);
        }
    }


}

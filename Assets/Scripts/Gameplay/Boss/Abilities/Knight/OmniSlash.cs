using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmniSlash : BaseBossAbility
{

    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected GameObject attackZonePrefab;
    [SerializeField] protected float projectileLifeTime;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected int projectileBlockCount;


    protected AttackVolume attackZone;
    private AfterImageController afterImageController;
    public override void Init()
    {
        base.Init();
        owner.ToggleCanAttack(true);

    }
    public void DoOmniSlash()
    {

        if (AudioManager.instance)
            AudioManager.instance.PlayThroughAudioPlayer("KnightSwing", owner.transform.position);
        canAttack = false;
        attacksLeft--;
        eventListener.OnShowAttackZone -= DoOmniSlash;
        eventListener.OnShootProjectile += SpawnSlashProjectile;
        eventListener.OnShowAttackZone += CreateAttackZone;
        eventListener.OnHideAttackZone += RemoveAttackZone;
  
        owner.PlayAnimation("OmniSlash");


    }

    public void CreateAttackZone()
    {
        eventListener.OnShowAttackZone -= CreateAttackZone;
        if (afterImageController) afterImageController.StartDrawing();
        if (attackZonePrefab)
        {

            attackZone = ObjectPoolManager.Spawn(attackZonePrefab, owner.transform.position, Quaternion.identity).GetComponent<AttackVolume>();
            if (attackZone)
            {
                attackZone.SetIsPlayerZone(false);
                attackZone.SetUpDamageVolume(1f, 10f, owner.transform.up, owner.gameObject);
                if (CamShake.instance) CamShake.instance.DoScreenShake(0.25f, 3f, 0f, 0.15f, 2f);
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
        if (afterImageController) afterImageController.StopDrawing();
    }


    public void SpawnSlashProjectile()
    {
        if (ObjectPoolManager.instance)
        {
            GameObject projectile = ObjectPoolManager.Spawn(projectilePrefab, owner.GetFirePoint().position, Quaternion.identity);
            projectile.GetComponent<IInitialisable>().Init();
            projectile.GetComponent<IProjectile>().SetUpProjectile(1.0f, owner.GetFirePoint().up, projectileSpeed, projectileLifeTime, projectileBlockCount, owner.gameObject);
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
        if (owner)
        {
            transform.rotation = owner.GetFirePoint().rotation;
            SpriteRenderer ownerRenderer = owner.GetRenderer();
            afterImageController = owner.GetAfterimageController();
            if (ownerRenderer && afterImageController) afterImageController.SetUpRenderer(ownerRenderer, 0.25f, 0.6f);
        }

    }

    public override void DisableAbility()
    {
        base.DisableAbility();
        if (attackZone)
        {
            ObjectPoolManager.Recycle(attackZone.gameObject);
            attackZone = null;
        }
        if (afterImageController) afterImageController.StopDrawing();
        if (!eventListener) return;
        if (isSuperCloseRange)
        {
            eventListener.OnAnimEnd -= DisableAbility;
        }
        eventListener.OnShowAttackZone -= CreateAttackZone;
        eventListener.OnShowAttackZone -= DoOmniSlash;
     

    }

}


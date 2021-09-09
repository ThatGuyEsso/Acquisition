using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeChaos : BaseBossAbility
{
 
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected GameObject attackZonePrefab;
    [SerializeField] protected float projectileLifeTime;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected int projectileBlockCount;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float attackDuration;
    [SerializeField] protected float rotationRate=250f;

    [SerializeField] private float sfxPlayRate;
    private float currTimeToSFX;
    private AfterImageController afterImageController;
    bool isAttacking;
    float currTimeToShoot=0;
    float curretAttackTime;
    protected AttackVolume attackZone;
    public void Update()
    {
        if (isAttacking)
        {
            owner.transform.Rotate(new Vector3(0f, 0f, -Time.deltaTime * rotationRate));
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


            if (currTimeToSFX <= 0)
            {

                if (AudioManager.instance)
                    AudioManager.instance.PlayThroughAudioPlayer("KnightSwing", owner.transform.position, true);
                currTimeToSFX = sfxPlayRate;
            }
            else
            {
                currTimeToSFX -= Time.deltaTime;
            }
        
        }
    }
    public void CreateAttackZone()
    {
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

    }
    public void StartBladeCircus()
    {
        if (afterImageController) afterImageController.StartDrawing();
        if (eventListener) eventListener.OnShowAttackZone -= StartBladeCircus;
        attacksLeft--;
        canAttack = false;
        isAttacking = true;
        owner.PlayAnimation("BladeChaos");
        Invoke("CreateAttackZone", 1f);
    }
    public void StopBladeCircus()
    {
        RemoveAttackZone();
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
        if (afterImageController) afterImageController.StopDrawing();
    }

    public void ShootProjectile()
    {

        GameObject projectile = ObjectPoolManager.Spawn(projectilePrefab, owner.GetFirePoint().position, Quaternion.identity);
        projectile.GetComponent<IInitialisable>().Init();
        projectile.GetComponent<IProjectile>().SetUpProjectile(1.0f, owner.GetFirePoint().up,
            projectileSpeed, projectileLifeTime,  projectileBlockCount, owner.gameObject);

        OnProjectileSpawned?.Invoke(projectile);
    }

    private void OnDisable()
    {
        RemoveAttackZone();
    }

    public override void DisableAbility()
    {
        base.DisableAbility();
        isAttacking = false;
        StopAllCoroutines();
        eventListener.OnAnimEnd -= EvaluateEnd;
        eventListener.OnShowAttackZone -= StartBladeCircus;
        RemoveAttackZone();
        if (afterImageController) afterImageController.StopDrawing();
    }
    public override void EnableAbility()
    {
        base.EnableAbility();
        curretAttackTime = attackDuration;
        if (eventListener) eventListener.OnShowAttackZone += StartBladeCircus;
        if (owner)
        {
            transform.rotation = owner.GetFirePoint().rotation;
            SpriteRenderer ownerRenderer = owner.GetRenderer();
            afterImageController = owner.GetAfterimageController();
            if (ownerRenderer && afterImageController) afterImageController.SetUpRenderer(ownerRenderer, 0.25f, 1f);
        }
    }
}

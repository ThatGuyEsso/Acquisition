using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbSiveBubbleShield : BubbleShield
{
    private InLargeOverTime sizeController;

    [SerializeField] private float absorbDamageIncrement;
    [SerializeField] private float absorbSizeIncrement;
    [SerializeField] private int maxAbsorbCount;
    private int absorbCount;

    [SerializeField] private float maxSize;

    private Vector3 defaultSize;
    private Vector3 currTargetSize;
    private float defaultDamage;
    protected override void OnEnable()
    {
        base.OnEnable();
        absorbCount = 0;

        reflectionDamage = defaultDamage;
    }

    public override void Awake()
    {
        base.Awake();
        sizeController = GetComponent<InLargeOverTime>();
        defaultSize = transform.localScale;
        if (sizeController) sizeController.SetInitSize(defaultSize);
        absorbCount = 0;
        defaultDamage = reflectionDamage;
    }


    public override void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject == gameObject) return;
        if (other.gameObject.CompareTag("Projectiles"))
        {

            Debug.Log("Destroyed projectile" + gameObject);
            IProjectile projectile = other.GetComponent<IProjectile>();
    
            if (projectile != null)
            {
                OnAbsorbProjectile();
                if (projectile.GetOwner() != owner)
                {
                    OnRelfected?.Invoke(projectile.GetSelf());
                    ProjectileData data = projectile.GetProjectileData();
                    projectile.ResetProjectile();
                    projectile.SetUpProjectile(reflectionDamage, data.dir * -1f, data.speed, data.lifeTime, data.blockCount, owner);

                    if (BossRoomManager.instance)
                    {
                        if (BossRoomManager.instance.GetBoss())
                            projectile.SetHomingTarget(BossRoomManager.instance.GetBoss().transform);

                    }
                    else
                    {
                        BaseBossAI boss = FindObjectOfType<BaseBossAI>();
                        if (boss)
                        {
                            projectile.SetHomingTarget(boss.transform);

                        }
                        else
                        {
                            if (other) ObjectPoolManager.Recycle(other.gameObject);
                        }
                    }
                    if (AudioManager.instance) AudioManager.instance.PlayThroughAudioPlayer("ShieldHit", transform.position, true);
                    if (!isHurt)
                    {
                        isHurt = true;
               
                        if (currHitPoints < 0) KillShield();
                        else
                        {
                            if (flashVFX) flashVFX.Flash();
                        }
                    }
                }
            }
        }
    }
    public void OnAbsorbProjectile()
    {
        absorbCount++;
        reflectionDamage += absorbDamageIncrement;
        if (absorbCount > maxAbsorbCount) absorbCount = maxAbsorbCount;

        currTargetSize = transform.localScale + Vector3.one * absorbSizeIncrement;
        if (currTargetSize.x > maxSize)
        {
            currTargetSize = defaultSize + Vector3.one * maxSize;
           
        }
        if (sizeController)
        {
            sizeController.SetUpGrowSetting(currTargetSize.x, 3f, 0.05f);
            sizeController.StartGrowing();
        }



    }
}

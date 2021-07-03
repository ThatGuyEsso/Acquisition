using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EternalFireball : ProjectileGrenade
{
    private InLargeOverTime sizeController;

    [SerializeField] private float absorbSizeIncrement;
    [SerializeField] private int maxAbsorbCount;
    [SerializeField] private Animator animator;
    private int absorbCount;

    [SerializeField] private float maxSize;

    private Vector3 defaultSize;
    private Vector3 currTargetSize;
    private Collider2D trigger;
    bool isBusy;

   
    override public void OnDamage(float dmg, Vector2 kBackDir, float kBackMag, GameObject attacker)
    {
        if (!isHurt)
        {

 
            isHurt = true;
            OnAbsorbProjectile();
            if (flashVFX)
            {
                flashVFX = GetComponent<SpriteFlash>();
                flashVFX.Init();
            }
            if (flashVFX) flashVFX.Flash();
            if (AudioManager.instance) AudioManager.instance.PlayThroughAudioPlayer("ShieldHit", transform.position, true);
        }

    }


    protected override void Awake()
    {
        base.Awake();
        sizeController = GetComponent<InLargeOverTime>();
        defaultSize = transform.localScale;
        if (sizeController) sizeController.SetInitSize(defaultSize);
        canCreateFragments = true;
        isBusy = false;
        animator = GetComponent<Animator>();
        trigger = GetComponent<Collider2D>();
        absorbCount = 0;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        animator.Play("Idle");
        isBusy = false;
        absorbCount = 0;
        if (sizeController) sizeController.SetUpGrowSetting(2.5f, 2f, 0.2f);
        if (trigger)  trigger.enabled = true;
    }
    public override void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & destroyProjectileLayer) != 0)
        {
            if (AudioManager.instance)
            {
                AudioManager.instance.PlayThroughAudioPlayer(hitSFXname, transform.position, true);
            }
            canCreateFragments = true;
            KillProjectile();
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectiles"))
        {
            if (other.GetComponent<IProjectile>().GetOwner() != owner || owner != null)
            {
                other.GetComponent<IProjectile>().BreakProjectile();
            }
           
        }

        if (other.gameObject.CompareTag("Player"))
        {

            if (other.gameObject != owner && owner != null)
            {
                if (other.GetComponent<IDamage>() != null)
                {
                    other.GetComponent<IDamage>().OnDamage(projectileDamage, rb.velocity, knockback, owner);
                    canCreateFragments = false;
                    DoBreakVFX();
                    KillProjectile();
                }


            }
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            if (other.gameObject != owner)
            {
                if (other.GetComponent<IDamage>() != null)
                {
                    other.GetComponent<IDamage>().OnDamage(projectileDamage, rb.velocity, knockback, owner);
                    canCreateFragments = false;
                    DoBreakVFX();
                    KillProjectile();
                }
            }
        }


    }

    protected override void KillProjectile()
    {
        if (canCreateFragments&&!isBusy)
        {
            isBusy = true;
            rb.velocity = Vector2.zero;
            if (trigger) trigger.enabled = false;
            animator.Play("Explode",0,0f);
        }
        else if(!isBusy)
        {
            ClearProjectile();
        }

      
    }
    public void ClearProjectile()
    {
        if (ObjectPoolManager.instance)
        {
            if (gameObject)
                ObjectPoolManager.Recycle(gameObject);
        }
        else
        {
            if (gameObject)
                Destroy(gameObject);
        }
    }

    protected override void OnDisable()
    {
        OnKilled?.Invoke();
        if (isHurt)
        {
            isHurt = false;
            currHurtTime = hurtTime;
            if (flashVFX) flashVFX.CancelFlash();
        }
        if (GameManager.instance)
        {
            GameManager.instance.OnNewEvent -= EvaluateNewGameEvent;
        }

   
        StopAllCoroutines();
        absorbCount = 0;
    }

    public void OnAbsorbProjectile()
    {
        absorbCount++;
        if (absorbCount > maxAbsorbCount) absorbCount = maxAbsorbCount;

        currTargetSize = transform.localScale + Vector3.one * absorbSizeIncrement;
        if (currTargetSize.x > maxSize)
        {
            currTargetSize = defaultSize + Vector3.one * maxSize;
            KillProjectile();
        }
        if (sizeController)
        {
            sizeController.SetUpGrowSetting(currTargetSize.x, 3f, 0.05f);
            sizeController.StartGrowing();
        }
        


    }


    public override void CreateFragments()
    {
        int count = fragmentCount + absorbCount;
        float angleIncrement = spreadAngle / count;
        float currentAngle = 0f;
        GameObject currentFragment;
        for (int i = 0; i < count; i++)
        {
            currentFragment = ObjectPoolManager.Spawn(fragmentPreab, transform.position, Quaternion.identity);
            IProjectile projFrag = currentFragment.GetComponent<IProjectile>();
            if (projFrag != null)
            {
                Vector2 dir = EssoUtility.GetVectorFromAngle(currentAngle).normalized;
                projFrag.SetUpProjectile(1.0f, dir, fragmenteSpeed, fragmentLifeTime, count, owner);

                if (owner.GetComponent<IBoss>() != null)
                {
                    IBoss boss = owner.GetComponent<IBoss>();

                    projFrag.SetHomingTarget(boss.GetTarget());
                }
                else
                {
                    if (BossRoomManager.instance)
                    {
                        if (BossRoomManager.instance.GetBoss())
                            projFrag.SetHomingTarget(BossRoomManager.instance.GetBoss().transform);

                    }
                    else
                    {
                        BaseBossAI boss = FindObjectOfType<BaseBossAI>();
                        if (boss)
                        {
                            projFrag.SetHomingTarget(boss.transform);

                        }
                        else
                        {
                            if (currentFragment) ObjectPoolManager.Recycle(currentFragment);
                        }
                    }

                }
            }
            else
            {
                if (currentFragment)
                    ObjectPoolManager.Recycle(currentFragment);
            }

            currentAngle += angleIncrement;
        }
    }


  
}

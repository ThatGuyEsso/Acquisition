using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGrenade : Base_Projectile
{



    [SerializeField] protected GameObject fragmentPreab;
    [SerializeField] protected float fragmentLifeTime;
    [SerializeField] protected float fragmenteSpeed;
    [SerializeField] protected int fragmentBlockCount;
    [SerializeField] protected int fragmentCount;


    [Range(0f,360f)]
    [SerializeField] protected float spreadAngle;

    protected bool canCreateFragments;


    override public void OnDamage(float dmg, Vector2 kBackDir, float kBackMag, GameObject attacker)
    {
        if (!isHurt)
        {
            canCreateFragments = true;
            isHurt = true;
            if (attacker != owner) blockCount--;
            if (blockCount <= 0) KillProjectile();
            if (flashVFX)
            {
                flashVFX = GetComponent<SpriteFlash>();
                flashVFX.Init();
            }
            if (flashVFX) flashVFX.Flash();
            if (AudioManager.instance) AudioManager.instance.PlayThroughAudioPlayer("ProjectileHurt", transform.position, true);
        }

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
                if (other.GetComponent<IDamage>() != null)
                {
                    other.GetComponent<IDamage>().OnDamage(projectileDamage, rb.velocity, knockback, owner);

                }
            }
            else
            {
                Vector2 dir = other.transform.position - transform.position;
                other.GetComponent<IProjectile>().RepelProjectile(dir, allyRepelForce);
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
                    KillProjectile();
                }
            }
        }
    }



    protected override void KillProjectile()
    {
        if (canCreateFragments)
        {
            CreateFragments();
        }

        base.KillProjectile();
    }


    virtual public void CreateFragments()
    {
        float angleIncrement = spreadAngle / fragmentCount;
        float currentAngle = 0f;
        GameObject currentFragment;
        for (int i = 0; i < fragmentCount; i++)
        {
            currentFragment = ObjectPoolManager.Spawn(fragmentPreab, transform.position,Quaternion.identity);
            IProjectile projFrag = currentFragment.GetComponent<IProjectile>();
            if (projFrag!=null)
            {
                Vector2 dir = EssoUtility.GetVectorFromAngle(currentAngle).normalized;
                projFrag.SetUpProjectile(1.0f, dir, fragmenteSpeed, fragmentLifeTime,fragmentBlockCount, owner);

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


    override public void RepelProjectile(Vector2 dir, float force)
    {
       //
    }
    override protected IEnumerator LifeTimer(float time)
    {
        yield return new WaitForSeconds(time);
        canCreateFragments = true;
        KillProjectile();
    }
}

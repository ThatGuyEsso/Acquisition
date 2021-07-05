using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleBomb : ProjectileGrenade
{
    [SerializeField] private Animator animator;
    [SerializeField] private string idleAnim;
    [SerializeField] private string explodeAnim;
    private bool isDead;

    override protected void OnEnable()
    {
        base.OnEnable();
        animator.Play(idleAnim);
        isDead = false;
    }
    override protected IEnumerator LifeTimer(float time)
    {
        yield return new WaitForSeconds(time);
        canCreateFragments = true;
        DoExplosion();
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
            DoExplosion();
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectiles"))
        {
            if (other.GetComponent<IProjectile>().GetOwner() != owner || owner == null)
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

    public void DoExplosion()
    {
        StopAllCoroutines();
        rb.velocity = Vector2.zero;
        animator.Play(explodeAnim, 0, 0f);
    }
    override public void OnDamage(float dmg, Vector2 kBackDir, float kBackMag, GameObject attacker)
    {
        if (!isHurt&&!isDead)
        {
            canCreateFragments = true;
            isHurt = true;
            if (attacker != owner) blockCount--;
            if (blockCount <= 0) {
                animator.Play(explodeAnim, 0, 0f);
                isDead = true;
            }
            if (flashVFX)
            {
                flashVFX = GetComponent<SpriteFlash>();
                flashVFX.Init();
            }
            if (flashVFX) flashVFX.Flash();

            if (AudioManager.instance) AudioManager.instance.PlayThroughAudioPlayer("ProjectileHurt", transform.position, true);
        }

    }
    protected override void KillProjectile()
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
}



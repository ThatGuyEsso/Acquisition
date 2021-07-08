using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingProjectile : Base_Projectile
{
    [SerializeField] private string bounceSFX;
    [SerializeField] private int bounceCount;


    private Animator animator;
    private int remainingBounces;
    bool isBusy;
    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        remainingBounces = bounceCount;
        if (animator) animator.Play("Idle");
        isBusy = false;
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {

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
                    DoExplosion();
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
                    DoExplosion();
                }
            }
        }
    }

    public void DoBounce(ContactPoint2D pointOfContact)
    {

        if (remainingBounces > 0)
        {
            remainingBounces--;

            if (AudioManager.instance)
            {
                AudioManager.instance.PlayThroughAudioPlayer(bounceSFX, transform.position, true);
            }
            Reflect(pointOfContact);
            isHurt = true;
            if (flashVFX)
            {
                flashVFX = GetComponent<SpriteFlash>();
                flashVFX.Init();
            }
            if (flashVFX) flashVFX.Flash();
        }
        else
        {
            DoExplosion();
        }
    }


    public void DoExplosion()
    {

        if (isBusy) return;
        rb.velocity = Vector2.zero;
        isBusy = true;
        if (animator) animator.Play("Explode",0,0f);
    }
    public void OnCollisionEnter2D(Collision2D other)
    {


        if (((1 << other.gameObject.layer) & destroyProjectileLayer) != 0)
        {
            if (AudioManager.instance)
            {
                AudioManager.instance.PlayThroughAudioPlayer(hitSFXname, transform.position, true);
            }

        }
  
    }

    public override void OnDamage(float dmg, Vector2 kBackDir, float kBackMag, GameObject attacker)
    {
        if (!isHurt&&!isBusy)
        {

            isHurt = true;
            if (attacker != owner) blockCount--;
            if (blockCount <= 0) DoExplosion();

            if (AudioManager.instance) AudioManager.instance.PlayThroughAudioPlayer("ProjectileHurt", transform.position, true);
            if (flashVFX)
            {
                flashVFX = GetComponent<SpriteFlash>();
                flashVFX.Init();
            }
            if (flashVFX) flashVFX.Flash();
        }
 
    }
    public void Reflect(ContactPoint2D pointOfContact)
    {

        if (rb)
        {
            ContactPoint2D contactPoint = pointOfContact;

            Vector2 dir = Vector2.Reflect(transform.up, contactPoint.normal);
            projectileSpeed += 2.0f;
            rb.velocity = dir * projectileSpeed;

            OrientateToMovement();
        }
        else
        {
            KillProjectile();
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
    }
}

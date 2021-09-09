using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InLargeOverTime))]
public class AbsorbsiveProjectile : HomingProjectile
{
    private InLargeOverTime sizeController;

    [SerializeField] private float absorbSizeIncrement;
    [SerializeField] private float absorbDamageIncrement;
    [SerializeField] private int maxAbsorbCount;
    private int absorbCount;

    [SerializeField] private float maxSize;

    private Vector3 defaultSize;
    private Vector3 currTargetSize;
 
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
    }
    protected override void OnEnable()
    {
        base.OnEnable();

        sizeController.enabled = true;
     
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & destroyProjectileLayer) != 0)
        {
            if (AudioManager.instance)
            {
                AudioManager.instance.PlayThroughAudioPlayer(hitSFXname, transform.position, true);
            }
            KillProjectile();
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectiles"))
        {
            if (other.GetComponent<IProjectile>().GetOwner() != owner || owner == null)
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
                    KillProjectile();
                }
            }
        }

        if (other.gameObject.CompareTag("Obstacles"))
        {



            if (other.GetComponent<IDamage>() != null)
            {
                other.GetComponent<IDamage>().OnDamage(projectileDamage, rb.velocity, knockback, owner);
                KillProjectile();
            }



        }


    }

    protected override void OnDisable()
    {
        base.OnDisable();
        absorbCount = 0;
    }

    public void OnAbsorbProjectile()
    {
        absorbCount++;
        projectileDamage += absorbDamageIncrement;
        if (absorbCount > maxAbsorbCount) absorbCount = maxAbsorbCount;

        if (absorbSizeIncrement > 0)
        {
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
   


    }
 
}


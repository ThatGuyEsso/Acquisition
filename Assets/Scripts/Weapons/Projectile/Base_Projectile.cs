using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Base_Projectile : MonoBehaviour,IInitialisable, IProjectile,IDamage
{
    [SerializeField] protected float projectileDamage;
    [SerializeField] protected float lifeTime;
    [SerializeField] protected string hitSFXname;
    [SerializeField] protected float knockback =0;
    [SerializeField] protected bool inDebug = false;
    [SerializeField] protected float hurtTime = 0.25f;
    [SerializeField] protected int blockCount = 0;//How much damage projectile can take be getting destroyed
    [SerializeField] protected SpriteFlash flashVFX;
    [SerializeField] protected LayerMask destroyProjectileLayer;
    protected GameObject owner;
    protected Rigidbody2D rb;
    protected bool isHurt;
    protected float currHurtTime;
    protected void Awake()
    {
        if (inDebug)
            Init();

        if (hitSFXname == string.Empty) hitSFXname = "ProjectileHit";
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    virtual public void Init()
    {
        if(!rb)
            rb = GetComponent<Rigidbody2D>();
        if (!flashVFX)
            flashVFX = GetComponent<SpriteFlash>();
        if (flashVFX) flashVFX.Init();


    }
    protected void Update()
    {
        if (isHurt)
        {
            if(currHurtTime <= 0)
            {
                isHurt = false;
                currHurtTime = hurtTime;
                if (flashVFX) flashVFX.EndFlash();
            }
            else
            {
                currHurtTime -= Time.deltaTime;
            }
        }
    }
    public void SetUp(Vector3 direction,float speed)
    {
        rb.velocity = direction * speed;
        OrientateToMovement();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & destroyProjectileLayer) != 0)
        {
            if (AudioManager.instance)
            {
                AudioManager.instance.PlayThroughAudioPlayer(hitSFXname, transform.position,true);
            }
            KillProjectile();
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectiles"))
        {
            if (other.GetComponent<IProjectile>().GetOwner() != owner||owner==null)
            {
                if (other.GetComponent<IDamage>() != null)
                {
                    other.GetComponent<IDamage>().OnDamage(projectileDamage, rb.velocity, knockback, owner);

                }
            }
        }

        if (other.gameObject.CompareTag("Player"))
        {

            if (other.gameObject != owner&&owner!=null)
            {
                if (other.GetComponent<IDamage>()!=null)
                {
                    other.GetComponent<IDamage>().OnDamage(projectileDamage, rb.velocity, knockback, owner);
                    KillProjectile();
                }
            
               
            }
        }else if (other.gameObject.CompareTag("Enemy")){
            if (other.gameObject != owner)
            {
                if (other.GetComponent<IDamage>() != null)
                {
                    other.GetComponent<IDamage>().OnDamage(projectileDamage, rb.velocity, knockback, owner);
                    KillProjectile();
                }
            }
        }
    }

    public void DecrementBlockCount()
    {
        blockCount--;
        if(blockCount < 0)
        {
            KillProjectile();
        }
    }

    protected void KillProjectile()
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

    public void SetUpProjectile(float damage, Vector2 dir, float speed, float lifeTime, int blockCount, GameObject owner)
    {
        if (rb)
        {
            SetUp(dir, speed);
            projectileDamage = damage;
        }
        else
        {
            Init();
            SetUp(dir, speed);
            projectileDamage = damage;
        }
        this.owner = owner;
        this.blockCount = blockCount;
        this.lifeTime = lifeTime;
        StartCoroutine(LifeTimer(lifeTime));
    }

    public GameObject GetOwner()
    {
        return owner;
    }

    public void OnDamage(float dmg, Vector2 kBackDir, float kBackMag, GameObject attacker)
    {
        if (!isHurt)
        {
            isHurt = true;
            if (attacker != owner) blockCount--;
            if (blockCount <= 0) KillProjectile();
            if (flashVFX)
            {
                flashVFX = GetComponent<SpriteFlash>();
                flashVFX.Init();
            }
            if (flashVFX) flashVFX.Flash();
        }
    
    }

    public void OrientateToMovement()
    {

        float targetAngle = EssoUtility.GetAngleFromVector((rb.velocity.normalized));
        /// turn offset -Due to converting between forward vector and up vector
        if (targetAngle < 0) targetAngle += 360f;
        transform.rotation = Quaternion.Euler(0.0f, 0f, targetAngle-90f);
    }

    virtual public void SetRotationSpeed(float rotSpeed)
    {
        //
    }

    public void ShootProjectile(float speed, Vector2 direction, float lifeTime)
    {
        if (rb)
        {
            SetUp(direction, speed);
            StartCoroutine(LifeTimer(lifeTime));
        }
    }


    public IEnumerator LifeTimer(float time)
    {
        yield return new WaitForSeconds(time);
        KillProjectile();
    }

    public void ResetProjectile()
    {
        StopAllCoroutines();
    }

    public ProjectileData GetProjectileData()
    {
        ProjectileData data = new ProjectileData(projectileDamage,
          rb.velocity.normalized, rb.velocity.magnitude, lifeTime, blockCount, owner);

        return data;
    }
}

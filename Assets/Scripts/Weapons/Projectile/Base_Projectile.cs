using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Base_Projectile : MonoBehaviour,IInitialisable, IProjectile,IDamage
{
    [SerializeField] protected float projectileDamage=0;
    [SerializeField] protected float knockback =0;
    [SerializeField] protected bool inDebug = false;
    [SerializeField] protected int blockCount = 0;//How much damage projectile can take be getting destroyed

    [SerializeField] protected LayerMask destroyProjectileLayer;
    protected GameObject owner;
    protected Rigidbody2D rb;

    protected void Awake()
    {
        if (inDebug)
            Init();
    }

    virtual public void Init()
    {
        if(!rb)
            rb = GetComponent<Rigidbody2D>();
        
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
            KillProjectile();
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectiles"))
        {
            if (other.GetComponent<IProjectile>().GetOwner() != owner)
            {
                if (other.GetComponent<IDamage>() != null)
                {
                    other.GetComponent<IDamage>().OnDamage(projectileDamage, rb.velocity, knockback, owner);

                }
            }
        }

        if (other.gameObject.CompareTag("Player"))
        {

            if (other.gameObject != owner)
            {
                if (other.GetComponent<IDamage>()!=null)
                {
                    other.GetComponent<IDamage>().OnDamage(projectileDamage, rb.velocity, knockback, owner);

                }
               
            }
        }else if (other.gameObject.CompareTag("Enemy")){
            if (other.gameObject != owner)
            {
                if (other.GetComponent<IDamage>() != null)
                {
                    other.GetComponent<IDamage>().OnDamage(projectileDamage, rb.velocity, knockback, owner);

                }
            }
        }
    }



    protected void KillProjectile()
    {
        if (ObjectPoolManager.instance)
            ObjectPoolManager.Recycle(gameObject);
        else
            Destroy(gameObject);
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
        Invoke("KillProjectile", lifeTime);
    }

    public GameObject GetOwner()
    {
        return owner;
    }

    public void OnDamage(float dmg, Vector2 kBackDir, float kBackMag, GameObject attacker)
    {
        if (attacker != owner) blockCount--;
        if (blockCount <= 0) KillProjectile();
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
            Invoke("KillProjectile", lifeTime);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Base_Projectile : MonoBehaviour,IInitialisable, IProjectile,IDamage
{
    protected float projectileDamage;
    [SerializeField] protected bool inDebug = false;
    protected int blockCount = 0;//How much damage projectile can take be getting destroyed

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
        if(((1 << other.gameObject.layer) & destroyProjectileLayer) != 0)
        {
            KillProjectile();
        }
    }



    protected void KillProjectile()
    {
        Destroy(gameObject);
    }

    public void SetUpProjectile(float damage, Vector2 dir, float speed, float lifeTime, int blockCount, GameObject owner)
    {
        if (rb)
        {
            SetUp(dir, speed);
            projectileDamage = damage;
        }
        this.owner = owner;
        this.blockCount = blockCount;
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
        transform.rotation = Quaternion.Euler(0.0f, 0f, targetAngle);
    }
}

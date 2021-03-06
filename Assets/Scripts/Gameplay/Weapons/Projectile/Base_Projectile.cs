using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Base_Projectile : MonoBehaviour, IInitialisable, IProjectile, IDamage
{
    [SerializeField] protected float projectileDamage;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float lifeTime;
    [SerializeField] protected float allyRepelForce = 5f;
    [SerializeField] protected string hitSFXname;
    [SerializeField] protected float knockback = 0;
    [SerializeField] protected bool inDebug = false;
    [SerializeField] protected float hurtTime = 0.25f;
    [SerializeField] protected int blockCount = 0;//How much damage projectile can take be getting destroyed
    [SerializeField] protected SpriteFlash flashVFX;
    [SerializeField] protected LayerMask destroyProjectileLayer;
    [SerializeField] protected GameObject breakVFX;
    protected GameObject owner;
    protected Rigidbody2D rb;
    protected bool isHurt;
    protected float currHurtTime;


    public System.Action OnKilled;
    public System.Action OnCollision;
    virtual protected void Awake()
    {
        if (inDebug)
            Init();

        if (hitSFXname == string.Empty) hitSFXname = "ProjectileHit";
    }
    virtual protected void OnEnable()
    {
        if (GameManager.instance)
        {
            GameManager.instance.OnNewEvent += EvaluateNewGameEvent;
        }

    }

    virtual protected void EvaluateNewGameEvent(GameEvents newEvent)
    {
        switch (newEvent)
        {

            case GameEvents.PlayerDefeat:
                if (gameObject)
                    ObjectPoolManager.Recycle(gameObject);
                break;

            case GameEvents.BossDefeated:

                if (gameObject)
                    ObjectPoolManager.Recycle(gameObject);
                break;


            case GameEvents.ExitGame:

                if (gameObject)
                    ObjectPoolManager.Recycle(gameObject);
                break;
        }
    }
    virtual protected void OnDisable()
    {
        OnKilled?.Invoke();
        if (isHurt)
        {
            isHurt = false;
            currHurtTime = hurtTime;
            if(flashVFX)flashVFX.CancelFlash();
        }
        if (GameManager.instance)
        {
            GameManager.instance.OnNewEvent -= EvaluateNewGameEvent;
        }

        DoBreakVFX();
        StopAllCoroutines();


    }

    public void DoBreakVFX()
    {
        if (breakVFX) ObjectPoolManager.Spawn(breakVFX, transform.position);
    }
    virtual public void Init()
    {
        if (!rb)
            rb = GetComponent<Rigidbody2D>();
        if (!flashVFX)
            flashVFX = GetComponent<SpriteFlash>();
        if (flashVFX) flashVFX.Init();


    }
    virtual protected void Update()
    {
        if (isHurt)
        {
            if (currHurtTime <= 0)
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
    public void SetUp(Vector3 direction, float speed)
    {
        rb.velocity = direction * speed;
        projectileSpeed = speed;
        OrientateToMovement();
    }

    virtual public void OnTriggerEnter2D(Collider2D other)
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
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectiles"))
        {
            if (other.GetComponent<IProjectile>().GetOwner() == owner)
            {
                Vector2 dir = other.transform.position - transform.position;
                other.GetComponent<IProjectile>().RepelProjectile(dir, allyRepelForce);
            }

        }

    }
    public void DecrementBlockCount()
    {
        blockCount--;
        if (blockCount < 0)
        {
            KillProjectile();
        }
    }

    virtual protected void KillProjectile()
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

    virtual public void SetUpProjectile(float damage, Vector2 dir, float speed, float lifeTime, int blockCount, GameObject owner)
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



    virtual public void OnDamage(float dmg, Vector2 kBackDir, float kBackMag, GameObject attacker)
    {
        if (!isHurt)
        {

            isHurt = true;
            if (attacker != owner) blockCount--;
            if (blockCount <= 0) KillProjectile();

            if (AudioManager.instance) AudioManager.instance.PlayThroughAudioPlayer("ProjectileHurt", transform.position, true);
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
        transform.rotation = Quaternion.Euler(0.0f, 0f, targetAngle - 90f);
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
        else
        {
            Init();
            SetUp(direction, speed);
            StartCoroutine(LifeTimer(lifeTime));
        }
    }


    virtual protected IEnumerator LifeTimer(float time)
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
          rb.velocity.normalized, projectileSpeed, lifeTime, blockCount, owner);

        return data;
    }

    virtual public void SetHomingTarget(Transform target)
    {
        //
    }
    virtual public void SetProximityHomingTarget(Transform target)
    {
        //
    }
    virtual public void RepelProjectile(Vector2 dir, float force)
    {
        //rb.AddForce(dir * force, ForceMode2D.Impulse);
    }

    public GameObject GetSelf()
    {
        return gameObject;
    }

    public void BreakProjectile()
    {
        KillProjectile();
    }

    public void SetSpeed(float speed)
    {
        projectileSpeed = speed;
    }

    virtual public bool IsHoming()
    {
        return false;
    }

    public void SetOwner(GameObject owner)
    {
        this.owner = owner;
    
    }

    virtual public void AssignBossTarget()
    {
       //
    }
}


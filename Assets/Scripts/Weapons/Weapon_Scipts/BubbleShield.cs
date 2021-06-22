using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleShield : MonoBehaviour
{
    public GameObject owner;
    [SerializeField] private int maxHitPoints;
    [SerializeField] private SpriteFlash flashVFX;
    [SerializeField] private float hurtTime;
    [SerializeField] private float bubbleTime;
    private bool isHurt;
    private float currHurtTime;
    private int currHitPoints;

    public System.Action OnDestroy;
    public void Awake()
    {
        flashVFX = GetComponent<SpriteFlash>();
        flashVFX.Init();
    }

    private void OnEnable()
    {
        currHurtTime = hurtTime;
        currHitPoints = maxHitPoints;
        isHurt = false;
        StartCoroutine(RecycleTime());

    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Projectiles")){
            IProjectile projectile = other.gameObject.GetComponent<IProjectile>();

            if (projectile != null)
            {
                if (projectile.GetOwner() != owner)
                {
                    ProjectileData data = projectile.GetProjectileData();
                    projectile.ResetProjectile();
                    projectile.SetUpProjectile(data.damage, data.dir * -1f, data.speed,data.lifeTime, data.blockCount, owner);
                    if (!isHurt)
                    {
                        isHurt = true;
                        currHitPoints--;
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


    private void Update()
    {
        if (isHurt)
        {
            if(currHurtTime <= 0)
            {
                isHurt = false;
                if (flashVFX) flashVFX.EndFlash();
                currHurtTime = hurtTime;
            }
            else
            {
                currHurtTime -= Time.deltaTime;
            }
        }
    }


    public void KillShield()
    {
        if (gameObject)
        {
            OnDestroy?.Invoke();
            transform.parent = null;
            if(gameObject)
                ObjectPoolManager.Recycle(gameObject);
        }
    }

    public void OnDisable()
    {
        StopAllCoroutines();
    }


    IEnumerator RecycleTime()
    {
        yield return new WaitForSeconds(bubbleTime);
        KillShield();
    }
}

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
    [SerializeField] private bool isInfinite =false;
    [SerializeField] private float reflectionDamage=20f;
    private bool isHurt;
    private float currHurtTime;
    private int currHitPoints;
    
    public System.Action OnDestroy;
    public System.Action<GameObject> OnRelfected;
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
        if(!isInfinite)
            StartCoroutine(RecycleTime());

    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == gameObject) return;
        if (other.gameObject.CompareTag("Projectiles")){

            Debug.Log("Destroyed projectile" + gameObject);
            IProjectile projectile = other.GetComponent<IProjectile>();

            if (projectile != null)
            {
                if (projectile.GetOwner() != owner)
                {
                    OnRelfected?.Invoke(projectile.GetSelf());
                    ProjectileData data = projectile.GetProjectileData();
                    projectile.ResetProjectile();
                    projectile.SetUpProjectile(reflectionDamage, data.dir * -1f, data.speed,data.lifeTime, data.blockCount, owner);
                
                    if (BossRoomManager.instance)
                    {
                        if (BossRoomManager.instance.GetBoss())
                            projectile.SetHomingTarget(BossRoomManager.instance.GetBoss().transform);
                 
                    }
                    else
                    {
                        BaseBossAI boss = FindObjectOfType<BaseBossAI>();
                        if (boss) {
                            projectile.SetHomingTarget(boss.transform);
                       
                        }
                        else
                        {
                            if (other) ObjectPoolManager.Recycle(other.gameObject);
                        }
                    }
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
    public void SetBubbleTime(float time) { bubbleTime = time; } 
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

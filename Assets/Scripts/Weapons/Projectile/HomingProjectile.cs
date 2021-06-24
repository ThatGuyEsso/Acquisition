using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FaceTarget))]
public class HomingProjectile : Base_Projectile
{
    private Transform homingTarget;
    private FaceTarget orientatonManager;
    [SerializeField] private GameObject proximityPrefab;
    [SerializeField] private bool useProximity;
    private bool canHome;
    private HomingProximityDetector detector;
    [SerializeField] private bool autoAssignTarget;
    override protected  void Awake()
    {
        base.Awake();
        orientatonManager = gameObject.GetComponent<FaceTarget>();
    }

     override protected void OnEnable()
    {
        base.OnEnable();
        if (useProximity)
        {
            detector = ObjectPoolManager.Spawn(proximityPrefab, transform.position,Quaternion.identity)
                .GetComponent< HomingProximityDetector>();
            if (detector)
            {
                detector.parent = gameObject;
                if (owner) detector.owner = owner;

             
            }

        }
    }
    override public void SetHomingTarget(Transform target)
    {
        if (useProximity) return;
        if(target.gameObject!= owner)
        {
            homingTarget = target;
            if(orientatonManager)
                orientatonManager.SetTarget(target);
            canHome = true;
        }
 
    }

    public override void SetProximityHomingTarget(Transform target)
    {
        if (!useProximity) return;
        if (target.gameObject != owner)
        {
            homingTarget = target;
            if (orientatonManager)
                orientatonManager.SetTarget(target);
            canHome = true;
        }
    }
    override protected void Update()
    {
        base.Update();

     

        if (homingTarget&& canHome)
        {
            orientatonManager.FaceCurrentTarget(-90f);
        }
    }

    public override void SetUpProjectile(float damage, Vector2 dir, float speed, float lifeTime, int blockCount, GameObject owner)
    {
        base.SetUpProjectile(damage, dir, speed, lifeTime, blockCount, owner);
        canHome = false;
        if (detector && useProximity)
        {

            detector.owner = owner;
        }


        if (autoAssignTarget)
        {
            AutoAssingTarget();
        }
    }

    public void AutoAssingTarget()
    {
        if(owner.GetComponent<IBoss>() == null)
        {
            if (BossRoomManager.instance)
            {
                if (BossRoomManager.instance.GetBoss())
                    SetHomingTarget(BossRoomManager.instance.GetBoss().transform);

            }
            else
            {
                BaseBossAI boss = FindObjectOfType<BaseBossAI>();
                if (boss)
                {
                    SetHomingTarget(boss.transform);

                }
                else
                {
                    if (gameObject) ObjectPoolManager.Recycle(gameObject);
                }
            }
        }
    }
    override protected void OnDisable()
    {
        base.OnDisable();
        if (detector)
            ObjectPoolManager.Recycle(detector);
    }


    protected void FixedUpdate()
    {
        if (projectileSpeed > 0f)
            rb.velocity = projectileSpeed * transform.up * Time.fixedDeltaTime;
    }
}

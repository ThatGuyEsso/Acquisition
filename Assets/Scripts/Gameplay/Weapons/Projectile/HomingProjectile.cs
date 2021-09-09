using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FaceTarget))]
public class HomingProjectile : Base_Projectile
{
    protected Transform homingTarget;
    protected FaceTarget orientatonManager;
    [SerializeField] protected GameObject proximityPrefab;
    [SerializeField] protected bool useProximity;
    protected bool canHome;
    protected HomingProximityDetector detector;
    [SerializeField] protected bool autoAssignTarget;
    override protected  void Awake()
    {
        base.Awake();
        orientatonManager = gameObject.GetComponent<FaceTarget>();
    }

     override protected void OnEnable()
    {
        base.OnEnable();
        //Debug.Break();
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
                    Debug.Log("Couldn-t find boss" + gameObject);
                    if (gameObject) ObjectPoolManager.Recycle(gameObject);
                }
            }
        }
        else
        {
            Transform newTarget = owner.GetComponent<IBoss>().GetTarget();
            if (newTarget)
            {
                SetHomingTarget(newTarget);

            }
        }
    }
    override protected void OnDisable()
    {
        base.OnDisable();
        if (detector)
            ObjectPoolManager.Recycle(detector);

        Debug.Log("Destroyed" + gameObject);
    }


    virtual protected void FixedUpdate()
    {
        if (!canHome&&!useProximity) return;
        if (projectileSpeed > 0f)
            rb.velocity = projectileSpeed * transform.up.normalized * Time.fixedDeltaTime;
    }

    public override bool IsHoming()
    {
        return autoAssignTarget || canHome;
    }

    public override void AssignBossTarget()
    {
        autoAssignTarget = true;
        AutoAssingTarget();
    }
}

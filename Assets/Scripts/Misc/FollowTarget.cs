using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private bool startOnEnable;
    [SerializeField] private bool selfDestroy = true;
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed;
    [SerializeField] private float rotateRate;
    private bool isFollowing;
    [SerializeField] private Rigidbody2D rb;
    protected FaceTarget orientatonManager;
    private bool addedFaceTarget;
    private Base_Projectile projectile;

    [SerializeField] private bool ownerIsPlayer;
    [SerializeField] private bool isAProjectile;
    public void SetUpFollower(Transform followTarget, float speed, float rotationRate,bool isProjectile)
    {
        target = followTarget;
        orientatonManager = gameObject.GetComponent<FaceTarget>();
        rb = GetComponent<Rigidbody2D>();
        followSpeed = speed;
        if (!orientatonManager)
        {  
 
            orientatonManager = gameObject.AddComponent<FaceTarget>();
            addedFaceTarget = true;

        }
        orientatonManager.SetRotationRate(rotationRate);
        orientatonManager.SetTarget(target);
        isFollowing = true;

        if (isProjectile)
        {
            projectile = gameObject.GetComponent<Base_Projectile>();
            if (projectile) projectile.OnCollision += Stop;
        }
    } 
    private void OnEnable()
    {
        if (startOnEnable)
        {

            isFollowing = true;
            if (ownerIsPlayer&& isAProjectile)
            {
                if (BossRoomManager.instance)
                {
                    SetUpFollower(BossRoomManager.instance.GetBoss().transform, followSpeed, rotateRate, isAProjectile);
                }
                else
                {
                    ObjectPoolManager.Recycle(gameObject);
                }
             
            }else if(!ownerIsPlayer && isAProjectile)
            {
                if (BossRoomManager.instance)
                {
                    SetUpFollower(BossRoomManager.instance.GetBoss().GetTarget(), followSpeed, rotateRate, isAProjectile);
                }
                else
                {
                    ObjectPoolManager.Recycle(gameObject);
                }
            }
        }
    }


    protected void FixedUpdate()
    {
        if (!isFollowing||!rb) return;
        if (followSpeed > 0f)
            rb.velocity = followSpeed * transform.up.normalized * Time.fixedDeltaTime;
    }


    private void Update()
    {
        if (target && isFollowing&&orientatonManager)
        {
            orientatonManager.FaceCurrentTarget(-90f);
        }
    }
    private void OnDisable()
    {
        if (addedFaceTarget && orientatonManager) Destroy(orientatonManager);
        if (selfDestroy)
            Destroy(this);
    }

    public void Stop()
    {
        isFollowing = false;
        if (projectile) projectile.OnCollision -= Stop;
        if (rb)
            rb.velocity = Vector2.zero;
    }
}

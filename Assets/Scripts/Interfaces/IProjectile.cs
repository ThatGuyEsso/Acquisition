using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ProjectileData
{
    public float damage;
    public Vector2 dir;
    public float speed;
    public float lifeTime;
    public int blockCount;
    public GameObject owner;

    public ProjectileData(float dmg, Vector2 direction, float speed, float lifeTime, int blockCount, GameObject owner)
    {
        damage = dmg;
        dir = direction;
        this.speed = speed;
        this.lifeTime = lifeTime;
        this.blockCount = blockCount;
        this.owner = owner;
    }
}
public interface IProjectile 
{
    void SetUpProjectile(float damage, Vector2 dir, float speed, float lifeTime,int blockCount,GameObject owner);

    void ShootProjectile(float speed, Vector2 direction,float lifeTime);
    void SetRotationSpeed(float rotSpeed);

    void ResetProjectile();
    ProjectileData GetProjectileData();
    GameObject GetOwner();
}

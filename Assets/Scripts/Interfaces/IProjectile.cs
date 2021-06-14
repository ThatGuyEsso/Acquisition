using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile 
{
    void SetUpProjectile(float damage, Vector2 dir, float speed, float lifeTime,int blockCount,GameObject owner);

    void ShootProjectile(float speed, Vector2 direction,float lifeTime);
    void SetRotationSpeed(float rotSpeed);
    GameObject GetOwner();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile 
{
    void SetUpProjectile(float damage, Vector2 dir, float speed, float lifeTime);
}

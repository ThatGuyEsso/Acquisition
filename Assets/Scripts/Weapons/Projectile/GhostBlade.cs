using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBlade : Base_Projectile   
{
    public System.Action<GhostBlade> OnBladeDestroyed;
    protected override void KillProjectile()
    {
        OnBladeDestroyed?.Invoke(this);
        gameObject.SetActive(false);
    }


    public override void SetUpProjectile(float damage, Vector2 dir, float speed, float lifeTime, int blockCount, GameObject owner)
    {
       
        projectileDamage = damage;
        
        this.owner = owner;
        this.blockCount = blockCount;
 
    }

    public override void OnDamage(float dmg, Vector2 kBackDir, float kBackMag, GameObject attacker)
    {
        
    }
}

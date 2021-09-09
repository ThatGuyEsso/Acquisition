using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScholarBuff : Base_BossBuff
{
    [SerializeField] private GameObject  absorbProjectilePrefab;
    bool isBound =false;
    protected override void BindToAbility(BaseBossAbility ability)
    {
        if (!ability||isBound) return;
        base.BindToAbility(ability);
        if (abilityToBuff)
        {
            abilityToBuff.OnProjectileSpawned += BuffProjetile;
            isBound = true;
        }
       
    }

    protected override void UnbindFromAbility(BaseBossAbility ability)
    {
        if (!ability) return;
        if (ability.name == string.Format("{0}(Clone)", abilityToBuffPrefab.name))
        {
            if (abilityToBuff)
            {
                abilityToBuff.OnProjectileSpawned -= BuffProjetile;
                abilityToBuff = null;
                isBound = false;
            }
        

        }

    }


    public void BuffProjetile(GameObject projObject)
    {
        
        if (projObject)
        {
            IProjectile projectile = projObject.GetComponent<IProjectile>();
            ProjectileData data = projectile.GetProjectileData();
            Transform projTrans = projObject.transform;

            ObjectPoolManager.Recycle(projObject);

            Base_Projectile proj = ObjectPoolManager.Spawn(absorbProjectilePrefab,
            projTrans.transform.position, projTrans.transform.rotation).GetComponent<Base_Projectile>();

            proj.SetUpProjectile(data.damage, data.dir, data.speed, data.lifeTime, data.blockCount, data.owner);
        
        }

       
    }
}

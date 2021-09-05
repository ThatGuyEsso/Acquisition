using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScholarBuff : Base_BossBuff
{
    [SerializeField] private GameObject  absorbProjectilePrefab;
    protected override void BindToAbility(BaseBossAbility ability)
    {
        if (!ability) return;
        base.BindToAbility(ability);
        if (abilityToBuff)
            abilityToBuff.OnProjectileSpawned += BuffProjetile;
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
            }
        

        }

    }


    public void BuffProjetile(GameObject projObject)
    {
        Base_Projectile proj = ObjectPoolManager.Spawn(absorbProjectilePrefab, 
            projObject.transform.position, projObject.transform.rotation).GetComponent<Base_Projectile>();
        if (proj)
        {
            IProjectile projectile = projObject.GetComponent<IProjectile>();
            ProjectileData data = projectile.GetProjectileData();


            proj.SetUpProjectile(data.damage, data.dir, data.speed, data.lifeTime, data.blockCount, data.owner);
            ObjectPoolManager.Recycle(proj);
        }

       
    }
}

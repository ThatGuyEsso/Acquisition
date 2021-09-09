using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElderBuff : Base_BossBuff
{
    [SerializeField] private float homingSpeed;
    [SerializeField] private float rotateToTargetRate =0.25f;
    protected override void BindToAbility(BaseBossAbility ability)
    {
        base.BindToAbility(ability);
        if(abilityToBuff)
            abilityToBuff.OnProjectileSpawned += BuffProjetile;
    }

    protected override void UnbindFromAbility(BaseBossAbility ability)
    {
        if (ability.name == string.Format("{0}(Clone)", abilityToBuffPrefab.name))
        {
            abilityToBuff.OnProjectileSpawned -= BuffProjetile;
            abilityToBuff = null;

        }
  
    }


    public void BuffProjetile(GameObject projObject)
    {
        IProjectile projectile = projObject.GetComponent<IProjectile>();

        if (projectile != null)
        {
            projObject.AddComponent<FollowTarget>().SetUpFollower(owner.GetTarget(), homingSpeed, rotateToTargetRate,true);
        }
    }
}

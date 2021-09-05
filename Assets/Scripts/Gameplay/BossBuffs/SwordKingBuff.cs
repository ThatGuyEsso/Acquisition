using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordKingBuff : Base_BossBuff
{
    [SerializeField] private float sizeMultiplier;
    [SerializeField] private int hitPointBonus;
    [SerializeField] private float sizeIncreaseRate;

    [SerializeField] private float growDelay;

    protected override void BindToAbility(BaseBossAbility ability)
    {
        base.BindToAbility(ability);

        if (abilityToBuff)
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
        Vector3 initialSize = projObject.transform.localScale;
        Vector3 targetSize = initialSize * sizeMultiplier;

        IProjectile projectile = projObject.GetComponent<IProjectile>();

        if (projectile != null)
        {
            ProjectileData data = projectile.GetProjectileData();

            projectile.SetUpProjectile(data.damage, data.dir, data.speed, data.lifeTime, data.blockCount + hitPointBonus, data.owner);
            InLargeOverTime multplier = projObject.GetComponent<InLargeOverTime>();
            if (multplier) multplier.SetUpGrowSetting(initialSize, targetSize.x, sizeIncreaseRate, growDelay);
            else
                projObject.AddComponent<InLargeOverTime>().SetUpGrowSetting(initialSize, targetSize.x, sizeIncreaseRate, growDelay);
        }
    }
}

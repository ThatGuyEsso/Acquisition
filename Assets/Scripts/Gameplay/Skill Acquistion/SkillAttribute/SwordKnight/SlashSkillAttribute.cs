using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashSkillAttribute : Base_SkillAttribute
{
    [SerializeField] private float sizeMultiplierSword;
    [SerializeField] private float sizeMultiplierArrow;
    [SerializeField] private float sizeMultiplierReflection;
    [SerializeField] private int hitPointBonus;
    [SerializeField] private float sizeIncreaseRate;

    [SerializeField] private float growDelay;

    private BubbleShield currentShield;

    public override void EvaluatePrimaryAbilityAttribute(GameObject spawnedAbiliity)
    {
        switch (owner.GetWeaponType())
        {
            case WeaponType.Sword:
                BuffProjetile(spawnedAbiliity, sizeMultiplierSword);
                break;
            case WeaponType.Bow:
                BuffProjetile(spawnedAbiliity, sizeMultiplierArrow);
                break;
            case WeaponType.Staff:
                break;
        };
    }


    public override void EvaluateSecondaryAbilityAttribute(GameObject spawnedAbiliity)
    {
        base.EvaluateSecondaryAbilityAttribute(spawnedAbiliity);

 
        switch (owner.GetWeaponType())
        {
    
            case WeaponType.Staff:
                if (currentShield) ClearShield();
                AddShield(spawnedAbiliity);
                break;
        };
    }
    public void BuffProjetile(GameObject projObject,float multiplier)
    {
        Vector3 initialSize = projObject.transform.localScale;
        Vector3 targetSize = initialSize * multiplier;

        IProjectile projectile = projObject.GetComponent<IProjectile>();

        if (projectile!=null)
        {
            ProjectileData data = projectile.GetProjectileData();

            projectile.SetUpProjectile(data.damage, data.dir, data.speed, data.lifeTime, data.blockCount + hitPointBonus, data.owner);
            projObject.AddComponent<InLargeOverTime>().SetUpGrowSetting(initialSize, targetSize.x, sizeIncreaseRate, growDelay);
        }
    }

    public void AddShield(GameObject shieldObj)
    {
        currentShield = shieldObj.GetComponent<BubbleShield>();
        if (currentShield) {
            currentShield.OnDestroy += ClearShield;
            currentShield.OnRelfected += ApplySkillToReflected;
        }
    }

    public void ClearShield()
    {
        currentShield.OnRelfected -= ApplySkillToReflected;
        currentShield.OnDestroy -= ClearShield;
        currentShield = null;
    }

    public void ApplySkillToReflected(GameObject reflected)
    {
        Vector3 initialSize = reflected.transform.localScale;
        Vector3 targetSize = initialSize * sizeMultiplierReflection;

        IProjectile projectile = reflected.GetComponent<IProjectile>();

        if (projectile != null)
        {
            ProjectileData data = projectile.GetProjectileData();

            projectile.SetUpProjectile(data.damage, data.dir, data.speed, data.lifeTime, data.blockCount + hitPointBonus, data.owner);
            reflected.AddComponent<InLargeOverTime>().SetUpGrowSetting(initialSize, targetSize.x, sizeIncreaseRate, growDelay);
        }
    }




}

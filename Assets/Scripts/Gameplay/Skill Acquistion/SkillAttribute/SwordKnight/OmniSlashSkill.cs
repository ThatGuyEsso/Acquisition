using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmniSlashSkill : Base_SkillAttribute
{
    [SerializeField] private int swordProjectileCount;
    [SerializeField] float swordMaxAngle;
    [SerializeField] private GameObject thrustPrefab;

    [SerializeField] private GameObject ghostBladePrefab;
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] private int bowProjectileCount;
    [SerializeField] float bowMaxAngle;

    [SerializeField] public float swordOffset;

    [SerializeField] public float bowOffest;

    private SpinningBlades currentBlades;
    private BubbleShield currentShield;
    public override void EvaluatePrimaryAbilityAttribute(GameObject spawnedAbiliity)
    {
        switch (owner.GetWeaponType())
        {
       
            case WeaponType.Bow:
                FireProjectilesInArc(owner.GetWeaponType(), spawnedAbiliity, owner.GetPrimaryProjectilePrefab(), swordMaxAngle, bowProjectileCount, bowOffest);
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

            case WeaponType.Sword:
                FireProjectilesInArc(owner.GetWeaponType(), spawnedAbiliity, owner.GetSecondaryProjectile(), swordMaxAngle, swordProjectileCount, swordOffset);
                break;
        };
    }



    public void FireProjectilesInArc(WeaponType type, GameObject originalProjectile,GameObject projectilePrefab, float maxAngle, int projectileCount,float offset)
    {
        if (!originalProjectile) return;
        float startingAngle = (EssoUtility.GetAngleFromVector(owner.GetFirePoint().up) + maxAngle / 2.0f);

        float currentAngle = startingAngle + offset;
        float angleIncrease = maxAngle / projectileCount;


        IProjectile proj = originalProjectile.GetComponent<IProjectile>();

        if(proj != null)
        {
            ProjectileData data = proj.GetProjectileData();
            if (originalProjectile)
                ObjectPoolManager.Recycle(originalProjectile);

            GameObject currentObject;
            for (int i = 0; i < projectileCount; i++)
            {

                Vector2 dir = EssoUtility.GetVectorFromAngle(currentAngle);
                currentObject = ObjectPoolManager.Spawn(projectilePrefab, owner.GetFirePoint().position, Quaternion.identity);

                if (currentObject)
                {
                    IProjectile currProj = currentObject.GetComponent<IProjectile>();
                    if(currProj != null)
                    {

                        if (currProj.IsHoming())
                        {
                            HomingSkillAttribute homeSkill = transform.parent.GetComponentInChildren<HomingSkillAttribute>();

                            if (homeSkill)
                            {
                                if(type == WeaponType.Sword)
                                    currProj.SetUpProjectile(data.damage, dir.normalized, homeSkill.GetSwordSpeed(), data.lifeTime, data.blockCount, data.owner);
                                else if(type == WeaponType.Bow)
                                    currProj.SetUpProjectile(data.damage, dir.normalized, homeSkill.GetBowSpeed(), data.lifeTime, data.blockCount, data.owner);
                                else
                                    currProj.SetUpProjectile(data.damage, dir.normalized, data.speed, data.lifeTime, data.blockCount, data.owner);
                            }
                            else
                            {
                                currProj.SetUpProjectile(data.damage, dir.normalized, data.speed, data.lifeTime, data.blockCount, data.owner);
                            }
                        }
                        else
                        {

                            currProj.SetUpProjectile(data.damage, dir.normalized, data.speed, data.lifeTime, data.blockCount, data.owner);
                        }
                    }
                    else
                    {
                        ObjectPoolManager.Recycle(currentObject);
                    }
                }

                currentAngle -= angleIncrease;



            }

        }
    }


    public void AddShield(GameObject shieldObj)
    {
        currentShield = shieldObj.GetComponent<BubbleShield>();
        if (currentShield)
        {
            currentShield.OnDestroy += ClearShield;
            currentBlades = ObjectPoolManager.Spawn(ghostBladePrefab, owner.GetPlayerTransform().position,Quaternion.identity).GetComponent <SpinningBlades>();
            if (currentBlades)
            {
                currentBlades.SetUp(owner.GetPlayerTransform().gameObject);
            }
        }
    }

    public void ClearShield()
    {

        if (currentBlades)
        {
            ObjectPoolManager.Recycle(currentBlades.gameObject);
        }
        if (currentShield)
        {
            currentShield.OnDestroy -= ClearShield;
            currentShield = null;
        }
      
    }


    protected override void OnDisable()
    {
        base.OnDisable();
        ClearShield();
    }
}

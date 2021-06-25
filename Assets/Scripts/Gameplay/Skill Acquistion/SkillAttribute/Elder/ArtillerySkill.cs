using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtillerySkill : Base_SkillAttribute
{
    [SerializeField] private GameObject hazardZonePrefab;
    [SerializeField] private float hazardTime;
    private GameObject playerObject;
    private DodgeRoll dodgeroll;

    public override void SetUpAttribute(Base_Weapon weaponOwner)
    {
        base.SetUpAttribute(weaponOwner);
        playerObject = weaponOwner.GetPlayerTransform().gameObject;

        if (playerObject)
        {
            dodgeroll = playerObject.GetComponent<DodgeRoll>();

            if (dodgeroll)
            {

                dodgeroll.OnRollBegun += AddHazardZone;
            
            }
        }
    }


    public void AddHazardZone()
    {
        AttackVolume volume = ObjectPoolManager.Spawn(hazardZonePrefab, owner.GetPlayerTransform().position, Quaternion.identity).GetComponent<AttackVolume>();
        if (volume) volume.SetDespawnTime(hazardTime);
    }
    protected override void OnEnable()
    {
        base.OnEnable();

        if (dodgeroll)
        {
            dodgeroll.OnRollBegun += AddHazardZone;
          
        }
    }

    protected override void OnDisable()
    {
        base.OnEnable();

        if (dodgeroll)
        {
            dodgeroll.OnRollBegun -= AddHazardZone;

        }
    }

}

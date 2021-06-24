using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HonourableChargeSkillAttribute : Base_SkillAttribute
{
    private GameObject playerObject;

    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private float damage;
    private UnlimitedAttackVolume shield;
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
                
                dodgeroll.OnRollBegun += AttachRollShield;
                dodgeroll.OnRollEnd += RemoveRollShield;
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (dodgeroll)
        {
            dodgeroll.OnRollBegun += AttachRollShield;
            dodgeroll.OnRollEnd += RemoveRollShield;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (dodgeroll)
        {
            dodgeroll.OnRollBegun -= AttachRollShield;
            dodgeroll.OnRollEnd -= RemoveRollShield;
        }
    }
    public void AttachRollShield()
    {
        if (shield) RemoveRollShield();

        shield = ObjectPoolManager.Spawn(shieldPrefab, playerObject.transform).GetComponent<UnlimitedAttackVolume>();
        shield.SetUpDamageVolume(damage, 0f, playerObject.transform.up, playerObject);
    }


    public void RemoveRollShield()
    {
        if (shield)
        {
            ObjectPoolManager.Recycle(shield.gameObject);
            shield = null;
        }
    }
}

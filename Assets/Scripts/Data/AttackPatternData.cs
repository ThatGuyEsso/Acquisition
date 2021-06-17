using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 [System.Serializable]
public class AttackPatternData 
{
    [Header("AttackSettings")]
    [Tooltip("Number of time ability can be used before cooldown")]
    [SerializeField] protected int maxAttackCount;
    [Tooltip("Time between ability uses")]
    [SerializeField] protected float attackRate;
    [Tooltip("Cooldown until ability Refreshes")]
    [SerializeField] protected float coolDown;
    [Tooltip("Range where character will switch to attack using ability")]
    [SerializeField] protected float attackRange;
    [Tooltip("Whether or not ability will control when boss can attack again")]
    [SerializeField] protected bool isManagingBossAttack;
    [Tooltip("Will try to commit attack even if player isn't in range")]
    [SerializeField] protected bool isPriority;
    [Tooltip("If boss executes this attacks if player is in close range")]
    [SerializeField] protected bool isSuperCloseRange ;


    [SerializeField] protected BaseBossAbility abilityPrefab;


    public BaseBossAbility CreateNewAbility(Transform parent)
    {
        BaseBossAbility ability = ObjectPoolManager.Spawn(abilityPrefab.gameObject, parent).GetComponent<BaseBossAbility>();

        ability.SetUpAbility(maxAttackCount, attackRate, attackRange, coolDown, isManagingBossAttack, isPriority, isSuperCloseRange);

        return ability;
    }
}

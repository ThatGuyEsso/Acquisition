using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBossAbility : MonoBehaviour, IInitialisable
{
    [Header("Settings")]
    [Tooltip("Number of time ability can be used before cooldown")]
    [SerializeField] protected int maxAttackCount;
    [Tooltip("Time between ability uses")]
    [SerializeField] protected float attackRate;
    [Tooltip("Cooldown until ability Refreshes")]
    [SerializeField] protected float coolDown;
    [SerializeField] protected string attackAnimationName;
    [SerializeField] protected GameObject attackAreaPrefab;
    [SerializeField] protected GameObject projectilePrefab;

    [SerializeField] protected List<base> transitionAbility;
    [SerializeField] protected List<GameObject> closeCombatAbility;
    protected int attacksLeft;

    virtual public void Init()
    {
        attacksLeft = maxAttackCount;
    }
}

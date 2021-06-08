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
    [SerializeField] protected float attackRange;

    [SerializeField] protected bool isPriority=false;
    [SerializeField] protected bool isSuperCloseRange = false;
    [SerializeField] protected string attackAnimationName;


    protected bool canAttack;
    protected GameObject attackZone;
    public BaseBossAI owner;
    protected int attacksLeft;
    public AttackAnimEventListener eventListener;


    
    virtual public void Init()
    {
        attacksLeft = maxAttackCount;
        canAttack = true;


    }

    public bool IsCloseRange() { return isSuperCloseRange; }
    public bool IsPriority() { return isPriority; }
    public bool CanAttack() { return canAttack; }
    public float Range() { return attackRange; }
    public string AnimationName() { return attackAnimationName; }

}

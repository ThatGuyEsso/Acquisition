using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseBossAbility : MonoBehaviour, IInitialisable
{
    [Header("Settings")]
    [Tooltip("Number of time ability can be used before cooldown")]
    [SerializeField] protected int maxAttackCount;
    [Tooltip("Time between ability uses")]
    [SerializeField] protected float attackRate;
    [Tooltip("Cooldown until ability Refreshes")]
    [SerializeField] protected float coolDown;
    [SerializeField] protected float attackRange;
    [SerializeField] protected bool isManagingBossAttack;


    [SerializeField] protected bool isPriority;
    [SerializeField] protected bool isSuperCloseRange;
    [SerializeField] protected string attackAnimationName;

    public Rigidbody2D bossRB;
    public bool isEnabled;
    protected DynamicConeCollider dynamicAttackZone;
    public BaseBossAI owner;
    protected int attacksLeft;
    public AttackAnimEventListener eventListener;
    protected bool canAttack;


    public void SetUpAbility(int attackCount,float attackRate,float attackRange, float longCooldown,bool isManagingControls,
        bool isPriority, bool isCloseRange)
    {
        maxAttackCount = attackCount;
        this.attackRate = attackRate;
        this.attackRange = attackRange;
        coolDown = longCooldown;
        isManagingBossAttack = isManagingControls;
        this.isPriority = isPriority;
        isSuperCloseRange = isCloseRange;

    }
    
    virtual public void Init()
    {
        ResetAbility();

    }

    protected IEnumerator BeginResetAbility(float time)
    {
        yield return new WaitForSeconds(time);
        ResetAbility();
    }

    protected IEnumerator BeginRefreshAttack(float time)
    {
        yield return new WaitForSeconds(time);
        ResetAttack();
    }
    public void ResetAbility()
    {
        attacksLeft = maxAttackCount;
        canAttack = true;

    }
    public void ResetAttack()
    {

        canAttack = true;

    }




    public bool IsCloseRange() { return isSuperCloseRange; }
    public bool IsPriority() { return isPriority; }
    public bool CanAttack() { return canAttack; }
    public float Range() { return attackRange; }
    public bool IsManagingAttack() { return isManagingBossAttack; }
    public string AnimationName() { return attackAnimationName; }
    virtual public void DisableAbility()
    {
        isEnabled = false;
    }
    virtual public void EnableAbility()
    {
        isEnabled = true;
       
    }



}

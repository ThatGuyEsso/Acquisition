using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BossStage
{
    Initial,
    Middle,
    End,
    Transition
};

public enum AIState
{
    Idle,
    Chase,
    Attack
};
public abstract class BaseBossAI : MonoBehaviour,IInitialisable,IBoss,IDamage
{

    [Header("Settings")]
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float aiTickRate= 0.25f;
    [SerializeField] protected Transform target;
    [SerializeField] protected BossStageData stageData;
    [Header("Componeents")]
    [SerializeField] protected TDNavMeshMovement navigation;
    [SerializeField] protected FaceTarget faceTarget;
    [SerializeField] protected FaceMovementDirection faceMovementDirection;
    [SerializeField] protected Animator animator;

    [SerializeField] protected float maxHurtTime;
    [SerializeField] protected AttackAnimEventListener attackAnimEvents;
    [SerializeField] protected List<Component> componentsToInit = new List<Component>();
    [SerializeField] protected List<BaseBossAbility> currentStageAbilities = new List<BaseBossAbility>();

    protected bool isInitialised;
    protected float currentHealth;
    protected int currentAttackIndex;
    protected BossStage currentStage;
    protected AIState currentAIState;
    protected bool canAttack;
    protected bool canLockOn=false;
    protected float currHurtTime;
    protected bool isHurt;
    [SerializeField] protected BaseBossAbility transitionAbility;
    [SerializeField] protected BaseBossAbility closeCombatAbility;

    [SerializeField] protected bool isBusy = false;
    [SerializeField] protected bool inDebug=false;
    protected void Awake()
    {
        if (inDebug) Init();
    }
    virtual public void Init()
    {
        currentHealth = maxHealth;
        currentAttackIndex = 0;
        currentStage = BossStage.Initial;
        currentAIState = AIState.Chase;
        if (componentsToInit.Count > 0)
        {
            for (int i = 0; i < componentsToInit.Count; i++)
            {
                IInitialisable initialisable = componentsToInit[i].GetComponent<IInitialisable>();
                initialisable.Init();
            }
        }
        if (target)
            navigation.StartAgent(target);
        if (!attackAnimEvents) attackAnimEvents = GetComponent<AttackAnimEventListener>();
        else
        {
            attackAnimEvents.OnAnimStart += OnAttackBegin;
            attackAnimEvents.OnAnimEnd += OnAttackEnd;
        }
        SetUpNextStage();
        isInitialised = true;
        ToggleCanAttack(true);
        InvokeRepeating("ProcessAI", 0.0f, aiTickRate);
        isBusy = false;
    }

    virtual public bool InRange()
    {
        if (currentStageAbilities.Count <= 0) return false;
        if (!target) return false;
        return Vector2.Distance(target.position, transform.position) <= currentStageAbilities[currentAttackIndex].Range();

    }
    virtual public bool InCloseRange()
    {
        if (!closeCombatAbility) return false;
        if (!target) return false;
        return Vector2.Distance(target.position, transform.position) <= closeCombatAbility.Range();

    }
    virtual protected void ProcessAI()
    {
     //
    }

    virtual protected void OnNewState(AIState newState)
    {
        currentAIState = newState;
        switch (currentAIState)
        {
            case AIState.Idle:
                break;
            case AIState.Chase:
                break;
            case AIState.Attack:
                break;
        }
    }


    virtual public void CycleToNextAttack()
    {
        currentStageAbilities[currentAttackIndex].DisableAbility();
        currentAttackIndex++;
        if (currentAttackIndex >= currentStageAbilities.Count) currentAttackIndex = 0;
        currentStageAbilities[currentAttackIndex].EnableAbility();
    }
 
    virtual protected void SetUpNextStage()
    {
        if (currentStageAbilities.Count<=0)
        {
            foreach (BaseBossAbility ability in currentStageAbilities)
            {
                ObjectPoolManager.Recycle(ability.gameObject);
            }
            currentStageAbilities.Clear();
        }

   

        switch (currentStage)
        {
            case BossStage.Initial:
                SetUpStageAbilities(stageData.initialStageAbilities);
                break;
            case BossStage.Middle:
                SetUpStageAbilities(stageData.middleStageAbilities);
                break;
            case BossStage.End:
                SetUpStageAbilities(stageData.finalStageAbiilities);
                break;
        }
    }

    protected void SetUpStageAbilities(List<GameObject> abilities)
    {
        for(int i = 0; i < abilities.Count; i++)
        {

      
            if (abilities[i].GetComponent<BaseBossAbility>())
            {
                BaseBossAbility ability= ObjectPoolManager.Spawn(abilities[i].gameObject, transform).GetComponent<BaseBossAbility>();
                if (ability.IsCloseRange()) closeCombatAbility = ability;
                ability.eventListener = attackAnimEvents;
                ability.owner = this;
                ability.GetComponent<IInitialisable>().Init();
                currentStageAbilities.Add(ability);
            }
        }

        currentStageAbilities[currentAttackIndex].EnableAbility();
    }


    public void OnAttackBegin() { isBusy = true; }
    public void OnAttackEnd() {
        isBusy = false; 
        
    }


  
    virtual protected void OnDisable()
    {
        if (isInitialised)
        {
            attackAnimEvents.OnAnimStart -= OnAttackBegin;
            attackAnimEvents.OnAnimEnd -= OnAttackEnd;
        }

    }
    public void DoAttack()
    {
        if (!currentStageAbilities[currentAttackIndex].enabled) currentStageAbilities[currentAttackIndex].EnableAbility();
        if (currentStageAbilities[currentAttackIndex].IsManagingAttack()) canLockOn = true;
        else canLockOn = false;
        animator.Play(currentStageAbilities[currentAttackIndex].AnimationName());
    }
    public void DoCloseQuarters()
    {
        closeCombatAbility.EnableAbility();
        if (closeCombatAbility.IsManagingAttack()) canLockOn = true;
        else canLockOn = false;
        animator.Play(closeCombatAbility.AnimationName());
    }


    virtual public Transform GetFirePoint()
    {
        return transform;
    }

    public void ToggleCanAttack(bool can)
    {
        canAttack = true;
    }

    virtual public void SetUseRigidBody(bool use)
    {
        //
    }

    public void SetCanLockOn(bool canLock) { canLockOn = canLock; }

    virtual public Rigidbody2D GetRigidBody()
    {
        return null;
    }

    public void SetIsBusy(bool busy) { isBusy = busy; }
    public void PlayAnimation(string animName)
    {
        animator.Play(animName);
    }

    public void OnDamage(float dmg, Vector2 kBackDir, float kBackMag, GameObject attacker)
    {
       
    }
}

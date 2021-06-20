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
    [SerializeField] protected SpriteFlash flashVFX;
    [SerializeField] protected string BossName;
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float aiTickRate= 0.25f;
    public Transform target;
    [SerializeField] protected BossStageData stageData;
    [SerializeField] protected GameObject bossUIPrefab;
    [SerializeField] protected string awakenAnimName;
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
    protected int currentStageIndex;
    protected BossStage currentStage;
    protected AIState currentAIState;
    protected bool canAttack;
    protected bool canLockOn=false;
    protected float currHurtTime;
    protected bool isHurt;
    public BossUI UI;
    [SerializeField] protected GameObject transitionAbilityPrefab;
    [SerializeField] protected BaseBossAbility closeCombatAbility;

    protected BaseBossAbility transitionAbility;
    [SerializeField] protected bool isBusy = false;
    [SerializeField] protected bool inDebug=false;
    protected bool isFighting;
    protected bool isTransitioning;

    public System.Action OnAwakened; 
    protected void Awake()
    {
        if (inDebug) Init();
    }
    virtual public void Init()
    {
        currentHealth = maxHealth;
        currentAttackIndex = 0;
        currentStage = BossStage.Transition;
        currentStageIndex = 0;
        currentAIState = AIState.Chase;
        currHurtTime = maxHurtTime;
        if (componentsToInit.Count > 0)
        {
            for (int i = 0; i < componentsToInit.Count; i++)
            {
                IInitialisable initialisable = componentsToInit[i].GetComponent<IInitialisable>();
                initialisable.Init();
            }
        }

        if (!attackAnimEvents) attackAnimEvents = GetComponent<AttackAnimEventListener>();
        else
        {
            attackAnimEvents.OnAnimStart += OnAttackBegin;
            attackAnimEvents.OnAnimEnd += OnAttackEnd;
        }

        isInitialised = true;
        UI = ObjectPoolManager.Spawn(bossUIPrefab, Vector3.zero, Quaternion.identity).GetComponent<BossUI>();
        if (UI)
        {
            UI.InitialiseUI(BossName);
            UI.progressBar.SetMaxValue(maxHealth);
         
            UI.OnUISpawned += AwakenBoss;
        }

        if (!flashVFX)
        {
            flashVFX = GetComponent<SpriteFlash>();
            flashVFX.Init();
        }
        else
        {
            flashVFX.Init();
        }
    }
    public void AwakenBoss()
    {
        UI.OnUISpawned -= AwakenBoss;
        if (inDebug)
            attackAnimEvents.OnAnimEnd += BeginFight;
        else
        {
            attackAnimEvents.OnAnimEnd += CallAwake;
        }
        animator.Play(awakenAnimName, 0, 0f);
    }

    public void CallAwake()
    {
        attackAnimEvents.OnAnimEnd -= CallAwake;
        OnAwakened?.Invoke();
    }
    virtual public void BeginFight()
    {
            if (isInitialised)
            {
                attackAnimEvents.OnAnimEnd -= BeginFight;
                currentStage = BossStage.Initial;   
                SetUpNextStage();
                ToggleCanAttack(true);
                InvokeRepeating("ProcessAI", 0.0f, aiTickRate);
                isBusy = false;
                isFighting = true;
                if (target)
                {
                    navigation.enabled = true;
                    navigation.Init();  
                    navigation.StartAgent(target);
                }

                if (GameManager.instance)
                    GameManager.instance.BeginNewEvent(GameEvents.BossFightStarts);
            }
        
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

    virtual public void OnNewState(AIState newState)
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
        if(currentStage!= BossStage.Transition)
        {
            currentStageAbilities[currentAttackIndex].DisableAbility();
            currentAttackIndex++;
            if (currentAttackIndex >= currentStageAbilities.Count) currentAttackIndex = 0;
            currentStageAbilities[currentAttackIndex].EnableAbility();
        }
        else
        {
            currentAttackIndex = 0;
            SetUpNextStage();
        }
 
    }
 
    virtual protected void SetUpNextStage()
    {

        currentStage = (BossStage)currentStageIndex;
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
    protected void InitialiseAbility(BaseBossAbility ability)
    {
        ability.eventListener = attackAnimEvents;
        ability.owner = this;
        IInitialisable initialise = ability.GetComponent<IInitialisable>();
        if (initialise != null) initialise.Init();
        ability.EnableAbility();
    }
    protected void SetUpStageAbilities(List<AttackPatternData> abilities)
    {
        for(int i = 0; i < abilities.Count; i++)
        {


            if (abilities[i]!=null)
            {

                BaseBossAbility ability = abilities[i].CreateNewAbility(transform);
                if (ability.IsCloseRange()) closeCombatAbility = ability;
                ability.eventListener = attackAnimEvents;
                ability.owner = this;
                IInitialisable initialise= ability.GetComponent<IInitialisable>();
                if (initialise != null) initialise.Init();
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
        if(currentStage != BossStage.Transition)
        {
            if (!currentStageAbilities[currentAttackIndex].isEnabled) currentStageAbilities[currentAttackIndex].EnableAbility();
            if (currentStageAbilities[currentAttackIndex].IsManagingAttack()) canLockOn = true;
            else canLockOn = false;
            animator.Play(currentStageAbilities[currentAttackIndex].AnimationName());
        }
        else
        {
            if (transitionAbility)
            {
                if (!transitionAbility.isEnabled) transitionAbility.EnableAbility();
                if (transitionAbility.IsManagingAttack()) canLockOn = true;
                else canLockOn = false;
                animator.Play(transitionAbility.AnimationName());
            }
        }

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

    virtual public void OnDamage(float dmg, Vector2 kBackDir, float kBackMag, GameObject attacker)
    {

        if (!isHurt &&currentStage != BossStage.Transition)
        {
            isHurt = true;
            currentHealth -= dmg;
            if (currentHealth <= 0f)
            {
                currentHealth = 0f;
                UI.DoHurtUpdate(currentHealth);
            }
            flashVFX.Flash();
            UI.DoHurtUpdate(currentHealth);
            EvaluateToTransition();
        }
    
    }

    virtual protected void HurTimer()
    {
      
        if(currHurtTime <= 0f)
        {
            isHurt = false;
            flashVFX.EndFlash();
            currHurtTime = maxHurtTime;
        }
        else
        {
            currHurtTime -= Time.deltaTime;
        }
        
    }

    virtual public void EvaluateToTransition()
    {
        float healthPercent;
        switch (currentStage)
        {
            case BossStage.Initial:
                 healthPercent = currentHealth / maxHealth;
                if(healthPercent<= 2f/3f)
                {
                    Debug.Log("Transition initial");
                    currentHealth = maxHealth * (2f / 3);

                    UI.DoHurtUpdate(currentHealth);
                    BeginTransitionStage();
                }
                break;
            case BossStage.Middle:
                healthPercent = currentHealth / maxHealth;
                if (healthPercent <= 1f / 3f)
                {
                    Debug.Log("Transition Middle");
                    currentHealth = maxHealth * (1f / 3);

                    UI.DoHurtUpdate(currentHealth);
                    BeginTransitionStage();
                }
                break;
        }

    }

    virtual public void BeginTransitionStage()
    {
        
        if (!transitionAbility)
            transitionAbility = ObjectPoolManager.Spawn(transitionAbilityPrefab.gameObject, transform, Vector3.zero).GetComponent<BaseBossAbility>();


        if (currentStageAbilities.Count >0)
        {
            foreach (BaseBossAbility ability in currentStageAbilities)
            {
                ability.DisableAbility();
                ObjectPoolManager.Recycle(ability.gameObject);
            }
            currentStageAbilities.Clear();
        }
        InitialiseAbility(transitionAbility);
        isBusy = false;
        currentStage = BossStage.Transition;
        currentStageIndex++;
        Debug.Log("Current Stage index= " + currentStageIndex);
        OnNewState(AIState.Attack);
        
    }

    virtual public void OnDestroy()
    {
        if (UI) ObjectPoolManager.Recycle(UI.gameObject);
    }
}
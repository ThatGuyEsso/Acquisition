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
    [SerializeField] protected string BossName;
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float aiTickRate= 0.25f;
    public Transform target;
    [SerializeField] protected BossStageData stageData;
    [SerializeField] protected GameObject bossUIPrefab;
    [Header("Components")]
    protected SpriteFlash flashVFX;
    [SerializeField] protected Transform firepoint;
    [SerializeField] protected TDNavMeshMovement navigation;
    [SerializeField] protected FaceTarget faceTarget;
    [SerializeField] protected FaceMovementDirection faceMovementDirection;
    [SerializeField] protected GameObject transitionShieldPrefab;

    [SerializeField] protected float maxHurtTime;
    [SerializeField] protected List<BaseBossAbility> currentStageAbilities = new List<BaseBossAbility>();

    [Header("Animation Settings")]
    [SerializeField] protected string awakenAnimName;
    [SerializeField] protected string bossDeathAnim;
    [SerializeField] protected AttackAnimEventListener attackAnimEvents;
    [SerializeField] protected Animator animator;

    [Header("SFX Settings")]
    [SerializeField] protected string awakenSFXName;
    [SerializeField] protected string hurtSFX;
    [SerializeField] protected string dieSFX;
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
    [HideInInspector]
    public BossUI UI;
    [SerializeField] protected GameObject transitionAbilityPrefab;
    [SerializeField] protected BaseBossAbility closeCombatAbility;

    protected BaseBossAbility transitionAbility;
    [SerializeField] protected bool isBusy = false;
    [SerializeField] protected bool inDebug=false;
    protected bool isFighting;
    protected bool isDead;
    protected bool isTransitioning;
    protected GameObject transitionShield;
    public System.Action OnAwakened; 
    protected void Awake()
    {
        if (animator.enabled) animator.enabled = false;
        if (inDebug) Init();
        if (GameManager.instance) GameManager.instance.OnNewEvent += EvaluateeNewGameEvents;
    }

    public void EvaluateeNewGameEvents(GameEvents gameEvent)
    {
        if (gameEvent == GameEvents.PlayerDefeat)
        {
            CancelBoss();
        }
    }
    virtual public void Init()
    {
        currentHealth = maxHealth;
        currentAttackIndex = 0;
        currentStage = BossStage.Transition;
        currentStageIndex = 0;
        currentAIState = AIState.Chase;
        currHurtTime = maxHurtTime;

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
        animator.enabled = true;
        UI.OnUISpawned -= AwakenBoss;
        if (inDebug)
            attackAnimEvents.OnAnimEnd += BeginFight;
        else
        {
            attackAnimEvents.OnAnimEnd += CallAwake;
        }

        animator.Play(awakenAnimName, 0, 0f);
        if (AudioManager.instance)
        {
            AudioManager.instance.PlayThroughAudioPlayer(awakenSFXName, transform.position);
        }
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

    public void CancelBoss()
    {
        if (currentStageAbilities.Count > 0)
        {
            foreach (BaseBossAbility ability in currentStageAbilities)
            {
                ability.DisableAbility();
                if (ability.gameObject)
                    ObjectPoolManager.Recycle(ability.gameObject);
            }
            currentStageAbilities.Clear();
            if (transitionAbility)
            {
                if (transitionAbility.isEnabled) transitionAbility.DisableAbility();

                ObjectPoolManager.Recycle(transitionAbility.gameObject);
            }

        }
        if (transitionShield)
        {
            ObjectPoolManager.Recycle(transitionShield);
            transitionShield = null;
        }
        OnNewState(AIState.Idle);
        isFighting = false;
        
    }
    virtual public void CycleToNextAttack()
    {
        if (isDead) return;
        if(currentStage!= BossStage.Transition)
        {
            if(currentAttackIndex< currentStageAbilities.Count)
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


        if (transitionShield)
        {
            ObjectPoolManager.Recycle(transitionShield);
            transitionShield = null;
        }
        if (currentStageAbilities.Count<=0)
        {
            foreach (BaseBossAbility ability in currentStageAbilities)
            {
                if(ability.gameObject)
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
        if (currentStageAbilities.Count <= 0) return;
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
            if (GameManager.instance) GameManager.instance.OnNewEvent -= EvaluateeNewGameEvents;
        }

    }
    public void DoAttack()
    {
        if(currentStage != BossStage.Transition)
        {
            if (!currentStageAbilities[currentAttackIndex])
            {
                CycleToNextAttack();
                return;
            }
            if (!currentStageAbilities[currentAttackIndex].isEnabled) currentStageAbilities[currentAttackIndex].EnableAbility();
            if (currentStageAbilities[currentAttackIndex].IsManagingAttack()) canLockOn = true;
            else canLockOn = false;
            animator.Play(currentStageAbilities[currentAttackIndex].AnimationName(),0,0f);
        }
        else
        {
            if (transitionAbility)
            {
                if (!transitionAbility.isEnabled) transitionAbility.EnableAbility();
                if (transitionAbility.IsManagingAttack()) canLockOn = true;
                else canLockOn = false;
                animator.Play(transitionAbility.AnimationName(),0,0f);
            }
        }

    }
    public void DoCloseQuarters()
    {
        if (closeCombatAbility&& closeCombatAbility != currentStageAbilities[currentAttackIndex])
        {
           if (!closeCombatAbility.isEnabled) closeCombatAbility.EnableAbility();
            if (closeCombatAbility.IsManagingAttack()) canLockOn = true;
            else canLockOn = false;
            animator.Play(closeCombatAbility.AnimationName(),0,0f);
        }
   
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
    public bool GetIsBusy( ) { return isBusy; }
    public void PlayAnimation(string animName)
    {
        if (isDead) return;
        animator.Play(animName);
    }

    virtual public void OnDamage(float dmg, Vector2 kBackDir, float kBackMag, GameObject attacker)
    {
  
        if (!isHurt &&currentStage != BossStage.Transition&&!isDead)
        {
          
            currentHealth -= dmg;
            if (currentHealth <= 0f)
            {
              
                currentHealth = 0f;
                KillBoss();
                isHurt = true;
                if (UI)
                    UI.DoHurtUpdate(currentHealth);
    

             
            }
            else
            {
                isHurt = true;
                flashVFX.Flash();
                UI.DoHurtUpdate(currentHealth);
                if (AudioManager.instance)
                {
                    AudioManager.instance.PlayThroughAudioPlayer(hurtSFX, transform.position,true);
                }

                EvaluateToTransition();
            }
       
        }
    
    }

    virtual protected void HurTimer()
    {
        if (isDead) return;
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
                if(healthPercent<= 2f/3f && healthPercent >= 1f / 3f)
                {
                    Debug.Log("Transition initial");
                    currentHealth = maxHealth * (2f / 3);

                    UI.DoHurtUpdate(currentHealth);
                    BeginTransitionStage();
          
                }
                break;
            case BossStage.Middle:
                healthPercent = currentHealth / maxHealth;
                if (healthPercent <= 1f / 3f&& healthPercent >0f )
                {
                    Debug.Log("Transition Middle");
                    currentHealth = maxHealth * (1f / 3);

                    UI.DoHurtUpdate(currentHealth);
                    BeginTransitionStage();
                
                }
                break;
            case BossStage.End:
           
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
                if(ability.gameObject)
                    ObjectPoolManager.Recycle(ability.gameObject);
            }
            currentStageAbilities.Clear();
            closeCombatAbility = null;
        }
        InitialiseAbility(transitionAbility);
        isBusy = false;
        currentStage = BossStage.Transition;
        currentStageIndex++;
        Debug.Log("Current Stage index= " + currentStageIndex);
        OnNewState(AIState.Attack);
        transitionShield = ObjectPoolManager.Spawn(transitionShieldPrefab, transform);
    }
    public void KillBoss()
    {
        isDead = true;
       
        if (currentStageAbilities.Count > 0)
        {
            foreach (BaseBossAbility ability in currentStageAbilities)
            {
                ability.DisableAbility();
                if(ability.gameObject)
                    ObjectPoolManager.Recycle(ability.gameObject);
            }
            currentStageAbilities.Clear();
        }
        animator.enabled = true;
        if (!attackAnimEvents.enabled) attackAnimEvents.enabled = true;
        attackAnimEvents.OnDeathComplete += EndBossFight;

        if (AudioManager.instance && dieSFX!=string.Empty) {
            AudioManager.instance.PlayThroughAudioPlayer(dieSFX, transform.position);
        }
        animator.Play(bossDeathAnim,0,0f);
        isBusy = false;
        if(UI)
        UI.HideUI();
        navigation.Stop();
      
        navigation.enabled = false;
        faceTarget.enabled = false;
    }

    virtual public void EndBossFight()
    {

        attackAnimEvents.OnDeathComplete -= EndBossFight;
        isFighting = false;
        if (GameManager.instance)
            GameManager.instance.BeginNewEvent(GameEvents.BossDefeated);
    }
    virtual public void OnDestroy()
    {
   
        if (UI) ObjectPoolManager.Recycle(UI.gameObject);
        if (GameManager.instance) GameManager.instance.OnNewEvent -= EvaluateeNewGameEvents;
    }

    public Transform GetTarget()
    {
        return target;
    }


    public SpriteRenderer GetRenderer() {

        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
 
        return renderer;
    }

    public AfterImageController GetAfterimageController()
    {

        AfterImageController afterImageController = GetComponentInChildren<AfterImageController>();
        
        return afterImageController;
    }

        

    
}
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
public abstract class BaseBossAI : MonoBehaviour,IInitialisable
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


    [SerializeField] protected AttackAnimEventListener attackAnimEvents;
    [SerializeField] protected List<Component> componentsToInit = new List<Component>();
    [SerializeField] protected List<BaseBossAbility> currentStageAbilities = new List<BaseBossAbility>();

    protected bool isInitialised;
    protected float currentHealth;
    protected int currentAttackIndex;
    protected BossStage currentStage;
    protected AIState currentAIState;

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
            attackAnimEvents.OnShowAttackZone += OnAttackBegin;
            attackAnimEvents.OnShowAttackZone += OnAttackEnd;
        }
        SetUpNextStage();
        isInitialised = true;
        InvokeRepeating("ProcessAI", 0.0f, aiTickRate);
    }

    virtual public bool InRange()
    {
        if (currentStageAbilities.Count <= 0) return false;
        if (!target) return false;
        return Vector2.Distance(target.position, transform.position) <= currentStageAbilities[currentAttackIndex].Range();

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


    virtual protected void CycleToNextAttack()
    {
        currentAttackIndex++;
        if (currentAttackIndex >= currentStageAbilities.Count) currentAttackIndex = 0;
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

            BaseBossAbility ability = abilities[i].GetComponent<BaseBossAbility>();
            if (ability)
            {
                ObjectPoolManager.Spawn(ability.gameObject, transform, Vector3.zero);
                if (ability.IsCloseRange()) closeCombatAbility = ability;
                ability.eventListener = attackAnimEvents;
                ability.owner = this;
                ability.Init();
                currentStageAbilities.Add(ability);
            }
        }
    }


    public void OnAttackBegin() { isBusy = true; }
    public void OnAttackEnd() {
        isBusy = false; 
        
    }


    public void BeginLongCooldown(float cooldown)
    {
        OnNewState(AIState.Idle);
        StopAllCoroutines();
        StartCoroutine(LongCooldown(cooldown));

    }


    protected IEnumerator LongCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        CycleToNextAttack();
        OnNewState(AIState.Chase);
    }
    protected void OnDisable()
    {
        if (isInitialised)
        {
            attackAnimEvents.OnShowAttackZone -= OnAttackBegin;
            attackAnimEvents.OnShowAttackZone -= OnAttackEnd;
        }

    }
    public void DoAttack()
    {
        animator.Play(currentStageAbilities[currentAttackIndex].AnimationName());
    }
}

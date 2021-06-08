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

    [Header("Componeents")]
    [SerializeField] protected TDNavMeshMovement navigation;
    [SerializeField] protected FaceTarget faceTarget;
    [SerializeField] protected FaceMovementDirection faceMovementDirection;
    [SerializeField] protected List<Component> componentsToInit = new List<Component>();
    protected float currentHealth;
    protected int currentAttack;
    protected BossStage currentStage;
    protected AIState currentAIState;

    [SerializeField] protected bool inDebug=false;
    protected void Awake()
    {
        if (inDebug) Init();
    }
    virtual public void Init()
    {
        currentHealth = maxHealth;
        currentAttack = 0;
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
    

        InvokeRepeating("ProcessAI", 0.0f, aiTickRate);
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


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightBoss : BaseBossAI,IInitialisable, IBoss,IDamage
{


    [SerializeField] private Rigidbody2D rb;

    public override void Init()
    {
        base.Init();
        rb = GetComponent<Rigidbody2D>();
        rb.Sleep();
        
    }
    protected override void ProcessAI()
    {
        if (!isFighting) return;
            if(currentStage == BossStage.Transition && !isBusy)
            {
                CycleToNextAttack();
            }
            switch (currentAIState)
            {
                case AIState.Idle:
                    break;
                case AIState.Chase:
                    if (currentStage != BossStage.Transition)
                    {
                        if (target)
                        {

                            navigation.SetDestination();
                            if (InRange())
                            {
                                OnNewState(AIState.Attack);
                            }
                            else
                            {
                                if( currentAttackIndex < currentStageAbilities.Count)
                                {
                                    if (currentStageAbilities[currentAttackIndex].IsPriority()) OnNewState(AIState.Chase);
                                    else
                                    {
                                        CycleToNextAttack();
                                        OnNewState(AIState.Chase);
                                    }
                                }
                                else 
                                {
                                    CycleToNextAttack();
                                    OnNewState(AIState.Chase);
                                }

                        }
                        }
                    }
           
                    break;
                case AIState.Attack:
                    if(currentStage != BossStage.Transition)
                    {
                        if (!InRange())
                        {
                            OnNewState(AIState.Chase);
                        }
                        else if (currentAttackIndex < currentStageAbilities.Count)
                        {
                            if (currentStageAbilities[currentAttackIndex].CanAttack() && !isBusy)
                            {
                                DoAttack();
                            }
                       
                       

                        }
                        else if (!isBusy)

                        {
                            CycleToNextAttack();
                            OnNewState(AIState.Chase);
                        }


                    }
                    else  
                    {
                        if (transitionAbility)
                        {
                            if (!InRange())
                            {
                                OnNewState(AIState.Chase);
                            }
                            else if (transitionAbility.CanAttack() && !isBusy)
                            {
                                DoAttack();
                            }

                        }
                    }
               
          
                    break;
            }
    }

    private void Update()
    {
        if (!isFighting) return;
        if (isDead) return;
        if (isHurt)
        {
            HurTimer();
        }
        //switch (currentAIState)
        //{
        //    case AIState.Idle:
        //        break;
        //    case AIState.Chase:
             
        //        break;
        //    case AIState.Attack:
        

        //        break;
        //}
    }
    private void LateUpdate()
    {
        if (!isFighting) return;
        if (isDead) return;
        switch (currentAIState)
        {
            case AIState.Idle:
                break;
            case AIState.Chase:
                faceMovementDirection.SmoothRotToMovement(navigation.navAgent.velocity);
                break;
            case AIState.Attack:

                if (!isBusy ||canLockOn)
                {
                    faceTarget.FaceCurrentTarget();
                }
                break;
        }
    }

    override public void OnNewState(AIState newState)
    {
        if (!isFighting) return;
        if (!isBusy)
        {
            currentAIState = newState;
            switch (currentAIState)
            {
                case AIState.Idle:
                    navigation.Stop();
                    rb.velocity = Vector2.zero;
                    navigation.enabled = false;
                    faceTarget.enabled = false;
                    if (!isDead) 
                        animator.Play("Idle");
                    break;
                case AIState.Chase:
                    if (isDead) return;
                    navigation.enabled = true;
                    faceTarget.enabled = false;
                    if (target)
                        navigation.StartAgent(target);
                   
                    animator.Play("Walking");
                    break;
                case AIState.Attack:
                    if (isDead) return;
                    navigation.Stop();
                    navigation.enabled = false;
                    faceTarget.enabled = true;
                    faceTarget.SetTarget(target);
                    if(currentStage != BossStage.Transition)
                    {
                        if (currentStageAbilities[currentAttackIndex].CanAttack() && !isBusy)
                        {
                            if (closeCombatAbility)
                            {
                                if (InCloseRange() && closeCombatAbility.CanAttack()&&!isBusy)
                                {
                                    DoCloseQuarters();
                                }
                                else
                                {
                                    DoAttack();
                                }
                            }
                            else
                            {
                                DoAttack();
                            }

                        }
                    }
                    else
                    {
                        DoAttack();
                    }

        
                    break;
            }
        }
     
   
    }

    public override Transform GetFirePoint()
    {
        if (firepoint)
            return firepoint;
        else return transform;
    }

    public void UseRigidBody(bool useRigidBody)
    {
        if (useRigidBody) rb.WakeUp();
        else rb.Sleep();
    }

    public override void SetUseRigidBody(bool use)
    {
        UseRigidBody(use);
    }

    public override Rigidbody2D GetRigidBody()
    {
        return rb;
    }

    public override void EndBossFight()
    {
        rb.velocity = Vector2.zero;
        GameStateManager.instance.runtimeData.isKnightDefeated = true;
        attackAnimEvents.OnDeathComplete -= EndBossFight;
        isFighting = false;
        if (GameManager.instance)
            GameManager.instance.BeginNewEvent(GameEvents.BossDefeated);
    }

    public override void BeginTransitionStage()
    {
        if (!transitionAbility)
        {
            transitionAbility = ObjectPoolManager.Spawn(transitionAbilityPrefab.gameObject, transform, Vector3.zero).GetComponent<BaseBossAbility>();
        
        }


        if (currentStageAbilities.Count > 0)
        {
            foreach (BaseBossAbility ability in currentStageAbilities)
            {
                ability.DisableAbility();
                if (ability.gameObject)
                    ObjectPoolManager.Recycle(ability.gameObject);
            }
            currentStageAbilities.Clear();
            closeCombatAbility = null;
        }


    
        if (BossRoomManager.instance && GetDistanceToCentre()> 1f)
        {
            isBusy = true;
         
            currentStage = BossStage.Transition;
            currentAIState = AIState.Chase;
            navigation.enabled = true;
            faceTarget.enabled = false;
            navigation.MoveToPoint(BossRoomManager.instance.GetRoomCentrePoint());
            animator.Play("Walking");
            StartCoroutine(WaitTilleAtCentre());
            transitionShield = ObjectPoolManager.Spawn(transitionShieldPrefab, transform);

        }
        else
        {
            InitialiseAbility(transitionAbility);
            isBusy = false;
            currentStageIndex++;
            Debug.Log("Current Stage index= " + currentStageIndex);
            currentStage = BossStage.Transition;
            OnNewState(AIState.Attack);
            transitionShield = ObjectPoolManager.Spawn(transitionShieldPrefab, transform);
        }

        OnAbilityAdded?.Invoke(transitionAbility);
    }


    private IEnumerator WaitTilleAtCentre()
    {
        while(GetDistanceToCentre() > 0.2f)
        {
            yield return null;
        }
        InitialiseAbility(transitionAbility);
        isBusy = false;
        currentStage = BossStage.Transition;
        currentStageIndex++;
        Debug.Log("Current Stage index= " + currentStageIndex);
        OnNewState(AIState.Attack);

    }
    private float GetDistanceToCentre()
    {
        return Vector2.Distance(BossRoomManager.instance.GetRoomCentrePoint(), transform.position);

    }
    protected override void SetUpNextStage()
    {
        base.SetUpNextStage();
        switch (currentStage)
        {
            case BossStage.Initial:
                animator.SetFloat("ChargeBuildSpeed", 1.0f);
                break;
            case BossStage.Middle:

                animator.SetFloat("ChargeBuildSpeed", 2f);
                break;
            case BossStage.End:

                animator.SetFloat("ChargeBuildSpeed", 3f);
                break;
        }
    }
}

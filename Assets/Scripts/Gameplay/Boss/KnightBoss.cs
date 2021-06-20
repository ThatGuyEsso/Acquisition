using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightBoss : BaseBossAI,IInitialisable, IBoss,IDamage
{

    [SerializeField] private Transform firepoint;
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
                            if (currentStageAbilities[currentAttackIndex].IsPriority()) OnNewState(AIState.Chase);
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
                    else if (currentStageAbilities[currentAttackIndex].CanAttack() && !isBusy)
                    {
                        DoAttack();
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
                    navigation.enabled = false;
                    faceTarget.enabled = false;
                    animator.Play("Idle");
                    break;
                case AIState.Chase:
                    navigation.enabled = true;
                    faceTarget.enabled = false;
                    if (target)
                        navigation.StartAgent(target);
                   
                    animator.Play("Walking");
                    break;
                case AIState.Attack:
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
}

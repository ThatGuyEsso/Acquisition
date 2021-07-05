using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheScholar : BaseBossAI
{
    protected override void ProcessAI()
    {
        if (!isFighting) return;
        if (currentStage == BossStage.Transition && !isBusy)
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
                            if (currentAttackIndex < currentStageAbilities.Count)
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
                if (currentStage != BossStage.Transition)
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

                if (!isBusy || canLockOn)
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
                    navigation.enabled = false;
                    faceTarget.enabled = false;
                    if (isDead)
                    {
                        animator.enabled = true;
                        animator.Play("ScholarDeath");
                    }
                    else
                        animator.Play("Idle");
                    break;
                case AIState.Chase:
                    if (isDead) return;
                    navigation.enabled = true;
                    faceTarget.enabled = false;
                    if (target)
                        navigation.StartAgent(target);

                    animator.Play("Walk");
                    break;
                case AIState.Attack:
                    if (isDead) return;
                    navigation.Stop();
                    navigation.enabled = false;
                    faceTarget.enabled = true;
                    faceTarget.SetTarget(target);
                    if (currentStage != BossStage.Transition)
                    {
                        if (currentStageAbilities[currentAttackIndex].CanAttack() && !isBusy)
                        {
                            if (closeCombatAbility)
                            {
                                if (InCloseRange() && closeCombatAbility.CanAttack() && !isBusy)
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


    public override void EndBossFight()
    {
        GameStateManager.instance.runtimeData.isScholarDefeated = true;
        attackAnimEvents.OnDeathComplete -= EndBossFight;
        isFighting = false;
        if (GameManager.instance)
            GameManager.instance.BeginNewEvent(GameEvents.BossDefeated);
    }


    override protected void SetUpNextStage()
    {
        base.SetUpNextStage();


        switch (currentStage)
        {
            case BossStage.Initial:
                animator.SetFloat("CastSpeed",1.0f);
                break;
            case BossStage.Middle:
            
                animator.SetFloat("CastSpeed", 2f);
                break;
            case BossStage.End:

                animator.SetFloat("CastSpeed", 3f);
                break;
        }
    }
}

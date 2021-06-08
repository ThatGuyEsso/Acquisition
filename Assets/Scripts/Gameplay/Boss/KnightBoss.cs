using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightBoss : BaseBossAI
{
    protected override void ProcessAI()
    {
        switch (currentAIState)
        {
            case AIState.Idle:
                break;
            case AIState.Chase:
                if (target)
                {
              
                    navigation.SetDestination();
                }
                break;
            case AIState.Attack:
                break;
        }
    }

    private void LateUpdate()
    {
        switch (currentAIState)
        {
            case AIState.Idle:
                break;
            case AIState.Chase:
                faceMovementDirection.SmoothRotToMovement(navigation.navAgent.velocity);
                break;
            case AIState.Attack:
                break;
        }
    }

    override protected void OnNewState(AIState newState)
    {
        currentAIState = newState;
        switch (currentAIState)
        {
            case AIState.Idle:
                navigation.enabled = false;
                faceTarget.enabled = false;
                break;
            case AIState.Chase:
                navigation.enabled = true;
                faceTarget.enabled = false;
                if (target)
                    navigation.StartAgent(target);
                break;
            case AIState.Attack:
                navigation.enabled = false;
                faceTarget.enabled = true;
                faceTarget.SetTarget(target);
                break;
        }
    }
}

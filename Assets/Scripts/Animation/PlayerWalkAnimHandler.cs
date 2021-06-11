using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkAnimHandler : MonoBehaviour
{
    public Animator animator;
    public TDInputMovement movement;


    private void Awake()
    {
        movement.OnWalk += OnPlayerWalk;
        movement.OnStop += OnStopWalk;
    }
    public void OnPlayerWalk()
    {
        animator.SetFloat("PlaySpeed", 1f);
        animator.Play("Run");
    }

    public void OnStopWalk()
    {
        animator.SetFloat("PlaySpeed", 0f);
    }

    public void OnDestroy()
    {
        movement.OnWalk += OnPlayerWalk;
        movement.OnStop += OnStopWalk;
    }
}

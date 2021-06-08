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
public abstract class BaseBossAI : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] protected float maxHealth;
    protected float currentHealth;
    protected int currentAttack;


}

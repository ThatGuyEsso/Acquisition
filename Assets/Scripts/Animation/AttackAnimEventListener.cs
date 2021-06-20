using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimEventListener: MonoBehaviour
{
    [SerializeField] private Animator animator;

    public Action OnShowAttackZone;
    public Action OnHideAttackZone;
    public Action OnShootProjectile;
    public Action OnAnimStart;
    public Action OnAnimEnd;
    public Action OnChargeIncrease;
    public Action OnDeathComplete;
    public void OnAnimBegun()
    {
        OnAnimStart?.Invoke();
    }
    public void OnDie()
    {
        OnDeathComplete?.Invoke();
    }


    public void OnAnimFinished()
    {
        OnAnimEnd?.Invoke();
    }
    public void OnSpawnAttackZone()
    {
        OnShowAttackZone?.Invoke();

    }

    public void OnDespawnAttackZone()
    {
        OnHideAttackZone?.Invoke();

    }
    public void OnArrowChargeIncrement()
    {
        OnChargeIncrease?.Invoke();

    }


    public void OnSpawnProjectile()
    {
        OnShootProjectile?.Invoke();
    }
}

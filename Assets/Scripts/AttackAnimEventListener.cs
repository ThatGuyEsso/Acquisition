using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimEventListener: MonoBehaviour
{
    [SerializeField] private Animator animator;

    public Action OnShowAttackZone;
    public Action OnHideAttackZone;
    public Action OnShootProjectile;


    public void OnSpawnAttackZone()
    {
        OnShowAttackZone?.Invoke();
    }

    public void OnDespawnAttackZone()
    {
        OnHideAttackZone?.Invoke();
    }

    public void OnSpawnProjectile()
    {
        OnHideAttackZone?.Invoke();
    }
}

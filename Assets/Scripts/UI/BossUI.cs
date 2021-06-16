using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class BossUI : MonoBehaviour
{ 
    public static BossUI instance;
    public ScalingProgressBar progressBar;
    [SerializeField] private TextMeshProUGUI bossNameDisplay;
    [SerializeField] private Animator animator;
    [SerializeField] private UIElementShake uiShaker;
    public Action OnUISpawned;
    public void InitialiseUI(string bossName)
    {
        bossNameDisplay.name = bossName;
        animator.Play("InitHealth");
    }


    public void OnUISpawnAnimComplete()
    {
        OnUISpawned?.Invoke();
        animator.enabled = false;
    }

    public void DoHurtUpdate(float newHealth)
    {
        progressBar.UpdateValue(newHealth);
        uiShaker.BeginViewBob();

    }

}

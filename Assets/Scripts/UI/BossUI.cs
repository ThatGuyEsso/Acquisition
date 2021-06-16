using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossUI : MonoBehaviour
{ 
    public static BossUI instance;
    [SerializeField] private ScalingProgressBar progressBar;
    [SerializeField] private TextMeshProUGUI bossNameDisplay;
    [SerializeField] private Animator animator;

    public void InitialiseUI(string bossName)
    {
        bossNameDisplay.name = bossName;
        animator.Play("InitHealth");
    }



}

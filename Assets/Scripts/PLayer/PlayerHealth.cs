using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamage,IInitialisable
{
    [SerializeField] private int maxHitPoints;
    [SerializeField] private float maxHurtTime;
    private float currHurtTime;
    private int currentHitPoint;
    private Rigidbody2D rb;
    private bool isHurt = false;

    public Action OnHurt;
    public Action OnNotHurt;
    public Action OnDie;
    public void Init()
    {
        currentHitPoint = maxHitPoints;
        rb = GetComponent<Rigidbody2D>();
        currHurtTime = maxHurtTime;
    }


    public void OnDamage(float dmg, Vector2 kBackDir, float kBackMag, GameObject attacker)
    {
        if (!isHurt)
        {
            currentHitPoint--;
            if(currentHitPoint <= 0)
            {
                KillPlayer();
            }
            else
            {
                isHurt = true;

                rb.AddForce(kBackDir * kBackMag, ForceMode2D.Impulse);
                OnHurt?.Invoke();
            }
   

        }

    }

    private void Update()
    {
        if (isHurt)
        {
            if(currHurtTime <= 0)
            {
                isHurt = false;
                OnNotHurt?.Invoke();
            }
        }
    }

    public void KillPlayer()
    {
        OnDie?.Invoke();
        Debug.Log("PlayerDied");
    }
}

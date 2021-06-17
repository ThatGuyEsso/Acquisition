using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamage,IInitialisable,ICharacterComponents
{
    [SerializeField] private int maxHitPoints;
    [SerializeField] private float maxHurtTime;
    [SerializeField] private float knockbackDecel;
    [SerializeField] private float knockBackMoveScalar=4f;
    [SerializeField] private TDInputMovement movement;
    [SerializeField] private GameObject deathMask;
    private float currHurtTime;
    private int currentHitPoint;
    private Rigidbody2D rb;
    private bool isHurt = false;
    Vector2 currentKnockBack;
    public Action OnHurt;
    public Action OnNotHurt;
    public Action OnDie;
    public void Init()
    {
        currentHitPoint = maxHitPoints;
        rb = GetComponent<Rigidbody2D>();
        currHurtTime = maxHurtTime;
        currentKnockBack = Vector2.zero;
        movement = GetComponent<TDInputMovement>();
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

                currentKnockBack = kBackDir * kBackMag;

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
            else
            {
                currHurtTime -= Time.deltaTime;
            }
        }

  
    }
    private void FixedUpdate()
    {

        if (currentKnockBack != Vector2.zero)
        {
            if (movement.IsCharacterMoving())
            {
                rb.AddForce(currentKnockBack * Time.deltaTime, ForceMode2D.Force);

                currentKnockBack = Vector2.Lerp(currentKnockBack, Vector2.zero, Time.deltaTime * knockbackDecel);
                if (currentKnockBack.magnitude <= 0.05f) currentKnockBack = Vector2.zero;
            }
            else
            {
                rb.velocity = currentKnockBack * Time.deltaTime;

                currentKnockBack = Vector2.Lerp(currentKnockBack, Vector2.zero, Time.deltaTime * knockbackDecel);
                if (currentKnockBack.magnitude <= 0.05f) currentKnockBack = Vector2.zero;
            }
        }
    }
    public void KillPlayer()
    {
        OnDie?.Invoke();
        GameManager.instance.BeginNewEvent(GameEvents.PlayerDefeat);
        ObjectPoolManager.Spawn(deathMask, transform.position, Quaternion.identity);
        Debug.Log("PlayerDied");
    }

    public void EnableComponent()
    {
        //
    }

    public void DisableComponent()
    {
        isHurt = false;
    }

    public void ResetComponent()
    {
        currHurtTime = maxHurtTime;
        isHurt = false;
        currentHitPoint = maxHitPoints;
    }
}

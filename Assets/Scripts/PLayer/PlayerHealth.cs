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
    [SerializeField] private SpriteFlash flashVFX;
    [SerializeField] private Animator deathAnimController;
    private float currHurtTime;
    private int currentHitPoint;
    private bool isDead = false;


    bool isHurt;
    public void Init()
    {
        currentHitPoint = maxHitPoints;
     
        currHurtTime = maxHurtTime;
        if (!flashVFX) flashVFX = GetComponent<SpriteFlash>();
    
        movement = GetComponent<TDInputMovement>();
        if (GameManager.instance)
        {
            GameManager.instance.OnNewEvent += EvaluateNewEvent;
        }
        
    }

    public void EvaluateNewEvent(GameEvents events)
    {
        switch (events)
        {
            case GameEvents.BossDefeated:
                currHurtTime = maxHurtTime;
                currentHitPoint = maxHitPoints;

                UpdateHealthDisplay();
                break;
            case GameEvents.PlayerDefeat:

                if (deathAnimController)
                {

                    deathAnimController.gameObject.SetActive(true);
                    AttackAnimEventListener animEvents = deathAnimController.GetComponent<AttackAnimEventListener>();
                    if(animEvents) animEvents.OnDeathComplete += ResetGame;
                }
                break;
            case GameEvents.DeathMaskComplete:
                if (deathAnimController)
                {
                   
                    deathAnimController.Play("deathAnimController", 0, 0f);
                }
                break;
        }
    }

    public void OnDamage(float dmg, Vector2 kBackDir, float kBackMag, GameObject attacker)
    {
        if (!isHurt&&!isDead)
        {
            if (CamShake.instance)
                CamShake.instance.DoScreenShake(0.25f, 5f, 0.05f, 0.5f, 5f);
            currentHitPoint--;
        
            if(currentHitPoint < 0)
            {
                KillPlayer();
            }
            else
            {
                if (AudioManager.instance) AudioManager.instance.PlayThroughAudioPlayer("PlayerDamage", transform.position);
                isHurt = true;
                UpdateHealthDisplay();
                //currentKnockBack = kBackDir * kBackMag;
                if (flashVFX) flashVFX.Flash();
           
            }
            

        }

    }

    public void ResetGame()
    {
        AttackAnimEventListener animEvents = deathAnimController.GetComponent<AttackAnimEventListener>();
        animEvents.OnDeathComplete += ResetGame;
        if (GameManager.instance)
            GameManager.instance.BeginNewEvent(GameEvents.RespawnPlayer);
    }

    private void Update()
    {
        if (isHurt)
        {
            if(currHurtTime <= 0)
            {
                isHurt = false;
                if (flashVFX) flashVFX.EndFlash();
                currHurtTime = maxHurtTime;
               
            }
            else
            {
                currHurtTime -= Time.deltaTime;
            }
        }

  
    }


    public void UpdateHealthDisplay()
    {
        if (PostProcessingManager.instance)
        {
            if (currentHitPoint == maxHitPoints)
            {
                PostProcessingManager.instance.ReturnToDefault();
            }else if(currentHitPoint == maxHitPoints-1)
            {

                PostProcessingManager.instance.ApplyLightDamageProfile();
            }else if(currentHitPoint == maxHitPoints - 2)
            {
                PostProcessingManager.instance.ApplyMidDamageProfile();
            }
        }
        else if (currentHitPoint == maxHitPoints - 3)
        {
            PostProcessingManager.instance.ApplyMaxDamageProfile();
        }
        else if (currentHitPoint <= maxHitPoints - 4)
        {
            PostProcessingManager.instance.ApplyMaxDamageProfile();
        }
    }
     
    
    private void FixedUpdate()
    {

        //if (currentKnockBack != Vector2.zero)
        //{
        //    //if (movement.IsCharacterMoving())
        //    //{
        //    //    rb.AddForce(currentKnockBack*Time.deltaTime,ForceMode2D.Force);

        //    //    currentKnockBack = Vector2.Lerp(currentKnockBack, Vector2.zero, Time.deltaTime * knockbackDecel);
        //    //    if (currentKnockBack.magnitude <= 0.05f) currentKnockBack = Vector2.zero;
        //    //}
        //}
        //else
        //{
        //    rb.velocity = currentKnockBack * Time.deltaTime;

        //    currentKnockBack = Vector2.Lerp(currentKnockBack, Vector2.zero, Time.deltaTime * knockbackDecel);
        //    if (currentKnockBack.magnitude <= 0.05f) currentKnockBack = Vector2.zero;
        //}
    }
    public void KillPlayer()
    {
        isDead = true;
        if (AudioManager.instance) AudioManager.instance.PlayThroughAudioPlayer("PlayerDeath", transform.position);
   
        if(GameManager.instance)
            GameManager.instance.BeginNewEvent(GameEvents.PlayerDefeat);
        ObjectPoolManager.Spawn(deathMask, transform.position, Quaternion.identity);


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

        UpdateHealthDisplay();
    }
}

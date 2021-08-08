using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;


public class AttackVolume : MonoBehaviour,IVolumes
{

    public Action OnPlayerHit;
    public Action OnObstacleHit;
    public Action WallHit;
    [SerializeField] protected bool isPlayerZone;

    [SerializeField] protected float damage;
    protected float knockBack;
    protected Vector2 direction;
    protected GameObject owner;


    
    protected void OnDisable()
    {
        StopAllCoroutines();
        if (GameManager.instance) GameManager.instance.OnNewEvent -= EvaluateGameEvent;
    }

    virtual protected void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayerZone)
        {
            if (other.CompareTag("Wall"))
            {
         
                OnObstacleHit?.Invoke();
                Debug.Log("Hit wall");
            }
            else if (other.CompareTag("Enemy"))
            {
                IDamage damageable = other.GetComponent<IDamage>();
                if (damageable != null)
                {
                    damageable.OnDamage(damage, direction, knockBack, owner);
                }
                Debug.Log("Hit Enemy");
            }
        }
        else
        {
            if (other.gameObject.CompareTag("Player"))
            {
                IDamage damageable = other.GetComponent<IDamage>();
                if (damageable != null)
                {
                    damageable.OnDamage(damage, direction, knockBack, owner);
                }
                OnPlayerHit?.Invoke();
                Debug.Log("Hit Player");
            }
            else if (other.gameObject.CompareTag("Wall"))
            {
                WallHit?.Invoke();
         ;
            }
        }

        if (other.gameObject.CompareTag("Obstacles"))
        {



            if (other.GetComponent<IDamage>() != null)
            {
                other.GetComponent<IDamage>().OnDamage(damage, transform.up, 0f, owner);
                OnObstacleHit?.Invoke();
            }



        }
    }

    virtual protected void OnTriggerStay2D(Collider2D other)
    {
        if (isPlayerZone)
        {
            if (other.CompareTag("Wall"))
            {

                WallHit?.Invoke();
                Debug.Log("Hit wall");
            }
            else if (other.CompareTag("Enemy"))
            {
                IDamage damageable = other.GetComponent<IDamage>();
                if (damageable != null)
                {
                    damageable.OnDamage(damage, direction, knockBack, owner);
                }
                Debug.Log("Hit Enemy");
            }
        }
        else
        {
            if (other.gameObject.CompareTag("Player"))
            {
                IDamage damageable = other.GetComponent<IDamage>();
                if (damageable != null)
                {
                    damageable.OnDamage(damage, direction, knockBack, owner);
                }
                OnPlayerHit?.Invoke();
                Debug.Log("Hit Player");
            }
            else if (other.gameObject.CompareTag("Wall"))
            {
                WallHit?.Invoke();
                Debug.Log("Hit wall");
            }
        }


    }


    virtual public void SetIsPlayerZone(bool isPlayerZone)
    {
        this.isPlayerZone = isPlayerZone;
    }

    virtual public void SetUpDamageVolume(float dmg, float kBack, Vector2 dir,GameObject owner)
    {
       
        damage=dmg;
        knockBack= kBack;
        direction=dir;
        this.owner = owner;
    }

    virtual public void EvaluateGameEvent(GameEvents gameEvent){
        switch (gameEvent)
        {
            case GameEvents.BossDefeated:
                StopAllCoroutines();
                if (gameObject) ObjectPoolManager.Recycle(gameObject);
                break;
            case GameEvents.PlayerDefeat:
                StopAllCoroutines();
                if (gameObject) ObjectPoolManager.Recycle(gameObject);
                break;
            case GameEvents.ExitGame:
                StopAllCoroutines();
                if (gameObject) ObjectPoolManager.Recycle(gameObject);
                break;
        }
    }

    private void OnEnable()
    {
        if (GameManager.instance) GameManager.instance.OnNewEvent += EvaluateGameEvent;
    }

    public IEnumerator TimeToDespawn(float time)
    {
        yield return new WaitForSeconds(time);
        ObjectPoolManager.Recycle(gameObject);
    } 
    public void SetDespawnTime(float time)
    {
        StartCoroutine(TimeToDespawn(time));
    }
}

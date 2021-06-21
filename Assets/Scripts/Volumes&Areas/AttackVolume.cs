using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;


public class AttackVolume : MonoBehaviour,IVolumes
{

    public Action OnPlayerHit;
    public Action OnObstacleHit;
    bool isPlayerZone;

    private float damage;
    private float knockBack;
    private Vector2 direction;
    private GameObject owner;

    private void OnEnable()
    {
        
    }
    public void OnTriggerEnter2D(Collider2D other)
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
                OnObstacleHit?.Invoke();
                Debug.Log("Hit wall");
            }
        }
 
     
    }

    public void SetIsPlayerZone(bool isPlayerZone)
    {
        this.isPlayerZone = isPlayerZone;
    }

    public void SetUpDamageVolume(float dmg, float kBack, Vector2 dir,GameObject owner)
    {
       
        damage=dmg;
        knockBack= kBack;
        direction=dir;
        this.owner = owner;
    }

    public void EvaluateGameEvent(GameEvents gameEvent){
       
    }
}

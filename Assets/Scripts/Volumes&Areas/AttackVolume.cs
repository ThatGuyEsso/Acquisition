using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;


public class AttackVolume : MonoBehaviour,IVolumes
{

    public Action OnPlayerHit;
    public Action OnObstacleHit;
    bool isPlayerZone;
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayerZone)
        {
            if (other.gameObject.CompareTag("Wall"))
            {
                OnObstacleHit?.Invoke();
                Debug.Log("Hit wall");
            }
            else if (other.gameObject.CompareTag("Enemy"))
            {
     
                Debug.Log("Hit Enemy");
            }
        }
        else
        {
            if (other.gameObject.CompareTag("Player"))
            {

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
}

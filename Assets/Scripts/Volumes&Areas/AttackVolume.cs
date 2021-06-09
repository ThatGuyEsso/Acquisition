using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class AttackVolume : MonoBehaviour
{

    public Action OnPlayerHit;
    public Action OnObstacleHit;
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")){
     
            OnPlayerHit?.Invoke();
            Debug.Log("Hit Player");
        }
        else if(other.gameObject.CompareTag("Wall"))
        {
            OnObstacleHit?.Invoke();
            Debug.Log("Hit wall");
        }
    }
}

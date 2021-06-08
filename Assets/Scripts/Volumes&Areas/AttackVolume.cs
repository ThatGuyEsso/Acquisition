using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackVolume : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")){
            Debug.Log("PLayer detected");
        }
    }
}

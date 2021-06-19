using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public BossRoomManager bossManager;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (bossManager)
            {
                bossManager.player = other.transform;
                bossManager.Init();
            }
                 


            
        }
    }
}

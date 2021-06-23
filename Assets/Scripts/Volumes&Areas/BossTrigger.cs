using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public BossRoomManager bossManager;

    bool isTriggered = false;
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")&&!isTriggered)
        {
            if (bossManager)
            {
                isTriggered = true;
                bossManager.player = other.transform;
                bossManager.Init();
            }
                 


            
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossExit : MonoBehaviour
{
    [SerializeField] private BossType bossType;
    bool isTriggered;
    [SerializeField] private BossDoor door;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")&&!isTriggered)
        {
            HubManager.instance.BeginBossRoomLoad(bossType);
            isTriggered = true;
           
        }

    }
}

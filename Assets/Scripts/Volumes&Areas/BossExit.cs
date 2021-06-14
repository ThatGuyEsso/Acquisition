using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossExit : MonoBehaviour
{
    [SerializeField] private BossType bossType;
    bool isTriggered;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")&&!isTriggered)
        {
            HubManager.instance.LoadSelectedBossRoom(bossType);
            isTriggered = true;
        }

    }
}

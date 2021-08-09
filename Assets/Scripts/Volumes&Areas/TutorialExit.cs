using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialExit : MonoBehaviour
{
    bool isTriggered;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            HubManager.instance.BeginTutorialRoomLoad();
            isTriggered = true;

        }

    }
}

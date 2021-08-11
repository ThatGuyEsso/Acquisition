using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHubTrigger : MonoBehaviour
{
    private bool isTriggered = false;
    [SerializeField] private TutorialHubManager manager;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            manager.StartTutorial();
        }
    }

}
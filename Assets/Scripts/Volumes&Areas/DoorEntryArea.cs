using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorEntryArea : MonoBehaviour
{
    [SerializeField] private SlidingDoor door;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (door.gameObject.activeInHierarchy)
            {
                door.BeginToOpen();
         
            }
      
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (door.gameObject.activeInHierarchy)
                door.BeginToClose();
        
        }
    }
}

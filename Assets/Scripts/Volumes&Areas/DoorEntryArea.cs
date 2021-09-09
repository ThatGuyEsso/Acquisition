using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorEntryArea : MonoBehaviour
{
    [SerializeField] private SlidingDoor door;

  
    bool isInRange;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!door) return;
            if (door.gameObject.activeInHierarchy)
            {
                door.BeginToOpen();
                isInRange = true;
            }
      
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player")&& isInRange)
        {
            if (!door) return;
            if (door.gameObject.activeInHierarchy &&!door.GetIsOpening()&&!door.GetIsOpen())
            {
                door.BeginToOpen();
              
            }


        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (door.gameObject.activeInHierarchy && door.GetIsOpening() || door.GetIsOpen())
            {
                door.BeginToClose();

            }


        }
    }
}

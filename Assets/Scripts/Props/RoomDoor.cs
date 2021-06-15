using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDoor : MonoBehaviour
{
    [SerializeField] private SlidingDoor door;
    public Transform corridorSpawn;
    [SerializeField] private bool isLocked;
    [SerializeField] private SpriteRenderer leftGFX, rightGFX;
    [SerializeField] private Sprite leftLockedSprite, leftOpenSprite, rightLockedSprite, rightOpenSprite;
    public void ToggleLock(bool locked)
    {
        isLocked = locked;
        if (locked)
        {
            leftGFX.sprite = leftLockedSprite;
            rightGFX.sprite = rightLockedSprite;
        }
        else
        {
            leftGFX.sprite = leftOpenSprite;
            rightGFX.sprite = rightOpenSprite;
        }
    }
    public void OpenDoors()
    {
        door.BeginToOpen();
    }

    public void CloseDoors()
    {
        door.BeginToClose();
    }

    public void Init()
    {
        door.SetUpDoor();
        ToggleLock(isLocked);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")&&!isLocked) OpenDoors();
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") ) CloseDoors();
    }
}

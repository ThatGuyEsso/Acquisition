using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [SerializeField] private float maxOpenDistance;
    [SerializeField] private float openSpeed;
    [SerializeField] private Transform leftDoor, rightDoor;
    bool isOpening;
    bool isClosing;

    [SerializeField] private SpriteRenderer leftGFX, rightGFX;
    [SerializeField] private Sprite leftLockedSprite, leftOpenSprite,rightLockedSprite, rightOpenSprite;

    Vector2 leftDoorClosedPos, rightDoorClosedPos;
    Vector2 leftDoorOpenPos, rightDoorOpenPos;
    public void SetUpDoor()
    {
        leftDoorClosedPos = leftDoor.transform.position;
        rightDoorClosedPos = rightDoor.transform.position;
        leftDoorOpenPos = leftDoor.transform.position - transform.right * maxOpenDistance;
        rightDoorOpenPos = rightDoor.transform.position + transform.right * maxOpenDistance;
    }



    public void ToggleLock(bool locked)
    {
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

    public void Update()
    {
        if (isOpening)
        {
            rightDoor.transform.position += transform.right * Time.deltaTime * openSpeed;

            leftDoor.transform.position -= transform.right * Time.deltaTime * openSpeed;
            if (rightDoor.position.x >= rightDoorOpenPos.x && leftDoor.position.x <= leftDoorOpenPos.x)
            {
                isOpening = false;
                rightDoor.position = rightDoorOpenPos;
                leftDoor.position = leftDoorOpenPos;
            }
        }else if (isClosing)
        {
            rightDoor.transform.position -= transform.right * Time.deltaTime * openSpeed;

            leftDoor.transform.position += transform.right * Time.deltaTime * openSpeed;
            if (rightDoor.position.x <= rightDoorClosedPos.x && leftDoor.position.x >= leftDoorClosedPos.x)
            {
                isClosing = false;
                rightDoor.position = rightDoorClosedPos;
                leftDoor.position = leftDoorClosedPos;
            }
        }

    }

    public void BeginToOpen()
    {
        isOpening = true;
        isClosing = false;
    }
    public void BeginToClose()
    {
        isOpening = false;
        isClosing = true;
    }
}

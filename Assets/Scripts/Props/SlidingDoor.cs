using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [SerializeField] private float maxOpenDistance;
    [SerializeField] private float openSpeed;
    [SerializeField] private bool isVertical;
    [SerializeField] private bool isReversed;
    [SerializeField] private Transform leftDoor, rightDoor;
    bool isOpening;
    bool isClosing;
    public bool isInteractable =true;



    Vector2 leftDoorClosedPos, rightDoorClosedPos;
    Vector2 leftDoorOpenPos, rightDoorOpenPos;
    public void SetUpDoor()
    {
        leftDoorClosedPos = leftDoor.transform.position;
        rightDoorClosedPos = rightDoor.transform.position;
        leftDoorOpenPos = leftDoor.transform.position - transform.right * maxOpenDistance;
        rightDoorOpenPos = rightDoor.transform.position + transform.right * maxOpenDistance;
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
            if (!isVertical)
            {
                if (rightDoor.position.x <= rightDoorClosedPos.x && leftDoor.position.x >= leftDoorClosedPos.x)
                {
                    isClosing = false;
                    rightDoor.position = rightDoorClosedPos;
                    leftDoor.position = leftDoorClosedPos;
                }
            }
            else
            {
                if (!isReversed)
                {
                    if (rightDoor.position.y <= rightDoorClosedPos.y && leftDoor.position.y >= leftDoorClosedPos.y)
                    {
                        isClosing = false;
                        rightDoor.position = rightDoorClosedPos;
                        leftDoor.position = leftDoorClosedPos;
                    }
                }
                else
                {
                    if (rightDoor.position.y >= rightDoorClosedPos.y && leftDoor.position.y <= leftDoorClosedPos.y)
                    {
                        isClosing = false;
                        rightDoor.position = rightDoorClosedPos;
                        leftDoor.position = leftDoorClosedPos;
                    }
                }
      
            }
   
        }

    }

    public void BeginToOpen()
    {
        if (isInteractable)
        {
            isOpening = true;
            isClosing = false;
            AudioManager.instance.PlayThroughAudioPlayer("DoorOpen", transform.position); //sound to open door
        }
     
    }
    public void BeginToClose()
    {
        if (isInteractable)
        {
            isOpening = false;
            isClosing = true;
            AudioManager.instance.PlayThroughAudioPlayer("DoorClose", transform.position); //sound to close door
        }
    
    }
}

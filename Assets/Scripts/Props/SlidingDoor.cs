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
    bool isOpen;
    public bool isInteractable =true;

    private AudioPlayer slideSoundPlayer;

    [SerializeField] Vector2 leftDoorClosedPos, rightDoorClosedPos;
    [SerializeField] Vector2 leftDoorOpenPos, rightDoorOpenPos;
    public void SetUpDoor()
    {
        leftDoorClosedPos = leftDoor.transform.position;
        rightDoorClosedPos = rightDoor.transform.position;
  
        leftDoorOpenPos = leftDoor.transform.position - transform.right * maxOpenDistance;
        rightDoorOpenPos = rightDoor.transform.position + transform.right * maxOpenDistance;

    }

    virtual protected void OnEnable()
    {
        if (GameManager.instance)
        {
            GameManager.instance.OnNewEvent += EvaluateNewGameEvent;
        }

    }

    virtual protected void EvaluateNewGameEvent(GameEvents newEvent)
    {
        switch (newEvent)
        {

         

            case GameEvents.ExitGame:

                if (slideSoundPlayer) slideSoundPlayer.KillAudio();
                    ObjectPoolManager.Recycle(gameObject);
                break;
        }
    }



    public void Update()
    {
        if (isOpening)
        {
            rightDoor.transform.position += transform.right * Time.deltaTime * openSpeed;

            leftDoor.transform.position -= transform.right * Time.deltaTime * openSpeed;

            if (!isVertical)
            {
                if (rightDoor.position.x >= rightDoorOpenPos.x && leftDoor.position.x <= leftDoorOpenPos.x)
                {
                    isOpening = false;
                    isOpen = true;
                    rightDoor.position = rightDoorOpenPos;
                    leftDoor.position = leftDoorOpenPos;
                    if (slideSoundPlayer) slideSoundPlayer.BeginFadeOut();
                    slideSoundPlayer = null;
                }
            }
            else
            {
                if (!isReversed)
                {
                    if (rightDoor.position.y >= rightDoorOpenPos.y && leftDoor.position.y <= leftDoorOpenPos.y)
                    {
                        isOpening = false;
                        isOpen = true;
                        rightDoor.position = rightDoorOpenPos;
                        leftDoor.position = leftDoorOpenPos;
                        if (slideSoundPlayer) slideSoundPlayer.BeginFadeOut();
                        slideSoundPlayer = null;
                    }
                }
                else
                {
                    if (rightDoor.position.y <= rightDoorOpenPos.y && leftDoor.position.y >= leftDoorOpenPos.y)
                    {
                        isOpening = false;
                        isOpen = true;
                        rightDoor.position = rightDoorOpenPos;
                        leftDoor.position = leftDoorOpenPos;
                        if (slideSoundPlayer) slideSoundPlayer.BeginFadeOut();
                        slideSoundPlayer = null;
                    }
                }
            }
    



        }else if (isClosing)
        {
            isOpen = false;
            rightDoor.transform.position -= transform.right * Time.deltaTime * openSpeed;

            leftDoor.transform.position += transform.right * Time.deltaTime * openSpeed;
            if (!isVertical)
            {
                if (rightDoor.position.x <= rightDoorClosedPos.x && leftDoor.position.x >= leftDoorClosedPos.x)
                {
                    isClosing = false;
                    rightDoor.position = rightDoorClosedPos;
                    leftDoor.position = leftDoorClosedPos;
                    if (slideSoundPlayer) slideSoundPlayer.BeginFadeOut();
                    slideSoundPlayer = null;
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
                        if (slideSoundPlayer) slideSoundPlayer.BeginFadeOut();
                        slideSoundPlayer = null;
                    }
                }
                else
                {
                    if (rightDoor.position.y >= rightDoorClosedPos.y && leftDoor.position.y <= leftDoorClosedPos.y)
                    {
                        isClosing = false;
                        rightDoor.position = rightDoorClosedPos;
                        leftDoor.position = leftDoorClosedPos;
                        if (slideSoundPlayer) slideSoundPlayer.BeginFadeOut();
                        slideSoundPlayer = null;
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
            if(slideSoundPlayer)
            {
                if (!slideSoundPlayer.IsPlaying())
                    slideSoundPlayer = AudioManager.instance.PlayThroughAudioPlayer("DoorSliding", transform.position); //sound to open door
            }
            else
            {
             
                    slideSoundPlayer = AudioManager.instance.PlayThroughAudioPlayer("DoorSliding", transform.position); //sound to open door
            }
          
        }
     
    }
    public void BeginToClose()
    {
        if (isInteractable)
        {
            isOpening = false;
            isClosing = true;
            if (slideSoundPlayer)
            {
                if (!slideSoundPlayer.IsPlaying())
                    slideSoundPlayer = AudioManager.instance.PlayThroughAudioPlayer("DoorSliding", transform.position); //sound to open door
            }
            else
            {

                slideSoundPlayer = AudioManager.instance.PlayThroughAudioPlayer("DoorSliding", transform.position); //sound to open door
            }
        }
    
    }


    private void OnDestroy()
    {
        if (GameManager.instance)
        {
            GameManager.instance.OnNewEvent -= EvaluateNewGameEvent;
        }
    }

    private void OnDisable()
    {
        if (GameManager.instance)
        {
            GameManager.instance.OnNewEvent -= EvaluateNewGameEvent;
        }
    }


    public bool GetIsOpening() {  return isOpening;}
    public bool GetIsOpen() { return isOpen; }
}

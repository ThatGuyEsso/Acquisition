using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccensionRune : MonoBehaviour
{
    private Transform playerTransfrom;

    private Animator animator;
    private float smoothRot;
    private float rotationRate =0.3f;

    private bool accensionComplete;
    public void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator) animator.enabled = false;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (GameManager.instance)
            {
                GameManager.instance.BeginNewEvent(GameEvents.BeginGameComplete);
                playerTransfrom = other.transform;
                if (animator)
                {
                    animator.enabled = true;
                    animator.Play("BeginAccension", 0, 0f);
                }
            }
             
        }
    }


    private void Update()
    {
        if (playerTransfrom&&!accensionComplete)
        {
            FaceCharacterToSelf(playerTransfrom);
        }
    }


    public void FaceCharacterToSelf( Transform characterTrans)
    {
        Vector2 toVCursor = characterTrans.position - transform.position;
        float targetAngle = Mathf.Atan2(toVCursor.y, toVCursor.x) * Mathf.Rad2Deg;//get angle to rotate

        //if (targetAngle < 0) targetAngle += 360f;
        float angle = Mathf.SmoothDampAngle(characterTrans.eulerAngles.z, targetAngle+90f, ref smoothRot, rotationRate);//rotate player smoothly to target angle
        characterTrans.rotation = Quaternion.Euler(0f, 0f, angle);//update angle
    }

    public void OnAccensionComplete()
    {
        accensionComplete = true;
        if (AccensionHubManager.instance) AccensionHubManager.instance.BeginWhiteOut();
    }
    
}

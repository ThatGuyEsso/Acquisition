using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTutorialHeld : AttackTutorial
{
    [SerializeField] private float timeToHold = 2f;

    float currHeldTime;
    bool isHeld;
    bool tutorialComplete;
    public override void CompleteTutorial()
    {
        base.CompleteTutorial();
        inputs.Disable();
        tutorialComplete = true;
    }
    public override void StartTutorial()
    {
        tutorialComplete = false;
        currHeldTime = timeToHold;
        if (isPrimary)
        {
            inputs.Attack.PrimaryAttack.started += _ => OnPressed();
            inputs.Attack.PrimaryAttack.canceled += _ => OnReleased();
        }
        else
        {
            inputs.Attack.SecondaryAttack.started += _ => OnPressed();
            inputs.Attack.SecondaryAttack.canceled += _ => OnReleased();
        }

        inputs.Enable();
    }


    public void OnPressed()
    {
        if (tutorialComplete) return;
        currHeldTime = timeToHold;
        isHeld = true;
    }

    public void OnReleased()
    {
        isHeld = false;
    }


    protected override void Update()
    {
        base.Update();

        if (isHeld)
        {
            if(currHeldTime <= 0f)
            {
                isHeld = false;
                CompleteTutorial();
            }
            else
            {
                currHeldTime-= Time.deltaTime;
            }
        }
    }

}

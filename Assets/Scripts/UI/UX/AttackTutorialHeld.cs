using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTutorialHeld : AttackTutorial
{
    [SerializeField] private float timeToHold = 2f;

    float currHeldTime;
    bool isHeld;
    public override void CompleteTutorial()
    {
        base.CompleteTutorial();
        inputs.Disable();
    }
    public override void StartTutorial()
    {
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

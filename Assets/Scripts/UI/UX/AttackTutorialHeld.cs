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
        if (isPrimary&&inputs!=null)
        {
            inputs.Attack.PrimaryAttack.started -= _ => OnPressed();
            inputs.Attack.PrimaryAttack.canceled -= _ => OnReleased();
        }
        else
        {
            inputs.Attack.SecondaryAttack.started -= _ => OnPressed();
            inputs.Attack.SecondaryAttack.canceled -= _ => OnReleased();

        }

        switch (weaponType)
        {
            case WeaponType.Sword:
                TutorialManager.instance.tutorialdata.isSwordTutorialComplete = true;
                TutorialManager.instance.tutorialdata.EvaluateTutorialComplete();
                SaveData.Current.SaveTutorialData(TutorialManager.instance.tutorialdata);
                SerialisationManager.Save(GameStateManager.instance.saveName, SaveData.Current);
                break;
            case WeaponType.Bow:

                TutorialManager.instance.tutorialdata.isBowTutorialComplete = true;
                TutorialManager.instance.tutorialdata.EvaluateTutorialComplete();
                SaveData.Current.SaveTutorialData(TutorialManager.instance.tutorialdata);
                SerialisationManager.Save(GameStateManager.instance.saveName, SaveData.Current);
                break;
            case WeaponType.Staff:

                TutorialManager.instance.tutorialdata.isStaffTutorialComplete = true;
                TutorialManager.instance.tutorialdata.EvaluateTutorialComplete();
                SaveData.Current.SaveTutorialData(TutorialManager.instance.tutorialdata);
                SerialisationManager.Save(GameStateManager.instance.saveName, SaveData.Current);
                break;

        }
    }
    public override void StartTutorial()
    {

        if (tutorialPrompts[tutorialPrompts.Count - 1])
            tutorialPrompts[tutorialPrompts.Count - 1].OnFadeEnd -= StartTutorial;

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

        if (tutorialComplete) return;
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

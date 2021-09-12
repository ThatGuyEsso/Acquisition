using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTutorial : InputTutorial
{
    public override void StartTutorial()
    {
        base.StartTutorial();
        inputs.Aiming.Aim.performed += _ => CompleteTutorial();
        inputs.MouseActivity.Move.performed += _ => CompleteTutorial();
        inputs.Enable();
    }

    public override void CompleteTutorial()
    {
        base.CompleteTutorial();
        inputs.Disable();
        TutorialManager.instance.tutorialdata.isAimTutorialComplete = true;
        TutorialManager.instance.tutorialdata.EvaluateTutorialComplete();
        SaveData.Current.SaveTutorialData(TutorialManager.instance.tutorialdata);
        SerialisationManager.Save(GameStateManager.instance.saveName, SaveData.Current);
    }

}

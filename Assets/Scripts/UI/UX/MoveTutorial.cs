using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTutorial : InputTutorial
{
    public override void StartTutorial()
    {
        base.StartTutorial();
        inputs.Movement.Move.performed += _ => CompleteTutorial();
        inputs.Enable();
    }

    public override void CompleteTutorial()
    {
        base.CompleteTutorial();
        inputs.Disable();
        TutorialManager.instance.tutorialdata.isMoveTutorialComplete = true;
        TutorialManager.instance.tutorialdata.EvaluateTutorialComplete();
        SaveData.Current.SaveTutorialData(TutorialManager.instance.tutorialdata);
        SerialisationManager.Save(GameStateManager.instance.saveName, SaveData.Current);
    }

}

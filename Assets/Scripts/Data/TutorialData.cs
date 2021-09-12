using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial Data Asset")]
public class TutorialData : ScriptableObject
{
    public bool isTutorialComplete;
    public bool isMoveTutorialComplete;
    public bool isAimTutorialComplete;
    public bool isDodgeTutorialComplete;
    public bool isSwordTutorialComplete;
    public bool isBowTutorialComplete;
    public bool isStaffTutorialComplete;


    public bool EvaluateTutorialComplete()
    {
        isTutorialComplete = isMoveTutorialComplete && isAimTutorialComplete && isAimTutorialComplete
            && isDodgeTutorialComplete && isSwordTutorialComplete && isBowTutorialComplete && isStaffTutorialComplete;
        return isTutorialComplete;
    }


    public void LoadTutorialData()
    {
        isTutorialComplete = SaveData.Current.tutorialComplete;
        isMoveTutorialComplete = SaveData.Current.moveComplete;
        isAimTutorialComplete = SaveData.Current.aimComplete;
        isDodgeTutorialComplete = SaveData.Current.dodgeComplete;
        isSwordTutorialComplete = SaveData.Current.swordComplete;
        isBowTutorialComplete = SaveData.Current.bowComplete;
        isStaffTutorialComplete = SaveData.Current.staffComplete;
    }
}

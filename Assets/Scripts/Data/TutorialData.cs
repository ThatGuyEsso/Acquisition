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
}

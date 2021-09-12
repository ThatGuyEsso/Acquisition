using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData 
{

    public bool tutorialComplete;
    public bool moveComplete;
    public bool aimComplete;
    public bool dodgeComplete;
    public bool swordComplete;
    public bool bowComplete;
    public bool staffComplete;

    public static SaveData _current;


    public static SaveData Current
    {
        get
        {
            if (_current == null)
            {
                _current = new SaveData();

            }
            return _current;
        }

        set
        {
            _current = value;
        }
    }
  


    public void SaveTutorialData(TutorialData data)
    {
        tutorialComplete = data.isTutorialComplete;
        moveComplete = data.isMoveTutorialComplete;
        aimComplete = data.isAimTutorialComplete;
        dodgeComplete=data.isDodgeTutorialComplete;
        swordComplete = data.isSwordTutorialComplete;
        bowComplete = data.isBowTutorialComplete;
        staffComplete = data.isStaffTutorialComplete;

    }


}

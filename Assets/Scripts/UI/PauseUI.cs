using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : Base_UI
{
    public override void InitUI(UIType uiType)
    {
        base.InitUI(uiType);
        
    }

    public void OnResume()
    {
        Debug.Log("Resume");
    }

    public void OnOptions()
    {
        uiManager.SwitchUI(UIType.Settings);
    }

    public void OnMainMenu()
    {
        Debug.Log("MainMenu");
    }
}

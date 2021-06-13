using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : Base_UI
{
    public override void InitUI(UIType uiType)
    {
        base.InitUI(uiType);
        Time.timeScale = 0;
    }

    public override void ExitUI()
    {
        base.ExitUI();
        Time.timeScale = 1;
    }

    public void OnResume()
    {
        uiManager.SwitchUI(UIType.GameUI);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : Base_UI
{
    public void OnResume()
    {
        uiManager.SetGameToPause(false);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : Base_UI
{
    public void OnResume()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        uiManager.SetGameToPause(false);
    }

    public void OnOptions()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        uiManager.SwitchUI(UIType.Settings);
    }

    public void OnMainMenu()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        Debug.Log("MainMenu");
    }

}

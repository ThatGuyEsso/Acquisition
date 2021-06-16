using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsUI : Base_UI
{

    public void OnBackButton()
    {
        uiManager.SwitchUI(previousUI);
    }
}

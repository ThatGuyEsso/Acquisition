using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class SettingsUI : Base_UI
{
 
    [SerializeField] private VideoSettings videoSettings;

    public override void InitUI(UIType uiType, UIType preUI)
    {
        base.InitUI(uiType, preUI);
        videoSettings.GetComponent<IInitialisable>().Init();
    }

    public void OnBack()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        uiManager.SwitchUI(previousUI);
    }



}

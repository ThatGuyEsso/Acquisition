using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsUI : Base_UI
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private TMPro.TMP_Dropdown resolutionDropDown;

    Resolution[] allResolutions;

    public override void InitUI(UIType uiType, UIType preUI)
    {
        base.InitUI(uiType, preUI);

        allResolutions = Screen.resolutions;

        resolutionDropDown.ClearOptions();
        List<string> r = new List<string>();

        int currentRes = 0;
        for (int i = 0; i < allResolutions.Length; i++)
        {
            string option = allResolutions[i].width + " x " + allResolutions[i].height;
            r.Add(option);

            if (allResolutions[i].width == Screen.currentResolution.width && allResolutions[i].height == Screen.currentResolution.height)
            {
                currentRes = i;
            }
        }
        resolutionDropDown.AddOptions(r);
        resolutionDropDown.value = currentRes;
        resolutionDropDown.RefreshShownValue();
    }

    public void SetVolume(float value)
    {
        mixer.SetFloat("MasterVolume", value);
    }

    public void SetWindowMode(int index)
    {
        if(index == 0)
            Screen.fullScreenMode = FullScreenMode.Windowed;
        else if(index == 1)
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        else if(index == 2)
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    public void SetResolution(int resIndex)
    {
        Resolution resolution = allResolutions[resIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void OnBack()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        uiManager.SwitchUI(previousUI);
    }
}

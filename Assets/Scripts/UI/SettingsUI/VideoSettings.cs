using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class VideoSettings : MonoBehaviour, IInitialisable
{
    [SerializeField] private TMPro.TMP_Dropdown resolutionDropDown;

    Resolution[] allResolutions;
    [SerializeField] private GameObject firstSelectedElement;
    public void Init()
    {
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

    public void SetWindowMode(int index)
    {
        if (index == 0)
            Screen.fullScreenMode = FullScreenMode.Windowed;
        else if (index == 1)
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        else if (index == 2)
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
    }

    public void SetResolution(int resIndex)
    {
        Resolution resolution = allResolutions[resIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void OnEnable()
    {
        if (UIManager.instance)
        {
            UIManager.instance.eventSystem.SetSelectedGameObject(firstSelectedElement);
            
        }
    }

}

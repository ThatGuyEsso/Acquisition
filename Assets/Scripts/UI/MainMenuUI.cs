using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    

    public void Play()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        GameStateManager.instance.BeginNewState(GameState.LoadingHubWorld);
        UIManager.instance.SwitchUI(UIType.None);
    }

    public void Credits()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        UIManager.instance.SwitchUI(UIType.Credits);
    }

    public void Settings()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        UIManager.instance.SwitchUI(UIType.Settings);
    }
    public void Quit()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        Application.Quit();
        Debug.Log("Quitting");

            
    }
}

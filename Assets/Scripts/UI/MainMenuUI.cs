using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    private void Awake()
    {
        UIManager.instance.SetMainMenu(gameObject);
    }

    public void Play()
    {
        AudioManager.instance.Play("ButtonPress");
        GameStateManager.instance.BeginNewState(GameState.LoadingHubWorld);
    }

    public void Credits()
    {
        AudioManager.instance.Play("ButtonPress");
        UIManager.instance.SwitchUI(UIType.Credits);
    }

    public void Settings()
    {
        AudioManager.instance.Play("ButtonPress");
        UIManager.instance.SwitchUI(UIType.Settings);
    }
    public void Quit()
    {
        AudioManager.instance.Play("ButtonPress");
        Application.Quit();
        Debug.Log("Quitting");

            
    }
}

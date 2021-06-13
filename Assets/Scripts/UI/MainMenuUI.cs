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
        GameStateManager.instance.BeginNewState(GameState.LoadingHubWorld);
    }

    public void Credits()
    {
        Debug.Log("Credits");
    }

    public void Settings()
    {
        UIManager.instance.SwitchUI(UIType.Settings);
    }
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quitting");

            
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
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

        Debug.Log("Settings");


    }
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quitting");

            
    }
}

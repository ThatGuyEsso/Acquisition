using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccensionHubManager : MonoBehaviour
{
    public static AccensionHubManager instance;

    bool playerEntered;

    private void Awake()
    {

        if (instance == false)
        {
            instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }

  
    }

    public void BeginWhiteOut()
    {
        if (LoadingScreen.instance) LoadingScreen.instance.SetLoadingScreenColour(Color.white);
        if (SceneTransitionManager.instance)SceneTransitionManager.instance.BeginLoadMenuScreen(SceneIndex.MainMenu, UIType.Credits);
        if (MusicManager.instance) MusicManager.instance.BeginSongFadeOut(5f);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")&& !playerEntered)
        {
            if (WeaponManager.instance)
            {
                playerEntered = true;
                WeaponManager.instance.RemoveWeapon();


            }
        }
    }
}

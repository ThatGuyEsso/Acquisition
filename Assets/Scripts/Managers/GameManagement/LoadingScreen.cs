using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LoadingScreen : MonoBehaviour, IInitialisable
{
    public static LoadingScreen instance;
    [Header("Loading Screen Component")]
    [SerializeField] private Image blackScreen;
    [SerializeField] private GameObject loadingCam;
    [Header("Loading Screen Settings")]
    [SerializeField] private float fadeInRate;
    [SerializeField] private float fadeOutRate;

    bool isFadingIn =false;
    bool isFadingOut =false;
    [SerializeField] private float currentAlpha;
    public Action OnFadeComplete;
    public void Init()
    {

        if (instance == false)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }

    
    }


    public void Update()
    {
        if (isFadingIn)
        {
            currentAlpha = Mathf.Lerp(currentAlpha, 1.0f, Time.deltaTime * fadeInRate);

            if (currentAlpha >=0.99)
            {
                currentAlpha = 1.0f;
                blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, currentAlpha);
              
                isFadingIn = false;
                OnFadeComplete?.Invoke();
            }
            else
            {

                blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, currentAlpha);
         
            }

           
        }
        else if (isFadingOut)
        {
            currentAlpha = Mathf.Lerp(currentAlpha, 0.0f, Time.deltaTime * fadeOutRate);

            if (currentAlpha <= 0.1f)
            {
                currentAlpha = 0f;
                blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, currentAlpha);

                isFadingOut = false;
                OnFadeComplete?.Invoke();
                DisableScreen();
            }
            else
            {

                blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, currentAlpha);
    
            }
        }
    }

    public void BeginFadeIn()
    {
        if (!blackScreen.gameObject.activeInHierarchy) blackScreen.gameObject.SetActive(true);
        if (!loadingCam.activeInHierarchy) loadingCam.SetActive(true);
        currentAlpha = 0f;
        blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, currentAlpha);

        isFadingOut = false;
        isFadingIn = true;
    }

    public void BeginFadeOut()
    {
        if (!blackScreen.gameObject.activeInHierarchy) blackScreen.gameObject.SetActive(true);
        if (!loadingCam.activeInHierarchy) loadingCam.SetActive(true);
        currentAlpha = 1.0f;
        isFadingIn = false;
        isFadingOut = true;
    }
    public void DisableScreen()
    {
        if (blackScreen.gameObject.activeInHierarchy) blackScreen.gameObject.SetActive(false);
        if (loadingCam.activeInHierarchy) loadingCam.SetActive(false);
    }

    public bool IsLoadingScreenOn()
    {
        return !isFadingOut && !isFadingIn && blackScreen.gameObject.activeInHierarchy;
    }

    public void SetLoadingScreenColour(Color color)
    {
        blackScreen.color = color;
    }
}

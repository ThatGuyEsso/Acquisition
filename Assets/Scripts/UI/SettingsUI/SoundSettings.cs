using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer musicVolume;
    [SerializeField] private AudioMixer sfxVoulume;
    [SerializeField] private AudioMixer uiVolume;
    [SerializeField] private GameObject firstSelectedElement;
    public void SetMusicVolume(float value)
    {
        musicVolume.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        sfxVoulume.SetFloat("SFXVolume", value);
    }

    public void SetUIVolume(float value)
    {
        uiVolume.SetFloat("UIVolume", value);
        if(value <= -80f)
        {
            if (MusicManager.instance) MusicManager.instance.ToggleMusic(false);
        }
        else
        {
            if (MusicManager.instance) MusicManager.instance.ToggleMusic(true);
        }
    }

    public void OnEnable()
    {
        if (UIManager.instance)
        {
            StartCoroutine(WaitToSelectGameObject());
        }
    }


    public IEnumerator WaitToSelectGameObject()
    {
        if (UIManager.instance)
        {
            UIManager.instance.eventSystem.SetSelectedGameObject(null);
            yield return null;
            UIManager.instance.eventSystem.SetSelectedGameObject(firstSelectedElement);
        }
    }
}

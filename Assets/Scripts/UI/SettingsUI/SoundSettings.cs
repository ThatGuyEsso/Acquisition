using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundType
{
    Music,
    UI,
    SFX
};
public class SoundSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer musicVolume;
    [SerializeField] private AudioMixer sfxVoulume;
    [SerializeField] private AudioMixer uiVolume;
    [SerializeField] private GameObject firstSelectedElement;
    public void SetMusicVolume(float value)
    {
  
        musicVolume.SetFloat("MusicVolume", Mathf.Log10(value) * 20f);
        if (value <= -1e-06)
        {
            if (MusicManager.instance) MusicManager.instance.ToggleMusic(false);
        }
        else
        {
            if (MusicManager.instance) MusicManager.instance.ToggleMusic(true);
        }
    }

    public void SetSFXVolume(float value)
    {

        sfxVoulume.SetFloat("SFXVolume", Mathf.Log10(value) * 20f);
    }

    public void SetUIVolume(float value)
    {
        uiVolume.SetFloat("UIVolume", Mathf.Log10(value) * 20f);
  
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

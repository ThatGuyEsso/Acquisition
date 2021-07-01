using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer musicVolume;
    [SerializeField] private AudioMixer sfxVoulume;
    [SerializeField] private AudioMixer uiVolume;

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
    }
}

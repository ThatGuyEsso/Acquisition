using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class SliderValueToMixer : MonoBehaviour
{
    private Slider slider;
    private SoundSettings soundSettings;
    [SerializeField] private bool isLinear = true;
    [SerializeField] private SoundType soundType;
    [SerializeField] private AudioMixer musicVolume;
    [SerializeField] private AudioMixer sfxVolume;
    [SerializeField] private AudioMixer uiVolume;
    private void Awake()
    {
        slider = GetComponent<Slider>();
        soundSettings = GetComponentInParent<SoundSettings>();
    }


    private void OnEnable()
    {
        if (!slider||!soundSettings) return;
        float vol;
        switch (soundType)
        {
            case SoundType.Music:

                if (!isLinear)
                {
                    musicVolume.GetFloat("MusicVolume", out vol);
                    slider.value = vol;
                }
                else
                {
                    musicVolume.GetFloat("MusicVolume", out vol);

            
                    vol = Mathf.Pow((vol / 20f), 10f);
                    soundSettings.SetMusicVolume(vol);
                    if (vol == 0) slider.value = vol;
                    else slider.value = 1.0f;
                }
                break;
            case SoundType.UI:
                if (!isLinear)
                {
                    uiVolume.GetFloat("UIVolume", out vol);
                    slider.value = vol;
                }
                else
                {
                    uiVolume.GetFloat("UIVolume", out vol);

                    
                    vol = Mathf.Pow((vol / 20f), 10f);
                    soundSettings.SetUIVolume(vol);
                    if (vol == 0) slider.value = vol;
                    else slider.value = 1.0f;
                }
                break;
            case SoundType.SFX:
                if (!isLinear)
                {
                    sfxVolume.GetFloat("SFXVolume", out vol);
                    slider.value = vol;
                }
                else
                {
                    sfxVolume.GetFloat("SFXVolume", out vol);

                    vol = Mathf.Pow((vol / 20f), 10f);
                    soundSettings.SetSFXVolume(vol);
                    if (vol == 0) slider.value = vol;
                    else slider.value = 1.0f;
                }
                break;
        }
    }
}

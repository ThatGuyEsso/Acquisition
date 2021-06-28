using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHooverSFX : MonoBehaviour
{
   
    [SerializeField] private string sfxName;
    [SerializeField] private bool isPitchRandom;
    public void PlayOnHooverSFX()
    {
        if(AudioManager.instance)
             AudioManager.instance.PlayUISound(sfxName, Vector3.zero, isPitchRandom);

    }
}

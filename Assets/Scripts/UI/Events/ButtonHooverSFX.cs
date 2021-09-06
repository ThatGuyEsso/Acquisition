using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHooverSFX : MonoBehaviour
{
   
    [SerializeField] private string sfxName;
    [SerializeField] private bool isPitchRandom;
    public void PlayOnHooverSFX(GameObject uiGameObject)
    {
        if (UIManager.instance)
        {
            if (UIManager.instance.eventSystem.currentSelectedGameObject != uiGameObject)
            {
                if (AudioManager.instance)
                    AudioManager.instance.PlayUISound(sfxName, Vector3.zero, isPitchRandom);
            }
        }
        else
        {
            if (AudioManager.instance)
                AudioManager.instance.PlayUISound(sfxName, Vector3.zero, isPitchRandom);
        }


    }
}

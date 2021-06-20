using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Cinemachine.PostFX;
public class PostProcessingManager : MonoBehaviour
{
    public static PostProcessingManager instance;
    [SerializeField] private VolumeProfile defaultProfile;
    [SerializeField] private VolumeProfile lightHurtProfile;
    [SerializeField] private VolumeProfile mediumHurtProfile;
    [SerializeField] private VolumeProfile maxHurtProfile;

    [SerializeField] private CinemachineVolumeSettings camVolumeSetting;


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
        camVolumeSetting = GetComponent<CinemachineVolumeSettings>();
        camVolumeSetting.m_Profile = defaultProfile;
    }

    public void ReturnToDefault()
    {
        camVolumeSetting.m_Profile = defaultProfile;
    }

    public void ApplyLightDamageProfile()
    {
        camVolumeSetting.m_Profile = lightHurtProfile;
    }

    public void ApplyMidDamageProfile()
    {
        camVolumeSetting.m_Profile = mediumHurtProfile;
    }

    public void ApplyMaxDamageProfile()
    {
        camVolumeSetting.m_Profile = maxHurtProfile;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    private int currrentTrackList;
    [SerializeField] private AudioSource primarySource;
    [SerializeField] private AudioSource secondarySource;

    [SerializeField] private float crossFadeRate;
    [SerializeField] private float fadeAmount;
    [SerializeField] private AudioMixerGroup musicAudioGroup;
    private bool isAwake;

    public void Awake()
    {

        if (instance == false)
        {
            instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        primarySource.clip = null;
        secondarySource.clip = null;
        primarySource.Stop();
        secondarySource.Stop();
        //Initialise variables
        currrentTrackList = 0;

        //Subscribe to intiation manager
   
        isAwake = true;

    }

}





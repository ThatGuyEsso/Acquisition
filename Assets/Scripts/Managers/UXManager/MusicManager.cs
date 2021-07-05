using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour,IInitialisable,IManager
{
    public static MusicManager instance;


    [SerializeField] private AudioSource primarySource;
    [SerializeField] private AudioSource secondarySource;

    [SerializeField] private float crossFadeRate;
    [SerializeField] private float fadeInRate;
    [SerializeField] private float fadeOutRate;

    [SerializeField] private float timeToNextSong;



    [SerializeField] private AudioMixerGroup musicAudioGroup;
    public Sound[] music;
    private float currentTimeToNextSong;
    bool isInitialised;

    public void Init()
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
        BindToGameStateManager();

        //Subscribe to intiation manager
    }

    public void BindToGameStateManager()
    {
        if (GameStateManager.instance)
        {
            isInitialised = true;
            GameStateManager.instance.OnNewGameState += EvaluateGameState;
        }
    }

    public void EvaluateGameState(GameState newState)
    {
        switch (newState)
        {
          
            case GameState.StartGame:
                break;
            case GameState.TitleScreen:
                break;
       
            case GameState.HubWorldLoadComplete:
                break;

            case GameState.CreditScene:
                break;
      
        }
    }


    public Sound GetSong(string name)
    {
        Sound currSong = Array.Find(music, music => music.name == name);
        return currSong;
    }
    public Sound GetSound(Sound sound)
    {
        foreach (Sound currSong in music)
        {
            if (currSong == sound) return currSong;
        }
        return null;
    }


    private void OnDestroy()
    {
        if(isInitialised && GameStateManager.instance)
        {
            GameStateManager.instance.OnNewGameState -= EvaluateGameState;
        }
    }
}





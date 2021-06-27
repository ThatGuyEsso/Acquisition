using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour, IAudio
{
    AudioSource source;
    string currentName;
    Sound currentSound;
    public void Awake()
    {
        source = gameObject.GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    public void SetUpAudioSource(Sound sound)
    {
        currentSound = sound;
        currentName = sound.name;
        source.clip = sound.clip;
        source.volume = sound.volume;
        source.pitch = sound.pitch;
        source.loop = sound.loop;
        source.outputAudioMixerGroup = sound.mixerGroup;
    }

    public void SetUpAudioSource(Sound sound, float pitch)
    {
        currentSound = sound;
        currentName = sound.name;
        source.clip = sound.clip;
        source.volume = sound.volume;
        source.pitch = pitch;
        source.loop = sound.loop;
        source.outputAudioMixerGroup = sound.mixerGroup;
    }


    public void Play()
    {
        StartCoroutine(ListiningToFinish());
    }

    public void KillAudio()
    {
        StopAllCoroutines();
        source.Stop();
        ObjectPoolManager.Recycle(gameObject);
    }

    public void PlayAtRandomPitch()
    {

        float randPitch = AudioManager.instance.GetRandomPitchOfSound(AudioManager.instance.GetSound(currentName));
        source.pitch = randPitch;
        StartCoroutine(ListiningToFinish());
    }

    public void PlaySoundAtRandomPitch()
    {
        float randPitch = Random.Range(currentSound.pitch - currentSound.pitchChange, currentSound.pitch + currentSound.pitchChange);
        source.pitch = randPitch;
        StartCoroutine(ListiningToFinish());
    }


    public string GetName() { return currentName; }

    IEnumerator ListiningToFinish()
    {

        source.Play();

        while (source.isPlaying)
        {
            yield return null;
        }
        if(gameObject)
            ObjectPoolManager.Recycle(gameObject);
    }
}

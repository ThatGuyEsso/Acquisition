using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundVolume : MonoBehaviour
{
    [SerializeField] private string soundName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        AudioManager.instance.PlayThroughAudioPlayer(soundName, transform.position);
    }


}

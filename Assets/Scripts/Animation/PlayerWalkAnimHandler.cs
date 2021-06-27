using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkAnimHandler : MonoBehaviour
{
    public Animator animator;
    public TDInputMovement movement;
    [SerializeField] private GameObject audioPlayerPrefab;
    [SerializeField] private GameObject dustVFX;
    private void Awake()
    {
        movement.OnWalk += OnPlayerWalk;
        movement.OnStop += OnStopWalk;
    }
    public void OnPlayerWalk()
    {
        if (animator.isActiveAndEnabled)
        {
            animator.SetFloat("PlaySpeed", 1f);
            animator.Play("Run");


        }


    }

    public void OnStopWalk()
    {
        if (animator.isActiveAndEnabled)
        {
            animator.SetFloat("PlaySpeed", 0f);
        }
    }

    public void OnStep()
    {
        if (AudioManager.instance &&ObjectPoolManager.instance)
        {
            IAudio aPlayer= ObjectPoolManager.Spawn(audioPlayerPrefab, transform.position, Quaternion.identity).GetComponent<IAudio>();
            if (aPlayer != null)
            {
                aPlayer.SetUpAudioSource(AudioManager.instance.GetSound("PlayerWalk"));
                aPlayer.Play();
            }
        }

        if (dustVFX) ObjectPoolManager.Spawn(dustVFX, transform.position, transform.rotation);
    }
    public void OnDestroy()
    {
        movement.OnWalk += OnPlayerWalk;
        movement.OnStop += OnStopWalk;
    }
}

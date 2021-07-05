using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimEventListener: MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    [SerializeField] private string walkSFX;
    [SerializeField] private GameObject dustVFX;


    private AudioPlayer loopedAudioPlayer;
    public Action OnShowAttackZone;
    public Action OnHideAttackZone;
    public Action OnShootProjectile;
    public Action OnAnimStart;
    public Action OnAnimEnd;
    public Action OnChargeIncrease;
    public Action OnDeathComplete;
    public Action OnPlaySFX;
    public Action OnStopSFX;
    public void OnAnimBegun()
    {
        OnAnimStart?.Invoke();
    }
    public void OnDie()
    {
        OnDeathComplete?.Invoke();
       
    }
    public void SpawnVFX(GameObject vfx)
    {
        if (ObjectPoolManager.instance) ObjectPoolManager.Spawn(vfx, transform.position, Quaternion.identity);

    }
    public void OnBeginSFX()
    {
        OnPlaySFX?.Invoke();
    }
    public void OnEndSFX()
    {
        OnStopSFX?.Invoke();
    }
    public void OnAnimFinished()
    {
        OnAnimEnd?.Invoke();
    }
    public void OnSpawnAttackZone()
    {
        OnShowAttackZone?.Invoke();

    }

    public void OnDespawnAttackZone()
    {
        OnHideAttackZone?.Invoke();

    }
    public void OnArrowChargeIncrement()
    {
        OnChargeIncrease?.Invoke();

    }


    public void OnSpawnProjectile()
    {
        OnShootProjectile?.Invoke();
    }

    public void PlayWalkSFX()
    {
        if (AudioManager.instance)
            AudioManager.instance.PlayGroupThroughAudioPlayer(walkSFX, transform.position, true);
    }

    public void CreateDustCFX()
    {
        if (dustVFX)
            ObjectPoolManager.Spawn(dustVFX, transform.position, transform.rotation);
    }


    public void PlayLoopedSFX(string sfxName)
    {
        if(sfxName != string.Empty)
        {
            loopedAudioPlayer = AudioManager.instance.PlayThroughAudioPlayer(sfxName, transform.position);
        }
    }

    public void PlaySFX(string sfxName)
    {
        if (sfxName != string.Empty)
        {
            AudioManager.instance.PlayThroughAudioPlayer(sfxName, transform.position);
        }
    }
    public void FadeOutLooped()
    {
        if (loopedAudioPlayer) loopedAudioPlayer.BeginFadeOut();
        loopedAudioPlayer = null;
    }
}

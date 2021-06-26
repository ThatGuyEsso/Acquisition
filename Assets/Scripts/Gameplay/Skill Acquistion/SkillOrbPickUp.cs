using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SkillOrbPickUp : MonoBehaviour
{
    [SerializeField] private BossType skillOrbType;

    [SerializeField] private Base_SkillAttribute skillAttributePrefab;
    [SerializeField] private GameObject displayVFXPrefab;
    [SerializeField] private float fadeOutRate;
    [SerializeField] private string hideSFX;
    [SerializeField] private string displaySFX;

    [SerializeField] private GameObject gfx;
    [SerializeField] private FadeOutSprite fadeControl;


    bool isFading;
    private Collider2D detectCollider;
    public System.Action<SkillOrbPickUp> OnSkillSelect;
    private void Awake()
    {
        fadeControl = GetComponent<FadeOutSprite>();
        if(!detectCollider)
        detectCollider = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")){
            if (WeaponManager.instance) WeaponManager.instance.AddWeaponSkill(skillAttributePrefab);
            if (CamShake.instance)
            {
                CamShake.instance.DoScreenShake(0.5f, 2f, 0.1f, 0.25f, 2f);
            }
           if(displayVFXPrefab) ObjectPoolManager.Spawn(displayVFXPrefab, transform.position, transform.rotation);
            OnSkillSelect?.Invoke(this);
            DestroyPickUp();
        }
    }



    public void EnablePickUp()
    {
        if (detectCollider) detectCollider.enabled =true ;

    
        if (displaySFX != string.Empty) AudioManager.instance.PlayThroughAudioPlayer(displaySFX, transform.position);
        gfx.SetActive(true);
        fadeControl.ShowSprite();
   
    }

    public void DisablePickUp()
    {
  
        if (hideSFX != string.Empty) AudioManager.instance.PlayThroughAudioPlayer(hideSFX, transform.position);
        if (detectCollider) detectCollider.enabled = false;

    }

    public void DisplayOrb()
    {
        EnablePickUp();
        fadeControl.ShowSprite();
        if (displayVFXPrefab) ObjectPoolManager.Spawn(displayVFXPrefab, transform.position, transform.rotation);

    }
    public void DisableLight()
    {
     
        if(fadeControl)
            fadeControl.OnFadeComplete -= DisableLight;
    }
    public void OnFadeFinished()
    {
        if(fadeControl)
            fadeControl.OnFadeComplete -= OnFadeFinished;
        isFading = false;
    }
    public void BeginToHidePickUp()
    {
        DisablePickUp();
        fadeControl.OnFadeComplete += DisableLight;
        fadeControl.BeginFadeOut(fadeOutRate);
    }

    public void BeginToDestroy()
    {
        DisablePickUp();
        StartCoroutine(WaitTillHiddenToDestroy());
    }
    public IEnumerator WaitTillHiddenToDestroy()
    {
        isFading = true;

        fadeControl.OnFadeComplete += OnFadeFinished;
        fadeControl.BeginFadeOut(fadeOutRate);
        while (isFading)
        {
            yield return null;
        }

        DestroyPickUp();
    }
    public void DestroyPickUp()
    {
       
        if (gameObject)
            ObjectPoolManager.Recycle(gameObject);
    }


    
}

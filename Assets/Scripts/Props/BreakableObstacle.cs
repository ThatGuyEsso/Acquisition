using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObstacle : MonoBehaviour,IDamage
{
    [Header("Obstacle Settings")]
    [SerializeField] private int hitPoints;
    [SerializeField] private float hurtTIme;

    [Header("UX FeedBack")]
    [SerializeField] private string hitSFX;
    [SerializeField] private GameObject hitVFX;
    [SerializeField] private GameObject breakVFX;
    private SpriteFlash flashVFX;


    private float currHurtTime;
    private int currentHitPoints;
    bool isHurt;


    public void Awake()
    {
        currentHitPoints = hitPoints;

        flashVFX = GetComponent<SpriteFlash>();
        if (flashVFX) flashVFX.Init();
    }
    public void OnDamage(float dmg, Vector2 kBackDir, float kBackMag, GameObject attacker)
    {
        if (!isHurt)
        {

            currHurtTime = hurtTIme;
            isHurt = true;
            if (flashVFX) flashVFX.Flash();

            if (AudioManager.instance&&hitSFX!= string.Empty) AudioManager.instance.PlayThroughAudioPlayer(hitSFX, transform.position, true);

            currentHitPoints--;
            if (currentHitPoints <= 0)
            {
                if(breakVFX)   ObjectPoolManager.Spawn(breakVFX, transform.position, transform.rotation);
                ObjectPoolManager.Recycle(gameObject);

            }
            else
            {
                if (hitVFX) ObjectPoolManager.Spawn(hitVFX, transform.position, transform.rotation);
            }

        }
    }



    private void Update()
    {
        if (isHurt)
        {
            if(currHurtTime<= 0f)
            {
                isHurt = false;
                if (flashVFX) flashVFX.EndFlash();
            }
            else
            {
                currHurtTime -= Time.deltaTime;
            }
        }
    }
}

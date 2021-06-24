using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SkillOrbPickUp : MonoBehaviour
{
    [SerializeField] private BossType skillOrbType;

    [SerializeField] private Base_SkillAttribute skillAttributePrefab;


    [SerializeField] private string hideSFX;
    [SerializeField] private string displaySFX;
    [SerializeField] private GameObject displayVFX;
    [SerializeField] private GameObject gfx;

    private Collider2D detectCollider;
    public System.Action OnSkillSelect;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")){
            if (WeaponManager.instance) WeaponManager.instance.AddWeaponSkill(skillAttributePrefab);
            DestroyPickUp();
        }
    }



    public void DisplayPickUp()
    {
        if (detectCollider) detectCollider.enabled =true ;

        if (displayVFX) ObjectPoolManager.Spawn(displayVFX, transform.position, Quaternion.identity);
        if (displaySFX != string.Empty) AudioManager.instance.PlayThroughAudioPlayer(displaySFX, transform.position);
        gfx.SetActive(true);
    }

    public void HidePickUp()
    {
        if (displayVFX) ObjectPoolManager.Spawn(displayVFX, transform.position, Quaternion.identity);
        if (hideSFX != string.Empty) AudioManager.instance.PlayThroughAudioPlayer(hideSFX, transform.position);
        if (detectCollider) detectCollider.enabled = false;
        gfx.SetActive(false);
    }
    public void DestroyPickUp()
    {
        HidePickUp();
        if (gameObject)
            ObjectPoolManager.Recycle(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class WeaponSpawner : MonoBehaviour
{
  
    [SerializeField] private SpriteRenderer pickupGFX;
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private GameObject spawnVFX;

    [SerializeField] private Animator deathAnimator;
    private Base_Weapon weapon;
    [Header("Sprites")]
    [SerializeField] private Sprite swordSprite, bowSprite,staffSprite;
    WeaponType type;
    bool isWeaponAvailable;
    public Action<WeaponType> OnWeaponReplaced;
    [SerializeField] private bool inDebug;

    public bool isInteractable=false;
    private void Awake()
    {
        pickupGFX.gameObject.SetActive(false);
        weapon = ObjectPoolManager.Spawn(weaponPrefab.gameObject, Vector3.zero, Quaternion.identity).GetComponent<Base_Weapon>();
        type = weapon.GetComponent<Equipable>().GetWeaponType();
        if (inDebug)
        {
            SpawnWeapon();
            isInteractable = true;
        }
        if (deathAnimator)
            if (deathAnimator.gameObject.activeInHierarchy) deathAnimator.gameObject.SetActive(false);

      
    }

    public void ToggleWeaponAvailable(bool isAvailable)
    {
        isWeaponAvailable = isAvailable;
        pickupGFX.gameObject.SetActive(isAvailable);
        if (!weapon&&isAvailable) 
        {
            weapon = ObjectPoolManager.Spawn(weaponPrefab.gameObject, Vector3.zero, Quaternion.identity).GetComponent<Base_Weapon>();
       
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isWeaponAvailable && other.CompareTag("Player")&&isInteractable){
            if(WeaponManager.instance.equippedWeapon != null)
            {
                if (WeaponManager.instance.equippedWeapon.GetWeaponType() != type)
                {
                    if (AudioManager.instance)
                        AudioManager.instance.PlayThroughAudioPlayer("WeaponPickUp", transform.position);
                    OnWeaponReplaced?.Invoke(WeaponManager.instance.equippedWeapon.GetWeaponType());
                    WeaponManager.instance.EquipWeapon(weapon);
                    ToggleWeaponAvailable(false);
                    if (GameManager.instance)
                        GameManager.instance.BeginNewEvent(GameEvents.WeaponPicked);
                }
            }
            else
            {
                if (AudioManager.instance)
                    AudioManager.instance.PlayThroughAudioPlayer("WeaponPickUp", transform.position);
                WeaponManager.instance.EquipWeapon(weapon);
                ToggleWeaponAvailable(false);
                if(GameManager.instance)
                    GameManager.instance.BeginNewEvent(GameEvents.WeaponPicked);
            }
       
        }
    }
    public void SpawnWeapon()
    {
    
        pickupGFX.gameObject.SetActive(true);
        isWeaponAvailable = true;
        ObjectPoolManager.Spawn(spawnVFX, pickupGFX.transform.position, Quaternion.identity);
        if(AudioManager.instance)
            AudioManager.instance.PlayThroughAudioPlayer("ItemBoom", transform.position); //Plays audio when weapon is spawned
        switch (type)
        {
            case WeaponType.Sword:
                pickupGFX.sprite = swordSprite;
                break;
            case WeaponType.Bow:
                pickupGFX.sprite = bowSprite;
                break;
            case WeaponType.Staff:
                pickupGFX.sprite = staffSprite;
                break;

            case WeaponType.none:
                pickupGFX.sprite = null;
                break;
        }

        if (CamShake.instance)
        {
            CamShake.instance.DoScreenShake(0.15f, 2f, 0.1f, 2f, 2f);
        }
    }

    public WeaponType GetWeaponType() { return type; }


    private void OnDestroy()
    {
        if (WeaponManager.instance&&WeaponManager.instance.equippedWeapon!=null)
        {
            if(weapon.GetType() != WeaponManager.instance.equippedWeapon.GetType())
            {
                if (weapon)
                    ObjectPoolManager.Recycle(weapon.gameObject);
            }
        }
        else
        {
            if (weapon)
                ObjectPoolManager.Recycle(weapon.gameObject);
        }

    }
}

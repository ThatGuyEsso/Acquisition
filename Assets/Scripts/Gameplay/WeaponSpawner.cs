using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class WeaponSpawner : MonoBehaviour
{
  
    [SerializeField] private SpriteRenderer pickupGFX;
    [SerializeField] private GameObject weaponPrefab;
    private Base_Weapon weapon;
    [Header("Sprites")]
    [SerializeField] private Sprite swordSprite, bowSprite,staffSprite;
    WeaponType type;
    bool isWeaponAvailable;
    public Action<WeaponType> OnWeaponReplaced;

    private void Awake()
    {
        pickupGFX.gameObject.SetActive(false);
        weapon = ObjectPoolManager.Spawn(weaponPrefab.gameObject, Vector3.zero, Quaternion.identity).GetComponent<Base_Weapon>();
        type = weapon.GetComponent<Equipable>().GetWeaponType();

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
        if (isWeaponAvailable && other.CompareTag("Player")){
            if(WeaponManager.instance.equippedWeapon != null)
            {
                if (WeaponManager.instance.equippedWeapon.GetWeaponType() != type)
                {
                    
                    OnWeaponReplaced?.Invoke(WeaponManager.instance.equippedWeapon.GetWeaponType());
                    WeaponManager.instance.EquipWeapon(weapon);
                    ToggleWeaponAvailable(false);
                    GameManager.instance.BeginNewEvent(GameEvents.WeaponPicked);
                }
            }
            else
            {
                WeaponManager.instance.EquipWeapon(weapon);
                ToggleWeaponAvailable(false);
                GameManager.instance.BeginNewEvent(GameEvents.WeaponPicked);
            }
       
        }
    }
    public void SpawnWeapon()
    {
    
        pickupGFX.gameObject.SetActive(true);
        isWeaponAvailable = true;
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
    }

    public WeaponType GetWeaponType() { return type; }
}
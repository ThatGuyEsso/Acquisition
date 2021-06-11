using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[RequireComponent(typeof(CircleCollider2D))]
public class WeaponManager : MonoBehaviour,IInitialisable
{
    [SerializeField] private List<WeaponPoints> points;
    
    private Dictionary<WeaponType, WeaponSlots> weaponSlots;

    public Action<WeaponType> OnWeaponEquipped;

    public void Init()
    {
        weaponSlots = new Dictionary<WeaponType, WeaponSlots>();

        foreach(WeaponPoints p in points)
        {
            weaponSlots.Add(p.weaponSlotType, p.slot);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Equipable weapon = other.GetComponentInParent<Equipable>();

        if (weapon != null)
        {
            WeaponSlots slot = weaponSlots[weapon.GetWeaponType()];

            //weapon.SetSlot(slot);
            OnWeaponEquipped?.Invoke(weapon.GetWeaponType());
        }

    }

    public void EvaluateWeaponEquipped(WeaponType weapon)
    {
        switch (weapon)
        {
            case WeaponType.none:
               
                break;
            case WeaponType.Sword:

                break;
            case WeaponType.Bow:

                break;
            case WeaponType.Staff:

                break;
        }
    }

}

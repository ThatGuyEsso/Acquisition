using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class WeaponManager : MonoBehaviour
{
    [SerializeField] private List<WeaponPoints> points;
    
    private Dictionary<string, WeaponSlots> weaponSlots;

    private void Awake()
    {
        weaponSlots = new Dictionary<string, WeaponSlots>();

        foreach(WeaponPoints p in points)
        {
            weaponSlots.Add(p.weaponNameSlot, p.slot);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Equipable weapon = other.GetComponentInParent<Equipable>();

        if (weapon != null)
        {
            WeaponSlots slot = weaponSlots[weapon.GetWeaponName()];

            weapon.SetSlot(slot);
        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[RequireComponent(typeof(CircleCollider2D))]
public class WeaponManager : MonoBehaviour,IInitialisable
{
    [SerializeField] private List<WeaponPoints> points;
    [SerializeField] private AttackAnimEventListener animEventListener;
    [SerializeField] private Transform swordFP,staffFP,bowFP,playerTransform;
    [SerializeField] protected TopPlayerGFXSolver animationSolver;
    private bool isInitialised;
    public Action<WeaponType> OnWeaponEquipped;

    public Equipable equippedWeapon;

    public void Init()
    {
        isInitialised = true;
        //
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isInitialised)
        {
            Equipable weapon = other.GetComponentInParent<Equipable>();

            if (weapon != null)
            {


                EvaluateWeaponEquipped(weapon);
                OnWeaponEquipped?.Invoke(weapon.GetWeaponType());
            }

        }

    }

    public void EvaluateWeaponEquipped(Equipable weapon)
    {
        switch (weapon.GetWeaponType())
        {
            case WeaponType.none:
                if (equippedWeapon != null)
                        equippedWeapon.UnEquip();
                    break;
            case WeaponType.Sword:
                if (equippedWeapon != null)
                {
                    equippedWeapon.UnEquip();
                    equippedWeapon = weapon;
                    equippedWeapon.Equip(swordFP, animEventListener, playerTransform, animationSolver);
                }
                else
                {
                    equippedWeapon = weapon;
                    equippedWeapon.Equip(swordFP, animEventListener, playerTransform, animationSolver);
                }
        

              
                break;
            case WeaponType.Bow:
                if (equippedWeapon != null)
                {
                    equippedWeapon.UnEquip();
                    equippedWeapon = weapon;
                    equippedWeapon.Equip(bowFP, animEventListener, playerTransform, animationSolver);
                }
                else
                {
                    equippedWeapon = weapon;
                    equippedWeapon.Equip(bowFP, animEventListener, playerTransform, animationSolver);
                }

                break;
            case WeaponType.Staff:
                if (equippedWeapon != null)
                {
                    equippedWeapon.UnEquip();
                    equippedWeapon = weapon;
                    equippedWeapon.Equip(staffFP, animEventListener, playerTransform, animationSolver);
                }
                else
                {
                    equippedWeapon = weapon;
                    equippedWeapon.Equip(staffFP, animEventListener, playerTransform, animationSolver);
                }
                break;
        }
    }

}

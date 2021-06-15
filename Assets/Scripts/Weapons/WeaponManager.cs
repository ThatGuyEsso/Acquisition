using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponManager : MonoBehaviour,IInitialisable
{

    public static WeaponManager instance;
    [SerializeField] private List<WeaponPoints> points;
    [SerializeField] private AttackAnimEventListener animEventListener;
    [SerializeField] private Transform swordFP,staffFP,bowFP,playerTransform;
    [SerializeField] protected TopPlayerGFXSolver animationSolver;
    [SerializeField] protected RunTimeData runTimeData;
    private bool isInitialised;
    public Action<WeaponType> OnWeaponEquipped;

    public Equipable equippedWeapon;

    

    public void Init()
    {
        if (instance == false)
        {
            instance = this;
            isInitialised = true;

        }
        else
        {
            Destroy(gameObject);
        }
 
        //
    }

    public void EquipWeapon(Base_Weapon newWeapon)
    {
        if (isInitialised)
        {
            Equipable weapon = newWeapon.GetComponent<Equipable>();

            if (weapon != null)
            {


                EvaluateWeaponEquipped(weapon);
                OnWeaponEquipped?.Invoke(weapon.GetWeaponType());
            }

        }

    }

    public void EvaluateWeaponEquipped(Equipable weapon)
    {
        runTimeData.equippedWeapon = weapon.GetWeaponType();
        switch (weapon.GetWeaponType())
        {
            case WeaponType.none:
                if (equippedWeapon != null)
                        equippedWeapon.UnEquip();
                runTimeData.hasWeapon = false;
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

                runTimeData.hasWeapon = true;


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
                runTimeData.hasWeapon = true;
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
                runTimeData.hasWeapon = true;
                break;


         
        }
    }

    public void RemoveWeapon()
    {
        if (equippedWeapon != null)
            equippedWeapon.UnEquip();
        runTimeData.hasWeapon = false;
        OnWeaponEquipped?.Invoke(WeaponType.none);
    }

}

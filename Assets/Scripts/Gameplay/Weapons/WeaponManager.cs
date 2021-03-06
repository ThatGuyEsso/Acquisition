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
        if (equippedWeapon == null) return;
        if (equippedWeapon != null)
            equippedWeapon.Delete();
        runTimeData.hasWeapon = false;
        OnWeaponEquipped?.Invoke(WeaponType.none);
    }


    public void AddWeaponSkill(Base_SkillAttribute attribute)
    {
        if (equippedWeapon != null)
        {
            equippedWeapon.AddSkillAttribute(attribute);
        }
  

    }




    public void DestroyWeapon()
    {
        if (equippedWeapon != null)
            equippedWeapon.UnEquip();
        runTimeData.hasWeapon = false;
        equippedWeapon = null;
        OnWeaponEquipped?.Invoke(WeaponType.none);
    }

    public void ToggleWeapon(bool isOn)
    {
        if (equippedWeapon != null)
        {

            if (isOn) equippedWeapon.EnableWeapon();
            else equippedWeapon.DisableWeapon();
        }
        else
        {
            Debug.Log("Couldn't reactivate weapons");
        }
    }

}

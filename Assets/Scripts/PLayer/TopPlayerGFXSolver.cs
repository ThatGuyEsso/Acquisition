using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class TopPlayerGFXSolver : MonoBehaviour, IInitialisable
{
    [Header("Animation Controllers")]
    [SerializeField] private RuntimeAnimatorController swordController, noWeaponController,bowController, staffController;
    [Header("Components")]
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private Animator animator;

    bool isInitialised;
    public void Init()
    {
        if (weaponManager)
        {
            weaponManager.OnWeaponEquipped += EvalauteWeaponEquipped;
            isInitialised =true;
        }
    }


    public void EvalauteWeaponEquipped(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Sword:
                if (swordController)
                {
                    animator.runtimeAnimatorController = swordController;
                    animator.Play("Idle_Sword");
                }
                break;
            case WeaponType.Bow:
                if (bowController)
                {
                    animator.runtimeAnimatorController = bowController;
                    animator.Play("Idle_Bow");
                }
                break;
            case WeaponType.Staff:

                if (staffController)
                {
                    animator.runtimeAnimatorController = staffController;
                    animator.Play("Idle_Staff");
                }
                break;
            case WeaponType.none:
                if (noWeaponController)
                {
                    animator.runtimeAnimatorController = noWeaponController;
                    animator.Play("Idle");
                }
                break;
        }
    }

    public void OnDestroy()
    {
        if(isInitialised)
            weaponManager.OnWeaponEquipped -= EvalauteWeaponEquipped;
      
    }
}
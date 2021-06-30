using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Runtime Data Asset")]
public class RunTimeData : ScriptableObject
{
    public bool isKnightDefeated, isElderDefeated, isScholarDefeated;
    public bool hasWeapon;
    public WeaponType equippedWeapon;


    public void ResetData()
    {
        isKnightDefeated = false;
        isElderDefeated = false;
        isScholarDefeated = false;
        hasWeapon = false;
        equippedWeapon = WeaponType.none;
    }



    public bool IsGameClear()
    {
        return isKnightDefeated && isElderDefeated && isScholarDefeated;
    }
}

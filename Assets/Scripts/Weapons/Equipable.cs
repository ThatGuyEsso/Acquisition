using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Equipable 
{
    string GetWeaponName();
    Base_Weapon GetWeapon();
    void SetSlot(WeaponSlots slot);



}

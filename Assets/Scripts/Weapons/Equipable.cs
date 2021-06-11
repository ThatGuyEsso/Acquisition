using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Equipable 
{
    WeaponType GetWeaponType();
    Base_Weapon GetWeapon();
    void SetSlot(WeaponSlots slot);

    void Equip(Transform firePoint,AttackAnimEventListener eventListener,Transform player, TopPlayerGFXSolver solver);

    void UnEquip();


}

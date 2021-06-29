using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Equipable 
{
    WeaponType GetWeaponType();
    Base_Weapon GetWeapon();
    void SetSlot(WeaponSlots slot);
    void DisableWeapon();
    void EnableWeapon();
    void Equip(Transform firePoint,AttackAnimEventListener eventListener,Transform player, TopPlayerGFXSolver solver);


    void AddSkillAttribute(Base_SkillAttribute attribute);
    void UnEquip();
    void Delete();

}

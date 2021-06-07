using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewWeapon", menuName = "Weapon/Create Weapon", order = 0)]
public class WeaponSettings : ScriptableObject
{
    public Base_Projectile primaryProjectile;
    public Base_Projectile secondaryProjectile;
    public float timeBetweenAttacks = 0.5f;
    public float primaryAttackTime = 1.0f;
    public float secondaryAttackTime = 1.0f;

}

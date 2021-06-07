using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectile", menuName = "Weapon/Create Projectile", order = 1)]
public class ProjectileSettings : ScriptableObject
{
    public Sprite ProjectilerSprite;
    public float damagerPerProjectile = 1.0f;
    public float projectileSpeed = 10.0f;
    public float projectileSlowing = 0.5f;

}

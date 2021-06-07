using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow_Weapon : Base_Weapon
{
    protected override void PrimaryAttack()
    {
        Instantiate(Settings.primaryProjectile.gameObject);
        Settings.primaryProjectile.Init();
    }

    protected override void SecondaryAttack()
    {
        
    }
}

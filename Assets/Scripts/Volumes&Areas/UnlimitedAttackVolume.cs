using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlimitedAttackVolume : AttackVolume
{

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (other.gameObject.CompareTag("Projectiles"))
        {

            IProjectile proj = other.GetComponent<IProjectile>();
            if (proj != null)
            {
                if(proj.GetOwner()!=owner|| !proj.GetOwner())
                proj.BreakProjectile();
                if (CamShake.instance)
                {
                    CamShake.instance.DoScreenShake(0.25f, 2f, 0f, 0.15f, 2f);
                }
            }
        }
    }


    protected override void OnTriggerStay2D(Collider2D other)
    {
        base.OnTriggerStay2D(other);

        if (other.gameObject.CompareTag("Projectiles"))
        {

            IProjectile proj = other.GetComponent<IProjectile>();
            if (proj != null)
            {
                if (proj.GetOwner() != owner || !proj.GetOwner())
                    proj.BreakProjectile();
                if (CamShake.instance)
                {
                    CamShake.instance.DoScreenShake(0.25f, 2f, 0f, 0.15f, 2f);
                }
            }
        }
    }
}

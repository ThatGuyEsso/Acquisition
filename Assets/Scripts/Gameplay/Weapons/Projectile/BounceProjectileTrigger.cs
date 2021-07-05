using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceProjectileTrigger : MonoBehaviour
{
    private BouncingProjectile parentProjectile;


    [SerializeField] private LayerMask bounceLayeyrs;
    private void Awake()
    {
        parentProjectile = GetComponentInParent<BouncingProjectile>();

        
    }


    private void OnCollisionEnter2D(Collision2D other)
    {

        if (((1 << other.gameObject.layer) & bounceLayeyrs) != 0)
        {

            if (!parentProjectile) return;
            if (other.contactCount > 0)
                parentProjectile.DoBounce(other.GetContact(0));
            else
                parentProjectile.BreakProjectile();


        }
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProximityDetector : MonoBehaviour
{
    public GameObject parent;
    public GameObject owner;




    public void OnTriggerEnter2D(Collider2D other)
    {
        if (owner)
        {
            if (other.gameObject.CompareTag("Player"))
            {

                if (other.gameObject != owner && owner != null)
                {

                    IProjectile projectile = parent.GetComponent<IProjectile>();
                    if(projectile != null)
                    {
                        projectile.SetProximityHomingTarget(other.transform);
                    }

                }
            }
            else if (other.gameObject.CompareTag("Enemy"))
            {
                if (other.gameObject != owner && owner != null)
                {

                    IProjectile projectile = parent.GetComponent<IProjectile>();
                    if (projectile != null)
                    {
                        projectile.SetProximityHomingTarget(other.transform);
                    }

                }
            }
        }
    }


    public void Update()
    {
        if (parent)
        {
            transform.position = parent.transform.position;
        }
    }
}

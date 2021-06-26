using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFragmentSpawner : MonoBehaviour
{
    private GameObject fragmentPrefab;
    private int fragmentCount;
    private float fragLifeTime;
    private float fragmentSpeed;
    private float angle;
    private bool spawnOnBreak;

    private GameObject fragmentsOwner;
    public void SetUpFragments(GameObject prefab, int count, float speed, float lifeTime, float archAngle,bool spawnOnBreak, GameObject owner)
    {
        fragmentsOwner = owner;
        this.spawnOnBreak = spawnOnBreak;
        angle = archAngle;
        fragmentCount = count;
        fragmentSpeed = speed;
        fragmentPrefab = prefab;
        fragLifeTime = lifeTime;
    }

    private void OnDisable()
    {
        if (spawnOnBreak)
        {
            SpawnProjectileFragment(transform.position);
        }


        Destroy(this);
    }


    public void SpawnProjectileFragment(Vector2 origin)
    {
        float angleIncrement = angle / fragmentCount;
        float currentAngle = 0f;
        GameObject currentFragment;
        for (int i = 0; i < fragmentCount; i++)
        {
            currentFragment = ObjectPoolManager.Spawn(fragmentPrefab, origin, Quaternion.identity);
            IProjectile projFrag = currentFragment.GetComponent<IProjectile>();
            if (projFrag != null)
            {
                
                Vector2 dir = EssoUtility.GetVectorFromAngle(currentAngle).normalized;
                projFrag.SetOwner(fragmentsOwner);
                projFrag.ShootProjectile(fragmentSpeed, dir, fragLifeTime);


               
            }
            else
            {
                if (currentFragment)
                    ObjectPoolManager.Recycle(currentFragment);
            }

            currentAngle += angleIncrement;
        }
    }

    public void SpawnProjectileFragment(Vector2 origin,float posOffset)
    {
        float angleIncrement = angle / fragmentCount;
        float currentAngle = 0f;
        GameObject currentFragment;
        for (int i = 0; i < fragmentCount; i++)
        {
            currentFragment = ObjectPoolManager.Spawn(fragmentPrefab, origin, Quaternion.identity);
            IProjectile projFrag = currentFragment.GetComponent<IProjectile>();
            if (projFrag != null)
            {

                Vector2 dir = EssoUtility.GetVectorFromAngle(currentAngle).normalized;
                if (posOffset != 0f)
                {
                    currentFragment.transform.position = origin + dir * posOffset;
                }
                projFrag.SetOwner(fragmentsOwner);
                projFrag.ShootProjectile(fragmentSpeed, dir, fragLifeTime);



            }
            else
            {
                if (currentFragment)
                    ObjectPoolManager.Recycle(currentFragment);
            }

            currentAngle += angleIncrement;
        }
    }
}

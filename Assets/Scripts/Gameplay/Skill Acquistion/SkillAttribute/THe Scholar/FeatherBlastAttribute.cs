using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatherBlastAttribute : Base_SkillAttribute
{

    private GameObject playerObject;

    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private int featherCount;
    [SerializeField] private float  projectileSpeed;
    [SerializeField] private float projectileLifeTime;

    private DodgeRoll dodgeroll;
    public override void SetUpAttribute(Base_Weapon weaponOwner)
    {
        base.SetUpAttribute(weaponOwner);
        playerObject = weaponOwner.GetPlayerTransform().gameObject;

        if (playerObject)
        {
            dodgeroll = playerObject.GetComponent<DodgeRoll>();

            if (dodgeroll)
            {

                dodgeroll.OnRollBegun += DoFeatherBlast;
        
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (dodgeroll)
        {
            dodgeroll.OnRollBegun += DoFeatherBlast;
        
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (dodgeroll)
        {
            dodgeroll.OnRollBegun -= DoFeatherBlast;
         
        }
    }
    public void DoFeatherBlast()
    {


        float angleIncrement = 360f / featherCount;
        float currentAngle = 0f;
        GameObject currFeather;

        for (int i = 0; i < featherCount; i++)
        {
            currFeather = ObjectPoolManager.Spawn(projectilePrefab, playerObject.transform.position, Quaternion.identity);
            IProjectile projFrag = currFeather.GetComponent<IProjectile>();
            if (projFrag != null)
            {
                Vector2 dir = EssoUtility.GetVectorFromAngle(currentAngle).normalized;
                currFeather.transform.position += (Vector3)dir * 1.5f;
                projFrag.SetOwner(owner.GetPlayerTransform().gameObject);
                projFrag.ShootProjectile(projectileSpeed, dir, projectileLifeTime);

               
            }
            else
            {
                if (currFeather)
                    ObjectPoolManager.Recycle(currFeather);
            }

            currentAngle += angleIncrement;
        }

    }



}

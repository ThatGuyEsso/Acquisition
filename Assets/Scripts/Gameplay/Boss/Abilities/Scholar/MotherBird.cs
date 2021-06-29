using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherBird : BaseBossAbility
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] protected float projectileLifeTime;
    [SerializeField] protected float projectileSpeed;
    [Header("Ability Settings")]
    [SerializeField] private float minAttackTIme;
    [SerializeField] private float maxAttackTIme;
    [SerializeField] private float projectileFireRate;
    [SerializeField] private int minProjectileCount;
    [SerializeField] private int maxProjectileCount;

    [SerializeField] private float angleIncrementDeviation;
    private float currAttackDuration;


    private float currTimeTillNextAttack;
    private bool isAttacking;
    public void BeginMotherBirdAttack()
    {
        attacksLeft--;

        currAttackDuration = Random.Range(minAttackTIme, maxAttackTIme);
        currTimeTillNextAttack = projectileFireRate;
        isAttacking = true;
        owner.PlayAnimation("DoMotherBird");
    }

    public void DoFeatherBlast()
    {
        int featherCount = Random.Range(minProjectileCount, maxProjectileCount + 1);

        float angleIncrement = 360f / featherCount;
        float currentAngle = 0f;
        GameObject currFeather;

        for (int i = 0; i < featherCount; i++)
        {
            currFeather = ObjectPoolManager.Spawn(projectilePrefab, transform.position, Quaternion.identity);
            IProjectile projFrag = currFeather.GetComponent<IProjectile>();
            if (projFrag != null)
            {
                Vector2 dir = EssoUtility.GetVectorFromAngle(currentAngle+ Random.Range(-angleIncrementDeviation,angleIncrementDeviation)).normalized;
                currFeather.transform.position += (Vector3)dir * 1.5f;
                projFrag.SetOwner(owner.gameObject);
                projFrag.ShootProjectile(projectileSpeed, dir, projectileLifeTime);

                if(owner.GetTarget())
                    projFrag.SetHomingTarget(owner.GetTarget());
              
            }
            else
            {
                if (currFeather)
                    ObjectPoolManager.Recycle(currFeather);
            }

            currentAngle += angleIncrement;
        }

    }
    public void StopAttack()
    {
        isAttacking = false;
        EvaluateRemainingAttacks();
    }
    private void Update()
    {
        if (isAttacking)
        {
            if(currAttackDuration<= 0)
            {
                StopAttack();
            }
            else
            {
                currAttackDuration -= Time.deltaTime;
                

            }

            if(currTimeTillNextAttack<= 0 && isAttacking)
            {
                DoFeatherBlast();
                currTimeTillNextAttack = projectileFireRate;
            }
            else
            {
                currTimeTillNextAttack -= Time.deltaTime;
            }
        }
    }


    public void EvaluateRemainingAttacks()
    {
        owner.PlayAnimation("EndMotherBird");
        if (attacksLeft <= 0)
        {
            eventListener.OnShowAttackZone -= BeginMotherBirdAttack;

            //Debug.Log("NO Attacks left");
         
            owner.CycleToNextAttack();
            StartCoroutine(BeginResetAbility(coolDown));

        }
        else
        {
         
            StartCoroutine(BeginRefreshAttack(attackRate));
        }
    }


    public override void EnableAbility()
    {
        base.EnableAbility();

        if (eventListener)
        {
            eventListener.OnShowAttackZone += BeginMotherBirdAttack;
        }
    }
    public override void DisableAbility()
    {
        base.DisableAbility();
        StopAllCoroutines();
        if (eventListener)
            eventListener.OnShowAttackZone -= BeginMotherBirdAttack;
    }
}

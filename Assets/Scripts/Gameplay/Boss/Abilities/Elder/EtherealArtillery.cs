using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtherealArtillery : BaseBossAbility
{
    [Header("Artillery Settings")]
    [SerializeField] private GameObject artilleryPrefab;
    [SerializeField] private int nMinOfArtillery;
    [SerializeField] private int nMaxOfArtillery;
    [SerializeField] private float minArtillerySpawnRate;
    [SerializeField] private float maxArtillerySpawnRate;

    

    public void BeginArtilleryBarrage()
    {
        attacksLeft--;
       

        int nBarrages = Random.Range(nMinOfArtillery, nMaxOfArtillery + 1);
        StartCoroutine(DoArtilleryBarrage(nBarrages));


    }


    public void EvaluateRemainingAttacks()
    {
        owner.PlayAnimation("EtherealArtillery_End");
        if (attacksLeft <= 0)
        {
            eventListener.OnShowAttackZone -= BeginArtilleryBarrage;
           
            //Debug.Log("NO Attacks left");
            StopAllCoroutines();
            owner.CycleToNextAttack();
            StartCoroutine(BeginResetAbility(coolDown));

        }
        else
        {
            //Debug.Log("¨Prime next attack");
            StopAllCoroutines();
            StartCoroutine(BeginRefreshAttack(attackRate));
        }
    }


    public IEnumerator DoArtilleryBarrage(int nBarrages)
    {
        float spawnRate;
        GameObject barrage;
        Vector3 spawnPoint = Vector3.zero;
        for (int i = 0; i < nBarrages; i++)
        {
            if (i == nBarrages - 1)
            {
                if (owner.GetTarget())
                {
                    spawnPoint = owner.GetTarget().position + (Vector3)Random.insideUnitCircle * 5f;
                    barrage = ObjectPoolManager.Spawn(artilleryPrefab, spawnPoint, Quaternion.identity);
                    ArtilleryZone barrageZone = barrage.GetComponent<ArtilleryZone>();
                    if (barrageZone)
                    {
                        barrageZone.SetHomingTarget(owner.GetTarget());
                    }
                }
                

            }
            else
            {
               
                if (BossRoomManager.instance)
                {
                    Vector2 bounds = BossRoomManager.instance.GetRoomHalfSize();
                    Vector2 randomPointInBounds = new Vector2(Random.Range(-bounds.x, bounds.x),
                        Random.Range(-bounds.y, bounds.y));

                    spawnPoint = BossRoomManager.instance.GetRoomCentrePoint() + randomPointInBounds;



                }
                else
                {
                    spawnPoint = transform.position+(Vector3)Random.insideUnitCircle * 10f;
                }
                ObjectPoolManager.Spawn(artilleryPrefab, spawnPoint, Quaternion.identity);

             
            }
         
            spawnRate = Random.Range(minArtillerySpawnRate, maxArtillerySpawnRate);
            yield return new WaitForSeconds(spawnRate);
        }
        EvaluateRemainingAttacks();
    }
    public override void EnableAbility()
    {
        base.EnableAbility();

        if (eventListener)
        {
            eventListener.OnShowAttackZone += BeginArtilleryBarrage;
        }
    }
    public override void DisableAbility()
    {
        base.DisableAbility();
        StopAllCoroutines();
        if(eventListener)
            eventListener.OnShowAttackZone -= BeginArtilleryBarrage;
    }
}

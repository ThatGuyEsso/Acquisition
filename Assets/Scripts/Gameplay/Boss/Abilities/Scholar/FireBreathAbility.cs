using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBreathAbility : BaseBossAbility
{
    [Header("Firebreath Settings")]
    [SerializeField] private GameObject damageVolumePrefab;
    [SerializeField] private GameObject fireBreathVFXPrefab;
    [SerializeField] private float breathDuration;
    [SerializeField] private float attackZoneRadius;

    [Range(0f,180f)]
    [SerializeField] private float attackZoneArchAngle;
    private float currDuration;
    private DynamicConeCollider damageVolume;
    private GameObject fireBreathVFX;
    private bool isAttacking;
    public void BeginFireBreath()
    {
        attacksLeft--;
        isAttacking = true;
        currDuration = breathDuration;

        if (fireBreathVFX) ObjectPoolManager.Recycle(fireBreathVFX);

        fireBreathVFX = ObjectPoolManager.Spawn(fireBreathVFXPrefab, owner.GetFirePoint().position, owner.GetFirePoint().rotation);

        if (damageVolume) ObjectPoolManager.Recycle(damageVolume);
        damageVolume = ObjectPoolManager.Spawn(damageVolumePrefab, Vector3.zero, Quaternion.identity).GetComponent<DynamicConeCollider>();

        if (damageVolume)
        {
            damageVolume.SetColliderShape(owner.GetFirePoint().up, attackZoneRadius, attackZoneArchAngle, owner.GetFirePoint().position);
            IVolumes attackVol = damageVolume.GetComponent<IVolumes>();
            if (attackVol != null)
            {
                attackVol.SetIsPlayerZone(false);
                attackVol.SetUpDamageVolume(1f, 10f, owner.GetFirePoint().up, owner.gameObject);
            }
        }


    }
 

    public void StopFireBreath()
    {
        if (fireBreathVFX)
        {
            fireBreathVFX.GetComponent<ParticleSystem>().Stop();

        }


        fireBreathVFX = null;
        if (damageVolume) ObjectPoolManager.Recycle(damageVolume);
        damageVolume = null;
        isAttacking = false;
        EvaluateRemainingAttacks();
    }

    private void Update()
    {
        if (isAttacking)
        {
            if(currDuration <= 0f)
            {
                StopFireBreath();
            }
            else
            {
                currDuration -= Time.deltaTime;
            }

         

        }
    }

    private void LateUpdate()
    {

        if (isAttacking)
        {
            if (fireBreathVFX)
            {
                fireBreathVFX.transform.position = owner.GetFirePoint().position;
                fireBreathVFX.transform.rotation = owner.GetFirePoint().rotation;
            }

            if (damageVolume)
            {
                damageVolume.SetColliderShape(owner.GetFirePoint().up, attackZoneRadius, attackZoneArchAngle, owner.GetFirePoint().position);
            }
        }

    }


    public void EvaluateRemainingAttacks()
    {
        owner.PlayAnimation("EndFireBreath");
        if (attacksLeft <= 0)
        {
            eventListener.OnShowAttackZone -= BeginFireBreath;

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

    public void Lockon()
    {
        eventListener.OnShowAttackZone -= Lockon;
        owner.SetCanLockOn(false);
    }

    public override void EnableAbility()
    {
        base.EnableAbility();

        if (eventListener)
        {
            eventListener.OnShowAttackZone += Lockon;
            eventListener.OnShowAttackZone += BeginFireBreath;
        }
    }
    public override void DisableAbility()
    {
        base.DisableAbility();
        StopAllCoroutines();
        if (eventListener)
            eventListener.OnShowAttackZone -= Lockon;
            eventListener.OnShowAttackZone -= BeginFireBreath;
    }
}

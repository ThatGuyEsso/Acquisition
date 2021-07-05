using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryZone : MonoBehaviour
{
    [SerializeField] private GameObject attackZonePrefab;
    [SerializeField] private float targetZoneSize;
    [SerializeField] private float growRate;
    [SerializeField] private bool canHome;
    [SerializeField] private float homingSpeed;
    [SerializeField] private float minHazardTime;
    [SerializeField] private float maxHazardTime;

    [SerializeField] private Transform target;
    [SerializeField] private Animator animator;
    private bool isHoming;
    private AttackVolume attackZone;
    float currentZoneSize;
    bool isGrowing;

    public System.Action<ArtilleryZone> OnTriggered;
    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        isGrowing = true;
        animator.enabled = false;
        if (GameManager.instance)
        {
            GameManager.instance.OnNewEvent += EvaluateNewGameEvent;
        }

        if (AudioManager.instance)
        {
            //AudioManager.instance.PlayThroughAudioPlayer("ElderCastSFX", transform.position);
      
        }

    }

    virtual protected void EvaluateNewGameEvent(GameEvents newEvent)
    {
        switch (newEvent)
        {

            case GameEvents.PlayerDefeat:
                StopAllCoroutines();
                if (gameObject)
                    ObjectPoolManager.Recycle(gameObject);
                break;

            case GameEvents.BossDefeated:
                StopAllCoroutines();
                if (gameObject)
                    ObjectPoolManager.Recycle(gameObject);
                break;
        }
    }

    public void Update()
    {


        if (isGrowing)
        {
            transform.localScale += Vector3.one * Time.deltaTime * growRate;
            if (transform.localScale.x >= targetZoneSize)
            {

                transform.localScale = Vector3.one * targetZoneSize;
                isGrowing = false;
                if (canHome) canHome = false;

                StartAttack();
            }
        }

    }

    private void LateUpdate()
    {

        if (canHome && target)
        {
            if (Vector2.Distance(target.position, transform.position) >= 0.5f)
            {
                Vector3 dir = target.position - transform.position;
                transform.position += dir.normalized * Time.deltaTime * homingSpeed;
            }

        }

    }

    public void StartAttack()
    {
        animator.enabled = true;
        animator.Play("Artillery_Charge", 0, 0f);
    }


    public void SpawnAttackZone()
    {
        attackZone = ObjectPoolManager.Spawn(attackZonePrefab, transform.position, Quaternion.identity).GetComponent<AttackVolume>();
        StartCoroutine(RecycleTimer(Random.Range(minHazardTime, maxHazardTime)));
        OnTriggered?.Invoke(this);
    }

    public void SetHomingTarget(Transform hometarget)
    {
        if (hometarget)
        {
            target = hometarget;
            isHoming = true;
        }
    }

    private IEnumerator RecycleTimer(float time)
    {
        yield return new WaitForSeconds(time);
        EndAttack();
    }

    public void EndAttack()
    {
        animator.Play("Artillery_Dissolve", 0, 0f);
        if (attackZone)
            ObjectPoolManager.Recycle(attackZone.gameObject);

        attackZone = null;
    }

    public void ClearArtilleryAttack()
    {
        if (gameObject)
            ObjectPoolManager.Recycle(gameObject);
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        animator.enabled = false;

        if (attackZone)
            ObjectPoolManager.Recycle(attackZone.gameObject);

    }
}

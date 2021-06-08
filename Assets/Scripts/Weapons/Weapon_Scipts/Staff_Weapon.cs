using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff_Weapon : Base_Weapon
{
    [SerializeField] private float beamDuration;
    [SerializeField] private GameObject sheild;
    private LineRenderer line;
    private MouseMoveCursor vCursor;
    private GameObject currentShield;
    private bool isFireing = false;

    public override void Init()
    {
        base.Init();
        line = GetComponent<LineRenderer>();
        vCursor = GameObject.FindGameObjectWithTag("Player").GetComponent<MouseMoveCursor>();

    }

    protected override void PrimaryAttack()
    {
        if (canFire == false)
            return;

        line.enabled = true;
        isFireing = true;

        StartCoroutine(PrimaryAttackDuration());
        base.PrimaryAttack();
    }

    IEnumerator PrimaryAttackDuration()
    {
        yield return new WaitForSeconds(beamDuration);
        isFireing = false;
        line.enabled = false;
    }

    private void Update()
    {
        if(isFireing)
            FireRay();
    }

    protected override void SecondaryAttack()
    {
        if (canFire == false)
            return;

        currentShield = Instantiate(sheild, transform.position, Quaternion.identity);

        base.SecondaryAttack();
    }

    protected override void AfterFireRate()
    {
        Destroy(currentShield);
    }

    private void FireRay()
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(firePoint.transform.position, firePoint.transform.up);

        if (hit != false)
        {
            Vector3 startPos = firePoint.transform.position;

            line.SetPosition(0, startPos);
            line.SetPosition(1, hit.transform.position);
            
        }
        else
        {
            Vector3 startPos = firePoint.transform.position;
            Vector3 dir = vCursor.GetVCusorPosition() - startPos;

            line.SetPosition(0, startPos);

            line.SetPosition(1, dir * 1000);

        }


    }






}

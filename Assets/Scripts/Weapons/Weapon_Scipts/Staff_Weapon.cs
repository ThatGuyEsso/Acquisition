using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff_Weapon : Base_Weapon
{
    [Header("Staff Settings")]
    [SerializeField] private float beamDuration;
    [SerializeField] private float sheildDuration = 5.0f;
    [SerializeField] private GameObject sheild;

    private LineRenderer line;
    private MouseMoveCursor vCursor;
    private GameObject currentShield;



    public override void Init()
    {
        base.Init();
        line = GetComponent<LineRenderer>();
        vCursor = GameObject.FindGameObjectWithTag("Player").GetComponent<MouseMoveCursor>();
    }

    protected override void PrimaryAttack()
    {
        if (!isWeaponActive)
            return;

        if (canPrimaryFire == true)
        {
            isFiringPrimary = true;
            canPrimaryFire = false;

            line.enabled = true;
            StartCoroutine(PrimaryAttackDuration());
        }
        else if (canPrimaryFire == false)
        {
            isFiringPrimary = false;
            canPrimaryFire = true;
            line.enabled = false;
            StopCoroutine(PrimaryAttackDuration());
            StartCoroutine(WaitForFirePrimaryRate(primaryFireRate));
        }


        
    }

    IEnumerator PrimaryAttackDuration() //The time the beam fires for
    {
        yield return new WaitForSeconds(beamDuration);
        isFiringPrimary = false;
        line.enabled = false;
        StartCoroutine(WaitForFirePrimaryRate(primaryFireRate));
    }

    private void Update()
    {
        if(isFiringPrimary) //When Beam is fired it is updated here
            FireRay();
    }


    private void FireRay()
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(firePoint.transform.position, firePoint.transform.up); //fire ray

        if (hit != false) //if the ray hits somthing then the line is drawn to that point   
        {
            Vector3 startPos = firePoint.transform.position;

            line.SetPosition(0, startPos);
            line.SetPosition(1, hit.transform.position);

        }
        else
        { // if the ray does not hit anything it is drawn for a far distance
            Vector3 startPos = firePoint.transform.position;
            Vector3 dir = vCursor.GetVCusorPosition() - startPos;

            line.SetPosition(0, startPos);

            line.SetPosition(1, dir * 1000);

        }


    }

    protected override void SecondaryAttack()
    {
        if (!isWeaponActive)
            return;
        currentShield = ObjectPoolManager.Spawn(sheild, transform.position, Quaternion.identity);

        base.SecondaryAttack();
        StartCoroutine(ShieldDuration());

    }

    private IEnumerator ShieldDuration()
    {
        yield return new WaitForSeconds(sheildDuration);
        ObjectPoolManager.Recycle(currentShield);
    }






}

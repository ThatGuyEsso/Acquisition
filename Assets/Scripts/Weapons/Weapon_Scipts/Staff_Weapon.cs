using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff_Weapon : Base_Weapon
{
    [Header("Staff Settings")]
    [SerializeField] private float beamDuration;

    [SerializeField] private float beamTickRate;
    [SerializeField] private float beamLength;
    [SerializeField] private LayerMask beamLayers;
    [SerializeField] private float sheildDuration = 5.0f;
    [SerializeField] private GameObject sheild;
    [SerializeField] private float beamDrawSpeed;

    private LineRenderer line;
    private MouseMoveCursor vCursor;
    private GameObject currentShield;
    private float currenTickTime;
    Vector2 currentPoint;
    Vector2 targetPoint;
    bool isDrawing;
    bool isBeamDying;


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

        if (!canPrimaryFire)
            return;

        if (isBusy)
            return;

        isBusy = true;
        canPrimaryFire = false;
        isIdle = false;
        currTimeToIdle = timeToIdle;
   
        attackEvents.OnShootProjectile += BeginBeam;
        animSolver.PlayAnimationFromStart("Primary_Staff");

    }

    public override void Equip(Transform firePoint, AttackAnimEventListener eventListener, Transform player, TopPlayerGFXSolver solver)
    {

        if (!isInitialised) Init();
        else
            inputAction.Enable();
        inputAction.Attack.PrimaryAttack.canceled += ctx => ResetPrimaryFire();
        inputAction.Attack.PrimaryAttack.started += ctx => PrimaryAttack();
        inputAction.Attack.SecondaryAttack.performed += ctx => SecondaryAttack();
        this.firePoint = firePoint;
        attackEvents = eventListener;
        playerTransform = player;
        SetCanFire(true);
        animSolver = solver;
        animSolver.movement.OnWalk += OnRun;
        animSolver.movement.OnStop += OnStop;
    }
    public override void UnEquip()
    {
        inputAction.Disable();
        inputAction.Attack.PrimaryAttack.started -= ctx => PrimaryAttack();
        inputAction.Attack.SecondaryAttack.performed -= ctx => SecondaryAttack();
        SetCanFire(false);
        animSolver.movement.OnWalk -= OnRun;
        animSolver.movement.OnStop -= OnStop;
        inputAction.Attack.PrimaryAttack.canceled -= ctx => ResetPrimaryFire();
    }
    private void Update()
    {
        if (isFiringPrimary) //When Beam is fired it is updated here
            FireRay();

        if (!isIdle && !isBusy)
        {
            if (currTimeToIdle <= 0f)
            {
                isIdle = true;
                if (isRunning)
                {
                    animSolver.PlayAnimation("Run_Staff");
               
                }
                else
                {
                    animSolver.PlayAnimation("Idle_Staff");
                }
                currTimeToIdle = timeToIdle;
          
            }
            else
            {
                currTimeToIdle -= Time.deltaTime;
            }
        }
  
    }


    private void LateUpdate()
    {
        if (isDrawing)
        {
            DrawBeam();
        }
    }


    private void DrawBeam()
    {
        if (line.positionCount == 0) line.positionCount = 2;

        Vector2 dirToPoint = targetPoint - currentPoint;
        currentPoint += dirToPoint.normalized * Time.deltaTime * beamDrawSpeed;
        if (Vector2.Distance(currentPoint, targetPoint) <= 0.05f) currentPoint = targetPoint;
        line.SetPosition(0, firePoint.position);
        line.SetPosition(1, targetPoint);
    }
    public void BeginBeam()
    {
        attackEvents.OnShootProjectile -= BeginBeam;
        currentPoint = firePoint.position;
        targetPoint = firePoint.position;
        isFiringPrimary = true;
        line.enabled = true;
        isDrawing = true;
        StartCoroutine(PrimaryAttackDuration());

    }

    IEnumerator PrimaryAttackDuration() //The time the beam fires for
    {
        yield return new WaitForSeconds(beamDuration);
        ResetPrimaryFire();
    }

 


    private void FireRay()
    {
        RaycastHit2D hit;
        Vector3 dir = (vCursor.GetVCusorPosition() - firePoint.position).normalized;
        hit = Physics2D.Raycast(firePoint.position, dir, beamLength,beamLayers); //fire ray

        if (hit) targetPoint = hit.point;
        else targetPoint = firePoint.position + dir * beamLength;


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




    public override void OnStop()
    {
        base.OnStop();
        if (isIdle)
        {
            animSolver.PlayAnimation("Idle_Staff");
        }
    }

    public override void OnRun()
    {
        base.OnRun();
        if (isIdle)
        {
            animSolver.PlayAnimation("Run_Staff");
        }
    }


    public override void ResetPrimaryFire()
    {
        if (isFiringPrimary)
        {
            attackEvents.OnAnimEnd -= ResetPrimaryFire;
            line.positionCount = 0;
            isFiringPrimary = false;
            isDrawing = false;
            isBusy = false;
            line.enabled = false;
            StopAllCoroutines();
            StartCoroutine(WaitForFirePrimaryRate(primaryFireRate));
        
        }
      

    
    
    }
}

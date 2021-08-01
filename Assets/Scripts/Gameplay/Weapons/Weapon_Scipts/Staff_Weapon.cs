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

    [SerializeField] private float beamDrawSpeed;

    [Header("Staff VFX")]
    [SerializeField] private GameObject  beatEndVFXPrefab;
    [SerializeField] private GameObject channelVFXPreab;
    private LineRenderer line;
    private MouseMoveCursor vCursor;

    private float currenTickTime;

    private float currBeamDuration;
    Vector2 currentPoint;
    Vector2 targetPoint;
    bool isDrawing;
    private GameObject beamEnd;
    private GameObject channelVFX;

    private AudioPlayer beamPlayer;
    public override void Init()
    {
        base.Init();
        line = GetComponent<LineRenderer>();
        vCursor = GameObject.FindGameObjectWithTag("Player").GetComponent<MouseMoveCursor>();
        if (!beamEnd) beamEnd = ObjectPoolManager.Spawn(beatEndVFXPrefab, Vector3.zero, Quaternion.identity);
        beamEnd.SetActive(false);
        if (!channelVFX) channelVFX = ObjectPoolManager.Spawn(channelVFXPreab, Vector3.zero, Quaternion.identity);
        channelVFX.SetActive(false);
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
        if (isFiringPrimary)
        {
            //When Beam is fired it is updated here
            FireRay();

            if(currenTickTime <= 0)
            {
                FireDamageRay();
                currenTickTime = beamTickRate;
            }
            else
            {
                currenTickTime -= Time.deltaTime;
            }

            if(currBeamDuration <= 0f)
            {
                currBeamDuration = beamDuration;
                ResetPrimaryFire();

            }
            else
            {
                currBeamDuration -= Time.deltaTime;
            }
        }

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
            if(!beamEnd.activeInHierarchy)beamEnd.SetActive(true);

            beamEnd.transform.position = targetPoint;
            beamEnd.transform.rotation = firePoint.rotation;

            if (!channelVFX.activeInHierarchy) channelVFX.SetActive(true);

            channelVFX.transform.position = firePoint.position;
            channelVFX.transform.rotation = firePoint.rotation;
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
        currBeamDuration = beamDuration;
        if (AudioManager.instance) AudioManager.instance.PlayThroughAudioPlayer("StaffSpellCast", playerTransform.position);
        if (!beamPlayer) beamPlayer = AudioManager.instance.PlayThroughAudioPlayer("StaffBeam", playerTransform);
        else
        {
            beamPlayer.KillAudio();
            beamPlayer = AudioManager.instance.PlayThroughAudioPlayer("StaffBeam", playerTransform);
        }
    }



    IEnumerator PrimaryAttackDuration() //The time the beam fires for
    {
        yield return new WaitForSeconds(beamDuration);
        ResetPrimaryFire();
    }

 

    public void CreateBubbleShield()
    {
        attackEvents.OnShowAttackZone -= CreateBubbleShield;
        attackEvents.OnHideAttackZone += ResetSecondaryFire;
        
        GameObject shield = ObjectPoolManager.Spawn(secondaryProjectile, playerTransform);
        shield.GetComponent<BubbleShield>().owner = playerTransform.gameObject;
        if(AudioManager.instance) AudioManager.instance.PlayThroughAudioPlayer("ShieldSpawn",playerTransform.position);
        OnSecondaryAbility?.Invoke(shield);
    }
    private void FireRay()
    {
        RaycastHit2D hit;

        Vector2 dir;

        Transform pointerTransform = null;
        if (ControllerManager.instance) pointerTransform = ControllerManager.instance.GetActiveCursor();

        if (pointerTransform != null)
        {
            dir = (pointerTransform.position - firePoint.position).normalized;
        }
        else
        {
            dir = firePoint.up;
        }

        hit = Physics2D.Raycast(firePoint.position, dir, beamLength,beamLayers); //fire ray

        if (hit) targetPoint = hit.point;
        else targetPoint = (Vector2)firePoint.position + dir * beamLength;


    }
    private void FireDamageRay()
    {
        RaycastHit2D hit;
        Vector2 dir;

        Transform pointerTransform = null;
        if (ControllerManager.instance) pointerTransform = ControllerManager.instance.GetActiveCursor();

        if (pointerTransform != null)
        {
            dir = (pointerTransform.position - firePoint.position).normalized;
        }
        else
        {
            dir = firePoint.up;
        }

        hit = Physics2D.Raycast(firePoint.position, dir, beamLength, beamLayers); //fire ray

        if (hit.collider)
        {
            IDamage target = hit.collider.GetComponent<IDamage>();
            if (target!=null)
            {
                target.OnDamage(primaryAttackDamage, playerTransform.up, 10f, playerTransform.gameObject);
            }
        }

        OnPrimaryAttack?.Invoke();
    }

    protected override void SecondaryAttack()
    {
        if (!isWeaponActive)
            return;

        if (!canSecondaryFire)
            return;

        if (isBusy)
            return;

        isBusy = true;
        canSecondaryFire = false;
        isIdle = false;
        currTimeToIdle = timeToIdle;

        attackEvents.OnShowAttackZone += CreateBubbleShield;
        animSolver.PlayAnimationFromStart("Secondary_Staff");
        OnSecondaryAttack?.Invoke();
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
        if(beamEnd)
            if (beamEnd.activeInHierarchy) beamEnd.SetActive(false);
       if(channelVFX)
            if (channelVFX.activeInHierarchy) channelVFX.SetActive(false);
        if (isFiringPrimary)
        {
            attackEvents.OnShootProjectile -= BeginBeam;
            attackEvents.OnAnimEnd -= ResetPrimaryFire;
            line.positionCount = 0;
            isFiringPrimary = false;
            isDrawing = false;
            isBusy = false;
            line.enabled = false;
      
            StartCoroutine(WaitForFirePrimaryRate(primaryFireRate));
            if (AudioManager.instance) AudioManager.instance.PlayThroughAudioPlayer("ShieldDespawn", transform.position);
        }
        else
        {
            line.positionCount = 0;
            isFiringPrimary = false;
            isDrawing = false;
            isBusy = false;
            line.enabled = false;
            attackEvents.OnShootProjectile -= BeginBeam;
            StopCoroutine(WaitForFirePrimaryRate(primaryFireRate));
           
            canPrimaryFire = true;
        }
        if (beamPlayer)
        {
            beamPlayer.KillAudio();
            beamPlayer = null;
        }

        
    }

    public override void DisableWeapon()
    {
        base.DisableWeapon();

        attackEvents.OnHideAttackZone -= ResetSecondaryFire;
        StopCoroutine(WaitForFireSecondaryRate(secondaryFireRate));
        if (beamPlayer)
        {
            beamPlayer.KillAudio();
            beamPlayer = null;
        }
        if (isFiringPrimary)
        {
            attackEvents.OnShootProjectile -= BeginBeam;
            attackEvents.OnAnimEnd -= ResetPrimaryFire;
            line.positionCount = 0;
            isFiringPrimary = false;
            isDrawing = false;
            isBusy = false;
            line.enabled = false;
            StopCoroutine(PrimaryAttackDuration());
        }
        else
        {
            line.positionCount = 0;
            isFiringPrimary = false;
            isDrawing = false;
            isBusy = false;
            line.enabled = false;
            attackEvents.OnShootProjectile -= BeginBeam;
            StopCoroutine(WaitForFirePrimaryRate(primaryFireRate));
            StopCoroutine(PrimaryAttackDuration());
      
        }
    }

    public override void EnableWeapon()
    {
        base.EnableWeapon();
        if (!canPrimaryFire) ResetPrimaryFire();
        if (!canSecondaryFire) ResetSecondaryFire();

    }
    public override void ResetSecondaryFire()
    {
        attackEvents.OnHideAttackZone -= ResetSecondaryFire;

        isBusy = false;
        StartCoroutine(WaitForFireSecondaryRate(secondaryFireRate));
    }
}

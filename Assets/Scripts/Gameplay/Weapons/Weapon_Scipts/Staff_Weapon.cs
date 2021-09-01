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

    [Header("Staff Cooldown UX")]
    [SerializeField] private GameObject cooldownSliderPrefab;
    [SerializeField] private Color beamCooldownBarColour;
    [SerializeField] private Color shieldCooldownBarColour;
    [SerializeField] private float cooldownBarOffset =1.2f;

    //Progress bar management
    private CooldownSlider primCooldownSlider;
    private ProgressBar primCooldownProgressBar;

    private CooldownSlider secCooldownSlider;
    private ProgressBar secCooldownProgressBar;

    private float primCurrFireCooldown;
    private float secCurrFireCooldown;


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
        InitCooldownProgressUI();
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
        ClearCooldownProgressBar();
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
        if (!canPrimaryFire)
        {
            if(primCurrFireCooldown <= 0)
            {
              
            
                if (primCooldownProgressBar) primCooldownProgressBar.UpdateSlider(primCurrFireCooldown);
                HidePrimaryCooldownProgressBar();
                canPrimaryFire = true;
            }
            else
            {
                primCurrFireCooldown -= Time.deltaTime;
                if (primCooldownProgressBar) primCooldownProgressBar.UpdateSlider(primCurrFireCooldown);
            }
        }

        if (!canSecondaryFire)
        {
            if (secCurrFireCooldown <= 0)
            {
            
             
                if (secCooldownProgressBar) secCooldownProgressBar.UpdateSlider(secCurrFireCooldown);
                HideSecondaryCooldownProgressBar();
                canSecondaryFire = true;
            }
            else
            {
                secCurrFireCooldown -= Time.deltaTime;
                if (secCooldownProgressBar) secCooldownProgressBar.UpdateSlider(secCurrFireCooldown);
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


            if (primCurrFireCooldown <= 0f)
                primCurrFireCooldown = primaryFireRate;
            canPrimaryFire = false;
            ShowPrimaryCooldownBar();

            if (AudioManager.instance) AudioManager.instance.PlayThroughAudioPlayer("ShieldDespawn", transform.position);
        }
        else if(canPrimaryFire)
        {
            animSolver.PlayAnimation("Idle_Staff");
            line.positionCount = 0;
            isFiringPrimary = false;
            isDrawing = false;
            isBusy = false;
            line.enabled = false;
            attackEvents.OnShootProjectile -= BeginBeam;
            StopCoroutine(WaitForFirePrimaryRate(primaryFireRate));
           
           
        }else if (primCurrFireCooldown > 0f)
        {
            canPrimaryFire = false;
            ShowPrimaryCooldownBar();
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
        HidePrimaryCooldownProgressBar();
        HideSecondaryCooldownProgressBar();
    }
    public void InitCooldownProgressUI()
    {
        if (cooldownSliderPrefab)
        {
            primCooldownSlider = ObjectPoolManager.Spawn(cooldownSliderPrefab, transform.position, cooldownSliderPrefab.transform.rotation)
                .GetComponent<CooldownSlider>();
            secCooldownSlider = ObjectPoolManager.Spawn(cooldownSliderPrefab, transform.position, cooldownSliderPrefab.transform.rotation)
              .GetComponent<CooldownSlider>();
            if (primCooldownSlider)
            {
                primCooldownSlider.SetUpCooldownSlider(beamCooldownBarColour, playerTransform, true, -Vector2.up * cooldownBarOffset);
                primCooldownProgressBar = primCooldownSlider.GetComponent<ProgressBar>();
                primCooldownProgressBar.InitSlider(primaryFireRate);
                primCooldownSlider.gameObject.SetActive(false);
            }
            if (secCooldownSlider)
            {
                secCooldownSlider.SetUpCooldownSlider(shieldCooldownBarColour, playerTransform, true, -Vector2.up * cooldownBarOffset);
                secCooldownProgressBar = secCooldownSlider.GetComponent<ProgressBar>();
                secCooldownProgressBar.InitSlider(secondaryFireRate);
                secCooldownSlider.gameObject.SetActive(false);
            }
        }
    }


    public void ShowPrimaryCooldownBar()
    {
        if (primCooldownSlider)
        {
            if (primCooldownSlider.gameObject.activeInHierarchy) return;
            primCooldownSlider.gameObject.SetActive(true);
            if (IsShieldOnCooldown())
            {
                primCooldownSlider.SetOffSet(-Vector2.up * (cooldownBarOffset * 1.35f));
            }
            else
            {
                primCooldownSlider.SetOffSet(-Vector2.up * (cooldownBarOffset));
            }
            
            if (primCooldownProgressBar) primCooldownProgressBar.UpdateSlider(primCurrFireCooldown);
        }
    }
    public void HidePrimaryCooldownProgressBar()
    {
        if (primCooldownSlider)
        {
            if (primCooldownSlider.gameObject.activeInHierarchy) primCooldownSlider.gameObject.SetActive(false);
        }

        if (IsShieldOnCooldown())
        {
            secCooldownSlider.SetOffSet(-Vector2.up * (cooldownBarOffset));
        }
    }
    public void ShowSecondaryCooldownBar()
    {
        if (secCooldownSlider)
        {
            if (secCooldownSlider.gameObject.activeInHierarchy) return;
            secCooldownSlider.gameObject.SetActive(true);
            if (IsBeamOnCooldown())
            {
                secCooldownSlider.SetOffSet(-Vector2.up * (cooldownBarOffset * 1.35f));
            }
            else
            {
                secCooldownSlider.SetOffSet(-Vector2.up * (cooldownBarOffset));
            }
            if (secCooldownProgressBar) secCooldownProgressBar.UpdateSlider(secondaryFireRate);
        }
    }
    public void HideSecondaryCooldownProgressBar()
    {
        if (secCooldownSlider)
        {
            if (secCooldownSlider.gameObject.activeInHierarchy) secCooldownSlider.gameObject.SetActive(false);
        }

        if (IsBeamOnCooldown())
        {
            primCooldownSlider.SetOffSet(-Vector2.up * (cooldownBarOffset ));
        }
     
    }
    public bool IsShieldOnCooldown()
    {
        if (secCooldownSlider)
        {
            return secCooldownSlider.gameObject.activeInHierarchy;
        }
        return false;
    }
    public bool IsBeamOnCooldown()
    {
        if (primCooldownSlider)
        {
            return primCooldownSlider.gameObject.activeInHierarchy;
        }
        return false;
    }
    public void ClearCooldownProgressBar()
    {
        if (primCooldownSlider) ObjectPoolManager.Recycle(primCooldownSlider);
        if (secCooldownSlider) ObjectPoolManager.Recycle(secCooldownSlider);
    
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
        if(secCurrFireCooldown<=0f)
            secCurrFireCooldown = secondaryFireRate;
        canSecondaryFire = false;
        ShowSecondaryCooldownBar();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Bow_Weapon : Base_Weapon
{
    [Header("Bow Settings")]
    [SerializeField] private float primaryShotSpeed;
    [SerializeField] private float primaryShotKnockBack;
    [SerializeField] private float primaryShotLifeTime;

    [SerializeField] private float secondaryShotSpeed;
    [SerializeField] private float secondaryShotLifeTime;

    [Header("Prefabs")]
    [SerializeField] private GameObject weakCharge, midCharge, superCharge;
    [Header("UX Settings")]
    [SerializeField] private GameObject lightPrefab;
    [SerializeField] private Color cooldownSliderColor;
    [SerializeField] private GameObject cooldownSliderPrefab;
    [SerializeField] private GameObject gamePadArrowPrefab;
    [SerializeField] private float gamePadArrowOffset;

    private CooldownSlider cooldownSlider;
    private ProgressBar cooldownProgressBar;
    private Transform vCursor;
    private int chargeCount = 0;
    bool isCharging = false;
    private Light2D light2d;

    private GameObject gamepadPointer;
    private float currSecFireCooldown;
    public override void Init()
    {
        base.Init();
      
    }

    protected override void PrimaryAttack()
    {
        if (!isWeaponActive)
            return;

        if (!canPrimaryFire) return;

        if (isBusy) return;


        isBusy = true;
        isIdle = false;
        currTimeToIdle = timeToIdle;
        canPrimaryFire = false;

        OnPrimaryAttack?.Invoke();
        attackEvents.OnShootProjectile += OnFireProjectile;
        attackEvents.OnPlaySFX += PlayArrowDrawSFX;
        attackEvents.OnAnimEnd += ResetPrimaryFire;
        animSolver.PlayAnimationFromStart("Primary_Bow");
   
    }


    private void Update()
    {
     
        if (!isIdle && !isBusy)
        {
            if (currTimeToIdle <= 0f)
            {
                isIdle = true;
                if (isRunning)
                {
                    animSolver.PlayAnimation("Run_Bow");
                    Debug.Log("run");
                }
                else
                {
                    animSolver.PlayAnimation("Idle_Bow");
                }
                currTimeToIdle = timeToIdle;
            }
            else
            {
                currTimeToIdle -= Time.deltaTime;
            }
        }

        if (!canSecondaryFire&&isEnabled)
        {
            if (currSecFireCooldown <= 0f)
            {
                canSecondaryFire = true;
                if (cooldownProgressBar) cooldownProgressBar.UpdateSlider(0f);
                HideCooldownProgressBar();
                if (secondaryHeld) SecondaryAttack();

            }
            else
            {
                currSecFireCooldown -= Time.deltaTime;
                if (cooldownProgressBar) cooldownProgressBar.UpdateSlider(currSecFireCooldown);
            }
        }
    }

    public override void OnFireProjectile()
    {
        Transform pointerTransform = null;
        if (ControllerManager.instance) pointerTransform = ControllerManager.instance.GetActiveCursor();

        Vector2 dir;
        if (pointerTransform != null)
        {
             dir = (pointerTransform.position - firePoint.position).normalized;
        }
        else
        {
            dir = firePoint.up;
        }
   
        GameObject go = ObjectPoolManager.Spawn(primaryProjectile, firePoint.transform.position, Quaternion.identity);
        IProjectile projectile = go.GetComponent<IProjectile>();
        if (projectile != null)
        {
            projectile.SetUpProjectile(primaryAttackDamage, dir, primaryShotSpeed, primaryShotLifeTime, 0, playerTransform.gameObject);
            OnPrimaryAbility?.Invoke(go);

        }
        attackEvents.OnShootProjectile -= OnFireProjectile;
        PlayArrowShotSFX();
      
    }

    public override void Equip(Transform firePoint, AttackAnimEventListener eventListener, Transform player, TopPlayerGFXSolver solver)
    {
        if (!isInitialised)
        {
            Init();
            inputAction.Attack.PrimaryAttack.started += ctx => PrimaryAttack();
            inputAction.Attack.PrimaryAttack.started += ctx => OnPrimaryHeld();
            inputAction.Attack.SecondaryAttack.started += ctx => SecondaryAttack();
            inputAction.Attack.SecondaryAttack.started += ctx => OnSecondaryHeld();
            inputAction.Attack.SecondaryAttack.canceled += ctx => EvaluateChargeShot();
            inputAction.Attack.SecondaryAttack.canceled += ctx => OnSecondaryReleased();
            inputAction.Attack.PrimaryAttack.canceled += ctx => OnPrimaryReleased();
        }
        else
            inputAction.Enable();

        this.firePoint = firePoint;
        attackEvents = eventListener;
        playerTransform = player;
        SetCanFire(true);
        animSolver = solver;
        animSolver.movement.OnWalk += OnRun;
        animSolver.movement.OnStop += OnStop;
        
        if (lightPrefab)
        {

            light2d = ObjectPoolManager.Spawn(lightPrefab, playerTransform).GetComponent<Light2D>();
            if (light2d) light2d.gameObject.SetActive(false);
            else ObjectPoolManager.Recycle(lightPrefab);
        }
        InitCooldownProgressUI();
        isEnabled = true;
        Debug.Log("Equip");
    }
    public override void UnEquip()
    {
        base.UnEquip();
        chargeCount = 0;
        StopAllCoroutines();
        attackEvents.OnAnimEnd -= ResetSecondaryFire;
        attackEvents.OnPlaySFX -= PlayArrowDrawSFX;
        isBusy = false;
        isCharging = false;
        isEnabled = false;
        if (light2d)
        {
            ObjectPoolManager.Recycle(light2d.gameObject);

        }
        ClearCooldownProgressBar();
        HideGamePadPonter();
        Debug.Log("unequip");
    }
    public override void DisableWeapon()
    {
        base.DisableWeapon();
        attackEvents.OnPlaySFX -= PlayArrowDrawSFX;
        attackEvents.OnShootProjectile -= OnFireProjectile;

        attackEvents.OnAnimEnd -= ResetPrimaryFire;
        if (chargeCount > 0)
        {
            attackEvents.OnChargeIncrease -= IncreaseCharge;

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
            if (chargeCount == 1)
            {

                IProjectile projectile = ObjectPoolManager.Spawn(weakCharge, firePoint.transform.position, Quaternion.identity)
                .GetComponent<IProjectile>();
                if (projectile != null)
                {
                    projectile.ShootProjectile(secondaryShotSpeed, dir, secondaryShotLifeTime);
                }
                chargeCount = 0;
            }
            else if (chargeCount == 2)
            {
                IProjectile projectile = ObjectPoolManager.Spawn(midCharge, firePoint.transform.position, Quaternion.identity)
              .GetComponent<IProjectile>();
                if (projectile != null)
                {
                    projectile.ShootProjectile(secondaryShotSpeed, dir, secondaryShotLifeTime);
                }
                chargeCount = 0;
            }
            else if (chargeCount >= 3)
            {
                IProjectile projectile = ObjectPoolManager.Spawn(superCharge, firePoint.transform.position, Quaternion.identity)
                .GetComponent<IProjectile>();
                if (projectile != null)
                {
                    projectile.ShootProjectile(secondaryShotSpeed, dir, secondaryShotLifeTime);
                }
                chargeCount = 0;
            }
            isCharging = false;
        }
        else
        {
            isCharging = false;
        }
        HideGamePadPonter();
        HideCooldownProgressBar();
    }


    public override void EnableWeapon()
    {

        base.EnableWeapon();
        if (!canPrimaryFire) ResetPrimaryFire();
        if (!canSecondaryFire) ResetSecondaryFire();
    }

    protected override void SecondaryAttack()
    {

        if (isWeaponActive == false)
            return;

        if (!canSecondaryFire)
            return;


        if (isBusy) return;


        isBusy = true;
        isIdle = false;
        canSecondaryFire = false;

        currTimeToIdle = timeToIdle;
        attackEvents.OnChargeIncrease += IncreaseCharge;
        isCharging = true;
        OnSecondaryAttack?.Invoke();
        attackEvents.OnPlaySFX += PlayArrowDrawSFX;
        animSolver.PlayAnimationFromStart("Secondary_Bow");
 
    }

    public void PlayArrowShotSFX()
    {

        if (AudioManager.instance)
        {
            AudioManager.instance.PlayThroughAudioPlayer("BowShot", playerTransform.position);
        }
    }

    public void PlayArrowDrawSFX()
    {
        attackEvents.OnPlaySFX -= PlayArrowDrawSFX;
        if (AudioManager.instance)
        {
            AudioManager.instance.PlayThroughAudioPlayer("BowDraw", playerTransform.position);
        }
    }

    private void EvaluateChargeShot()
    {

        if (isCharging)
        {
            attackEvents.OnChargeIncrease -= IncreaseCharge;

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

            if (chargeCount == 1)
            {
                GameObject projObject = ObjectPoolManager.Spawn(weakCharge, firePoint.transform.position, Quaternion.identity);
                IProjectile projectile = projObject.GetComponent<IProjectile>();

                if (projectile != null)
                {
                    projectile.SetOwner(playerTransform.gameObject);
                    projectile.ShootProjectile(secondaryShotSpeed, dir, secondaryShotLifeTime);
                    OnSecondaryAbility?.Invoke(projObject);
                }


            }
            else if (chargeCount == 2)
            {
                GameObject projObject = ObjectPoolManager.Spawn(midCharge, firePoint.transform.position, Quaternion.identity);
                IProjectile projectile = projObject.GetComponent<IProjectile>();
                if (projectile != null)
                {
                    projectile.SetOwner(playerTransform.gameObject);
                    projectile.ShootProjectile(secondaryShotSpeed, dir, secondaryShotLifeTime);
                    OnSecondaryAbility?.Invoke(projObject);
                }


            }
            else if (chargeCount >= 3)
            {
                GameObject projObject = ObjectPoolManager.Spawn(superCharge, firePoint.transform.position, Quaternion.identity);
                IProjectile projectile = projObject.GetComponent<IProjectile>();
                if (projectile != null)
                {
                    projectile.SetOwner(playerTransform.gameObject);
                    projectile.ShootProjectile(secondaryShotSpeed, dir, secondaryShotLifeTime);
                    OnSecondaryAbility?.Invoke(projObject);
                }

            }

            attackEvents.OnAnimEnd += ResetSecondaryFire;
            animSolver.PlayAnimationFromStart("ReleaseCharge");
            PlayArrowShotSFX();

        }
        if (light2d)
        {
            light2d.gameObject.SetActive(false);

        }


    }
    private void IncreaseCharge()
    {

        chargeCount++;
        if (AudioManager.instance)
        {
            AudioManager.instance.PlayGThroughAudioPlayerPitchShift("ArrowCharge", playerTransform.position, chargeCount / 10f);
        }
        if (light2d)
        {
            light2d.gameObject.SetActive(true);
            light2d.intensity = chargeCount/1.25f;
            light2d.pointLightOuterRadius = chargeCount;
        }

    }

    public override void OnStop()
    {
        base.OnStop();
        if (isIdle)
        {
            animSolver.PlayAnimation("Idle_Bow");
        }
    }

    public override void OnRun()
    {
        base.OnRun();
        if (isIdle)
        {
            animSolver.PlayAnimation("Run_Bow");
        }
    }


    override public void ResetSecondaryFire()
    {
        attackEvents.OnAnimEnd -= ResetSecondaryFire;
        isBusy = false;
        isCharging = false;

        if (chargeCount > 0)
        {
           
            canSecondaryFire = false;
            if(currSecFireCooldown<=0f)
                    currSecFireCooldown = secondaryFireRate;
            ShowCooldownProgressBar();

        }else if(currSecFireCooldown > 0f)
        {
            canSecondaryFire = false;
            ShowCooldownProgressBar();
        }


        else
        {
            canSecondaryFire = true;
        }
        chargeCount = 0;
    }

    public void InitCooldownProgressUI()
    {
        if (cooldownSliderPrefab)
        {
            cooldownSlider = ObjectPoolManager.Spawn(cooldownSliderPrefab, transform.position, cooldownSliderPrefab.transform.rotation).GetComponent<CooldownSlider>();
            if (cooldownSlider)
            {
                cooldownSlider.SetUpCooldownSlider(cooldownSliderColor, playerTransform, true,  -Vector2.up*1.2f);
                cooldownProgressBar = cooldownSlider.GetComponent<ProgressBar>();
                cooldownProgressBar.InitSlider(secondaryFireRate);
                cooldownSlider.gameObject.SetActive(false);
            }
        }
    }

    public void ClearCooldownProgressBar()
    {
        if (cooldownSlider) ObjectPoolManager.Recycle(cooldownSlider);
    }
    public void ShowCooldownProgressBar()
    {
        if (cooldownSlider)
        {
            cooldownSlider.gameObject.SetActive(true);
            if (cooldownProgressBar) cooldownProgressBar.UpdateSlider(currSecFireCooldown);
        }
    }
    public void HideCooldownProgressBar()
    {
        if (cooldownSlider)
        {
            if (cooldownSlider.gameObject.activeInHierarchy) cooldownSlider.gameObject.SetActive(false);
        }
    }

    public override void ResetPrimaryFire()
    {
        base.ResetPrimaryFire();
        if (primaryHeld) PrimaryAttack();
    }

    public void ShowGamePadPointer()
    {
        if (!gamepadPointer)
        {
            gamepadPointer = ObjectPoolManager.Spawn(gamePadArrowPrefab, playerTransform.position + firePoint.up * gamePadArrowOffset, firePoint.rotation);
            gamepadPointer.transform.SetParent(firePoint);
        }
        else
        {
            gamepadPointer.SetActive(true);
            gamepadPointer.transform.position = playerTransform.position + firePoint.up * gamePadArrowOffset;
            gamepadPointer.transform.rotation = firePoint.rotation;
            gamepadPointer.transform.SetParent(firePoint);
        }
    }
    public void HideGamePadPonter()
    {
        if (gamepadPointer)
        {
            ObjectPoolManager.Recycle(gamepadPointer);
            gamepadPointer = null;
        }
    }

    protected override void OnPrimaryHeld()
    {
        base.OnPrimaryHeld();
        if (ControllerManager.instance)
        {
            if (!ControllerManager.instance.IsUsingMouse())
            {
                ShowGamePadPointer();
            }
        }
    }
    protected override void OnPrimaryReleased()
    {
        base.OnPrimaryReleased();
        HideGamePadPonter();
    }

    protected override void OnSecondaryHeld()
    {
        base.OnSecondaryHeld();
        if (ControllerManager.instance)
        {
            if (!ControllerManager.instance.IsUsingMouse())
            {
                ShowGamePadPointer();
            }
        }
    }

    protected override void OnSecondaryReleased()
    {
        base.OnSecondaryReleased();
        HideGamePadPonter();
    }
}
   

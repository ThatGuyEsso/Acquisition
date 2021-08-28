using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Weapon : Base_Weapon, Equipable
{
    [Header("Sword Settings")]
    [SerializeField] private DynamicConeCollider swordCollider;

    [SerializeField] private float primaryCollisionRadius = 1.5f;
    [SerializeField] private float secondaryCollisionRadius = 2.5f;
    [SerializeField] private Color cooldownSliderColor;

    [Header("Sword Slash Porjectiles Settings")]
    [SerializeField] private float primarySlashSpeed = 5f;
    [SerializeField] private float primarySlashLifeTIme = 2f;

    [SerializeField] private float SecondarySlashSpeed = 10f;
    [SerializeField] private float SecondarySlashLifeTIme = 3f;
    [SerializeField] private Vector2 secondaryProjectileOffset;
    private DynamicConeCollider sCollider;
    bool isLeftSwing;



    [SerializeField] private GameObject cooldownSliderPrefab;

    private CooldownSlider cooldownSlider;
    private ProgressBar cooldownProgressBar;
    private float currSecFireCooldown;

    protected override void PrimaryAttack()
    {
        if (!isWeaponActive)
            return;

        if (!canPrimaryFire)
            return;

        if (isBusy) return;
        isBusy = true;
        attackEvents.OnShowAttackZone += CreateAttackCollider;
        attackEvents.OnHideAttackZone+= DestroyAttackZone;
        attackEvents.OnShootProjectile += OnFireProjectile;
        attackEvents.OnAnimEnd += ResetPrimaryFire;
        if (isIdle || !isLeftSwing)
        {
            isIdle = false;
            isLeftSwing = true;
            currTimeToIdle = timeToIdle;
            animSolver.PlayAnimation("Primary");
            if (AudioManager.instance)
                AudioManager.instance.PlayThroughAudioPlayer("SwordSwing", playerTransform.position, true);
        }
        else
        {
            isIdle = false;
            isLeftSwing = false;
            currTimeToIdle = timeToIdle;
            animSolver.PlayAnimation("Primary_2");
            if (AudioManager.instance)
                AudioManager.instance.PlayThroughAudioPlayer("SwordSwing", playerTransform.position, true);
        }

        OnPrimaryAttack?.Invoke();
        canPrimaryFire = false;

    }
    private void Update()
    {
        if (!isIdle)
        {
            if (currTimeToIdle <=0f)
            {
                isIdle = true;
                if (isRunning)
                {
                    animSolver.PlayAnimation("Run_Sword");

                }
                else
                {
                    animSolver.PlayAnimation("Idle_Sword");
                }
                currTimeToIdle = timeToIdle;
            }
            else
            {
                currTimeToIdle -= Time.deltaTime;
            }
        }
        if (!canSecondaryFire)
        {
            if (currSecFireCooldown <= 0f)
            {
                canSecondaryFire = true;
                if (cooldownProgressBar) cooldownProgressBar.UpdateSlider(0f);
                HideCooldownProgressBar();

            }
            else
            {
                currSecFireCooldown -= Time.deltaTime;
                if (cooldownProgressBar) cooldownProgressBar.UpdateSlider(currSecFireCooldown);
            }
        }
    }
    public void CreateAttackCollider()
    {
        sCollider = ObjectPoolManager.Spawn(swordCollider.gameObject, Vector3.zero, Quaternion.identity).GetComponent<DynamicConeCollider>();
        sCollider.SetColliderShape(firePoint.transform.up, primaryCollisionRadius, 90f, playerTransform.position);
        if (sCollider.GetComponent<IVolumes>() != null) {
            IVolumes volume = sCollider.GetComponent<IVolumes>();
            volume.SetIsPlayerZone(true);
            volume.SetUpDamageVolume(primaryAttackDamage, 10f, firePoint.up, playerTransform.gameObject);
        }
        attackEvents.OnShowAttackZone -= CreateAttackCollider;
     
    }
    public void CreateSecondaryAttackCollider()
    {
        sCollider = ObjectPoolManager.Spawn(swordCollider.gameObject, Vector3.zero, Quaternion.identity).GetComponent<DynamicConeCollider>();
        sCollider.SetColliderShape(firePoint.transform.up, secondaryCollisionRadius, 60f, playerTransform.position);
        if (sCollider.GetComponent<IVolumes>() != null)
        {
            IVolumes volume = sCollider.GetComponent<IVolumes>();
            volume.SetIsPlayerZone(true);
            volume.SetUpDamageVolume(primaryAttackDamage, 10f, firePoint.up, playerTransform.gameObject);
        }
        attackEvents.OnShowAttackZone -= CreateSecondaryAttackCollider;

    }

    public void DestroyAttackZone()
    {
        if(sCollider)
            Destroy(sCollider.gameObject);
        sCollider = null;
        attackEvents.OnHideAttackZone -= DestroyAttackZone;



    }

    public override void OnFireProjectile()
    {
        GameObject projectileObject = ObjectPoolManager.Spawn(primaryProjectile, firePoint.transform.position, Quaternion.identity);
        IProjectile projectile = projectileObject.GetComponent<IProjectile>();

        if (projectile!=null)
        {
            projectile.SetUpProjectile(primaryProjectileDamage, firePoint.up, primarySlashSpeed, primarySlashLifeTIme, 0, playerTransform.gameObject);
            OnPrimaryAbility?.Invoke(projectileObject);
        }
        attackEvents.OnShootProjectile -= OnFireProjectile;

    }


    public  void OnFireSecondaryProjectile()
    {
        if (AudioManager.instance)
            AudioManager.instance.PlayThroughAudioPlayer("SwordThrust", playerTransform.position);
        GameObject projectileObject = ObjectPoolManager.Spawn(secondaryProjectile, firePoint.transform.position, Quaternion.identity);
        IProjectile projectile = projectileObject.GetComponent<IProjectile>();
        if (projectile != null)
        {
            projectile.SetUpProjectile(secondaryProjectileDamage, firePoint.up,SecondarySlashSpeed, SecondarySlashLifeTIme,3, playerTransform.gameObject);
            OnSecondaryAbility?.Invoke(projectileObject);
        }
        attackEvents.OnShootProjectile -= OnFireSecondaryProjectile;
    }

    public override void Equip(Transform firePoint, AttackAnimEventListener eventListener, Transform player, TopPlayerGFXSolver solver)
    {
        if (!isInitialised)
        {
            Init();
            inputAction.Attack.PrimaryAttack.started += ctx => OnPrimaryHeld();
            inputAction.Attack.PrimaryAttack.canceled += ctx => OnPrimaryReleased();
            inputAction.Attack.SecondaryAttack.started += ctx => OnSecondaryHeld();
            inputAction.Attack.SecondaryAttack.canceled += ctx => OnSecondaryReleased();
            inputAction.Attack.PrimaryAttack.performed += ctx => PrimaryAttack();
            inputAction.Attack.SecondaryAttack.performed += ctx => SecondaryAttack();
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
        InitCooldownProgressUI();

    }

    
    protected override void SecondaryAttack()
    {
        if (isBusy)
        {
          
            //Debug.Log("BUSY");
           
            return;
        }
        if (!isWeaponActive)
        {

            //Debug.Log("INACTIVE");

            return;
        }

        if (!canSecondaryFire)
        {
            //Debug.Log("can't attack");
            return;
        }

        isIdle = false;
        isBusy = true;
        canSecondaryFire = false;
        isLeftSwing = true;
        attackEvents.OnShowAttackZone += CreateSecondaryAttackCollider;
        attackEvents.OnHideAttackZone += DestroyAttackZone;
        attackEvents.OnShootProjectile += OnFireSecondaryProjectile;
        attackEvents.OnAnimEnd += ResetSecondaryFire;
        currTimeToIdle = timeToIdle;
  
        animSolver.PlayAnimationFromStart("Secondary");

        OnSecondaryAttack?.Invoke();
        //Debug.Log("Second Attack");


    }



    public override void UnEquip()
    {
        base.UnEquip();
        ClearCooldownProgressBar();
    }

    public override void OnStop()
    {
        base.OnStop();
        if (isIdle)
        {
            animSolver.PlayAnimation("Idle_Sword");
        }
    }

    public override void OnRun()
    {
        base.OnRun();
        if (isIdle)
        {
            animSolver.PlayAnimation("Run_Sword");
        }
    }
    override public void ResetPrimaryFire()
    {
        canPrimaryFire = true;
        isBusy = false;
        attackEvents.OnAnimEnd -= ResetPrimaryFire;
        if (primaryHeld) PrimaryAttack();
    }

    override public void ResetSecondaryFire()
    {
        attackEvents.OnAnimEnd -= ResetSecondaryFire;
        isBusy = false;
        canSecondaryFire = false;
        currSecFireCooldown = secondaryFireRate;
        ShowCooldownProgressBar();
        Debug.Log("Not busy");
    }

    public override void DisableWeapon()
    {
        base.DisableWeapon();
        StopAllCoroutines();
        attackEvents.OnShowAttackZone -= CreateSecondaryAttackCollider;
        attackEvents.OnHideAttackZone -= DestroyAttackZone;
        attackEvents.OnShootProjectile -= OnFireSecondaryProjectile;
        attackEvents.OnAnimEnd -= ResetSecondaryFire;


        attackEvents.OnShowAttackZone -= CreateAttackCollider;
        attackEvents.OnHideAttackZone -= DestroyAttackZone;
        attackEvents.OnShootProjectile -= OnFireProjectile;
        attackEvents.OnAnimEnd -= ResetPrimaryFire;
        DestroyAttackZone();
    }



    public override void EnableWeapon()
    {
        base.EnableWeapon();
        if (!canPrimaryFire)
        {
            ResetPrimaryFire();
        }
        if (!canSecondaryFire)
        {
            ResetSecondaryFire();
        }
    }

    protected override IEnumerator WaitForFireSecondaryRate(float time)
    {
        canSecondaryFire = false;
        yield return new WaitForSeconds(time);
        canSecondaryFire = true;
        if (secondaryHeld) SecondaryAttack();
    }


    public void InitCooldownProgressUI()
    {
        if (cooldownSliderPrefab)
        {
            cooldownSlider = ObjectPoolManager.Spawn(cooldownSliderPrefab, transform.position, cooldownSliderPrefab.transform.rotation).GetComponent<CooldownSlider>();
            if (cooldownSlider)
            {
                cooldownSlider.SetUpCooldownSlider(cooldownSliderColor, playerTransform, true, -Vector2.up * 1.2f);
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
}

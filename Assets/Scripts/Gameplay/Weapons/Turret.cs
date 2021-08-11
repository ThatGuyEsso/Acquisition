using System.Collections;
using System;
using UnityEngine;

public class Turret : MonoBehaviour,IDamage
{
    [SerializeField] private Transform firepoint;
    [SerializeField] private float fireRate;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileLifeTime;
    [SerializeField] private int projectileBlockCount;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float maxHealth;
    [SerializeField] private BossUI healthUIPrefab;

    [SerializeField] private float maxHurtTime;
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject destroyVFX;
    public Action OnTurretDestroy;
    private SpriteFlash flashVFX;

    private float currTimeToFire;
    private bool isFiring = false;
    private BossUI healthUI;
    private Rotate rotation;
    private float currHealth;
    private float currHurtTime;
    private bool canBeHurt, isHurt;

    private void Awake()
    {
        currHurtTime = maxHurtTime;
        currTimeToFire = fireRate;
        currHealth = maxHealth;
        rotation = GetComponent<Rotate>();
        if(rotation)
             rotation.isRotating = false;

        flashVFX = GetComponent<SpriteFlash>();
        if(flashVFX)
            flashVFX.Init();
    }


    public void InitTurret()
    {
        if (healthUIPrefab)
            healthUI = ObjectPoolManager.Spawn(healthUIPrefab, Vector3.zero, Quaternion.identity);

        if(healthUI)
        {
            healthUI.progressBar.SetMaxValue(maxHealth);
            healthUI.InitialiseUI("Test Turret");
            healthUI.OnUISpawned += ActivateTurret;
        }

    }

    public void ActivateTurret()
    {
        if(healthUI) healthUI.OnUISpawned -= ActivateTurret;
        rotation.isRotating = true;
        isFiring = true;
    }

    public void DeactivateTurret()
    {
        if (healthUI)
        {
            healthUI.HideUI();
            healthUI.OnUISpawned+= DestroyTurret;
        }

        rotation.isRotating = false;
        isFiring = false;
    }


    public void DestroyTurret()
    {
        if (healthUI) healthUI.OnUISpawned -= DestroyTurret;

        OnTurretDestroy?.Invoke();
        if (destroyVFX) ObjectPoolManager.Spawn(destroyVFX, transform.position, Quaternion.identity);
        ObjectPoolManager.Recycle(healthUI);
        ObjectPoolManager.Recycle(gameObject);

    }
    private void Update()
    {
        if (isFiring)
        {
            if(currTimeToFire <= 0f)
            {
                ShootProjectile();
                currTimeToFire = fireRate;
            }
            else
            {
                currTimeToFire -= Time.deltaTime;
            }
        }

        if(canBeHurt && isHurt)
        {
            if(currHurtTime <= 0f)
            {
                isHurt = false;
                currHurtTime = maxHurtTime;
                if (flashVFX) flashVFX.EndFlash();
            }
            else
            {
                currHurtTime -= Time.deltaTime;
            }
        }
    }

    public void ShootProjectile()
    {
        IProjectile proj = ObjectPoolManager.Spawn(projectilePrefab, firepoint.position, firepoint.rotation).GetComponent<IProjectile>();
        if (proj!=null){
            proj.SetUpProjectile(1f, firepoint.transform.up, projectileSpeed, projectileLifeTime, projectileBlockCount, gameObject);
        }
    }

    public void SetCanBeHurt(bool isHurtable) {
        canBeHurt = isHurtable;

    
        if (shield) shield.SetActive(!isHurtable);
    
    }
    
    public void OnDamage(float dmg, Vector2 kBackDir, float kBackMag, GameObject attacker)
    {
        if (attacker != gameObject&&!isHurt&&canBeHurt)
        {
            if (flashVFX) flashVFX.Flash();
            isHurt = true;
            currHealth -= dmg;
            healthUI.DoHurtUpdate(currHealth);

            if (currHealth <= 0)
            {
                DeactivateTurret();
            }
        }
    }
}

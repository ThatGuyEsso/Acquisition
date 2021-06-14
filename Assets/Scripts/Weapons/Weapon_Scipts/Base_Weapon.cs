using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WeaponType
{
    Sword,
    Bow,
    Staff,
    none
};
public class Base_Weapon : MonoBehaviour, IInitialisable, Equipable
{
    
    [SerializeField] protected WeaponType weaponType;
    protected Transform firePoint;
    protected Transform playerTransform;
    protected AttackAnimEventListener attackEvents;
    protected TopPlayerGFXSolver animSolver;
    [Header("Attack Settings")]
    [SerializeField] protected float primaryAttackDamage;
    [SerializeField] protected float secondaryAttackDamage;


    [Header("Projectiles")]
    [SerializeField] protected GameObject primaryProjectile;
    [SerializeField] protected GameObject secondaryProjectile;
    [SerializeField] protected float primaryProjectileDamage;
    [SerializeField] protected float secondaryProjectileDamage;

    [Header("TimeFireFrate")]
    [SerializeField] protected float primaryFireRate = 1.0f;
    [SerializeField] protected float secondaryFireRate = 1.0f;
    [SerializeField] protected float timeToIdle=3f;

    [Header("Debug")]
    [SerializeField] private bool inDebug = false;

    protected Controls inputAction;
    private BoxCollider2D boxCollider;

    protected bool isWeaponActive = false;
    protected bool canPrimaryFire = false;
    protected bool canSecondaryFire = false;

    protected bool isFiringPrimary = false;
    protected bool isFiringSecondary = false;
    protected bool isInitialised;
    protected bool isBusy;
    protected bool isRunning;
    protected bool isIdle = true;
    protected float currTimeToIdle;
    private void Awake()
    {
        if(inDebug) 
            Init();
    }

    public virtual void Init()
    {
        inputAction = new Controls();
        inputAction.Enable();
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        SetCanFire(false);
        isInitialised=true;
    }

    protected virtual void PrimaryAttack()
    {
        Debug.Log("PrimaryAttack");
        StartCoroutine(WaitForFirePrimaryRate(primaryFireRate));
    }

    protected virtual void SecondaryAttack()
    {
        Debug.Log("SecondAttack");
        StartCoroutine(WaitForFirePrimaryRate(secondaryFireRate));
    }

    protected IEnumerator WaitForFirePrimaryRate(float time)
    {
        canPrimaryFire = false;
        yield return new WaitForSeconds(time);
        canPrimaryFire = true;
    }

    protected IEnumerator WaitForFireSecondaryRate(float time)
    {
        canSecondaryFire = false;
        yield return new WaitForSeconds(time);
        canSecondaryFire = true;
    }

    protected void SetCanFire(bool status)
    {
        isWeaponActive = status;

        if (status == false)
        {
            canPrimaryFire = false;
            canSecondaryFire = false;
            
        }
        else if (status == true)
        {
            canPrimaryFire = true;
            canSecondaryFire = true;
            
        }
    }

    public virtual Base_Weapon GetWeapon()
    {
        return this;
    }

    public virtual WeaponType GetWeaponType()
    {
        return weaponType;
    }

    public void SetSlot(WeaponSlots slot)
    {
        transform.parent = slot.transform;
        transform.position = slot.GetSlotPos();
        transform.localRotation = Quaternion.identity;
        SetCanFire(true);
        boxCollider.enabled = false;
    }

    virtual public void OnFireProjectile()
    {

    }

    virtual public void Equip(Transform firePoint, AttackAnimEventListener eventListener,Transform player, TopPlayerGFXSolver solver)
    {
        if (!isInitialised) 
        {
            Init();
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

    }

    virtual public void UnEquip()
    {
        inputAction.Disable();
 
        SetCanFire(false);
        animSolver.movement.OnWalk -= OnRun;
        animSolver.movement.OnStop -= OnStop;
    }


    virtual public void OnRun()
    {
        isRunning = true;
    }

    virtual public void OnStop()
    {
        isRunning = false;
    }

    virtual public void ResetPrimaryFire()
    {
        isBusy = false;
        canPrimaryFire = true;
        attackEvents.OnAnimEnd -= ResetPrimaryFire;
    }

    virtual public void ResetSecondaryFire()
    {
        attackEvents.OnAnimEnd -= ResetSecondaryFire;
        isBusy = false;
       
    }
}

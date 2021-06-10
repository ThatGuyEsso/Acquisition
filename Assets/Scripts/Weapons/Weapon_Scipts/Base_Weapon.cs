using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_Weapon : MonoBehaviour, IInitialisable, Equipable
{
    
    [SerializeField] protected string weaponName;
    [SerializeField] protected GameObject firePoint;

    [Header("Projectiles")]
    [SerializeField] protected Base_Projectile primaryProjectile;
    [SerializeField] protected Base_Projectile secondaryProjectile;

    [Header("TimeFireFrate")]
    [SerializeField] protected float primaryFireRate = 1.0f;
    [SerializeField] protected float secondaryFireRate = 1.0f;

    [Header("Debug")]
    [SerializeField] private bool inDebug = false;

    private Controls inputAction;
    private BoxCollider2D boxCollider;

    protected bool isWeaponActive = false;
    protected bool canPrimaryFire = false;
    protected bool canSecondaryFire = false;

    protected bool isFiringPrimary = false;
    protected bool isFiringSecondary = false;


    private void Awake()
    {
        if(inDebug) 
            Init();
    }

    public virtual void Init()
    {
        inputAction = new Controls();
        inputAction.Enable();
        inputAction.Attack.PrimaryAttack.performed += ctx => PrimaryAttack();
        inputAction.Attack.SecondaryAttack.performed += ctx => SecondaryAttack();
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        SetCanFire(false);
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

    public virtual string GetWeaponName()
    {
        return weaponName;
    }

    public void SetSlot(WeaponSlots slot)
    {
        transform.parent = slot.transform;
        transform.position = slot.GetSlotPos();
        transform.localRotation = Quaternion.identity;
        SetCanFire(true);
        boxCollider.enabled = false;
    }


}

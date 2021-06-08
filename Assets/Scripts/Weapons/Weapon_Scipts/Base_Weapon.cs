using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_Weapon : MonoBehaviour, IInitialisable, Equipable
{
    [SerializeField] protected string weaponName;
    [SerializeField] protected GameObject firePoint;
    [SerializeField] protected WeaponSettings settings;
    [SerializeField] private bool inDebug = false;

    private Controls inputAction;
    private BoxCollider2D boxCollider;

    protected bool canFire = false;

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
    }

    protected virtual void PrimaryAttack()
    {
        Debug.Log("PrimaryAttack");
        StartCoroutine(WaitForFireRate(settings.primaryAttackTime));
    }

    protected virtual void SecondaryAttack()
    {
        Debug.Log("SecondAttack");
        StartCoroutine(WaitForFireRate(settings.secondaryAttackTime));
    }

    protected IEnumerator WaitForFireRate(float time)
    {
        canFire = false;
        yield return new WaitForSeconds(time);
        canFire = true;
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
        canFire = true;
        boxCollider.enabled = false;
    }


}

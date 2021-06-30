using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
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
    [SerializeField] protected List<Base_SkillAttribute> attributes = new List<Base_SkillAttribute>();

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
    protected bool primaryHeld = false;
    protected bool secondaryHeld = false;

    //Abillity actions
    public Action OnPrimaryAttack;
    public Action<GameObject> OnPrimaryAbility;

    public Action OnSecondaryAttack;
    public Action<GameObject> OnSecondaryAbility;

    private void Awake()
    {
        if(inDebug) 
            Init();
    }


    virtual protected void OnDisable()
    {
        if (isInitialised)
        {
            if (GameManager.instance) GameManager.instance.OnNewEvent -= EvaluateNewGameEvent;
            DisableWeapon();

        }
    }



    protected void EvaluateNewGameEvent(GameEvents newEvent)
    {
        switch (newEvent)
        {
     
            case GameEvents.PlayerDefeat:
          
                break;

            case GameEvents.ExitGame:
                ObjectPoolManager.Recycle(gameObject);
                break;


        }
    }
    public virtual void Init()
    {
        if (GameManager.instance) GameManager.instance.OnNewEvent += EvaluateNewGameEvent;
        inputAction = new Controls();
        inputAction.Enable();
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        SetCanFire(false);
        isInitialised=true;
        DontDestroyOnLoad(gameObject);
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

    virtual protected IEnumerator WaitForFireSecondaryRate(float time)
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
        primaryHeld = false;
        secondaryHeld = false;
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

    virtual public void DisableWeapon()
    {
        if (inputAction != null) inputAction.Disable();
        primaryHeld = false;
        secondaryHeld = false;
        isWeaponActive = false;
        isBusy = false;
    }

    virtual public void EnableWeapon()
    {
        if (inputAction != null) inputAction.Enable();
        isWeaponActive = true;
        isBusy = false;
    }

    protected void OnPrimaryHeld()
    {
        primaryHeld = true;
    }

    protected void OnPrimaryReleased()
    {
        primaryHeld = false;
    }
    protected void OnSecondaryHeld()
    {
        secondaryHeld = true;
    }

    protected void OnSecondaryReleased()
    {
        secondaryHeld = false;
    }

    public void Delete()
    {
        UnEquip();
        if (attributes.Count > 0)
        {
            foreach(Base_SkillAttribute attrib in attributes)
            {
                if (attrib)
                    ObjectPoolManager.Recycle(attrib.gameObject);
            }
            attributes.Clear();
        }
        if (gameObject)
            ObjectPoolManager.Recycle(gameObject);
    }

    public void AddSkillAttribute(Base_SkillAttribute attribute)
    {
        Base_SkillAttribute attrib = ObjectPoolManager.Spawn(attribute, transform);
        if (attrib)
        {
            attrib.SetUpAttribute(this);
            attributes.Add(attrib);
            
        }
   
    }

    public Transform GetPlayerTransform() { return playerTransform; }
    public Transform GetFirePoint() { return firePoint ; }

    public GameObject GetPrimaryProjectilePrefab() { return primaryProjectile; }
    public GameObject GetSecondaryProjectile() { return secondaryProjectile; }

    public void SetPrimaryProjectilePrefab(GameObject primPrefab) {  primaryProjectile = primPrefab; }
    public void SetSecondaryProjectile(GameObject secPrefab) { secondaryProjectile = secPrefab; }
}

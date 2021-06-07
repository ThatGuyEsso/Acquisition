using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_Weapon : MonoBehaviour, IInitialisable
{
    private Controls inputAction;
    [SerializeField] private GameObject WeaponEquiptPoint;
    [SerializeField] protected WeaponSettings Settings;
    [SerializeField] private bool inDebug = false;

    private void Awake()
    {
        if(inDebug) 
            Init();
    }

    public virtual void Init()
    {
        inputAction = new Controls();
        inputAction.Enable();
        inputAction.Attack.PrimaryAttack.performed += ctx => Invoke("PrimaryAttack", Settings.primaryAttackTime);
        inputAction.Attack.SecondaryAttack.performed += ctx => Invoke("SecondaryAttack", Settings.secondaryAttackTime);


    }

    protected virtual void PrimaryAttack()
    {
        Debug.Log("PrimaryAttack");
    }

    protected virtual void SecondaryAttack()
    {
        Debug.Log("SecondAttack");
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "player")
        {
            Debug.Log("Equipt");
        }
    }


}

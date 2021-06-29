using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoenixGrace : Base_SkillAttribute
{

   
    private PlayerHealth healthController;

    [SerializeField] private GameObject activateVFX;


    public override void SetUpAttribute(Base_Weapon weaponOwner)
    {
        base.SetUpAttribute(weaponOwner);

        GameObject playerObject = weaponOwner.GetPlayerTransform().gameObject;

        if (playerObject)
        {

            healthController = playerObject.GetComponent<PlayerHealth>();

            if (healthController)
            {
                healthController.OnHurt += ActivatePhoenixGrace;
                if (GameManager.instance)
                {
                    GameManager.instance.OnNewEvent += EvaluateNewGameEvent;
                }
            }
        }

      

    }



    virtual protected void EvaluateNewGameEvent(GameEvents newEvent)
    {
        if(newEvent == GameEvents.BossDefeated)
        { 
                EnablePhoenixGrace();
           
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (!healthController) return;
        healthController.OnHurt -= ActivatePhoenixGrace;

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (!healthController) return;
        healthController.OnHurt += ActivatePhoenixGrace;

    }
    public void EnablePhoenixGrace()
    {
        if (!healthController) return;
        healthController.OnHurt += ActivatePhoenixGrace;
    }


    public void ActivatePhoenixGrace()
    {
        if (!healthController) return;
        healthController.OnHurt -= ActivatePhoenixGrace;

        healthController.ResetComponent();
        if (activateVFX) ObjectPoolManager.Spawn(activateVFX, healthController.transform.position, Quaternion.identity);
    }
}

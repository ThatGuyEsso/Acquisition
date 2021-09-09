using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Base_BossBuff : MonoBehaviour
{
    protected BaseBossAI owner;

    [SerializeField] protected GameObject abilityToBuffPrefab;
    protected BaseBossAbility abilityToBuff;
    protected void EvaluateNewGameEvent(GameEvents newEvent)
    {
        switch (newEvent)
        {
          
            case GameEvents.PlayerDefeat:
                if (abilityToBuff)
                    UnbindFromAbility(abilityToBuff);
                if (gameObject)
                    ObjectPoolManager.Recycle(gameObject);
                break;
            
            
     
   
               
            case GameEvents.BossDefeated:
                if (abilityToBuff)
                    UnbindFromAbility(abilityToBuff);
                if (gameObject)
                    ObjectPoolManager.Recycle(gameObject);
                break;
         
        }
    }
    virtual public void InitBuff(BaseBossAI boss)
    {
        owner = boss;
        boss.OnAbilityAdded += BindToAbility;
        boss.OnAbilityRemoved += UnbindFromAbility;
        GameManager.instance.OnNewEvent += EvaluateNewGameEvent;
    } 


    virtual protected void BindToAbility(BaseBossAbility ability)
    {
        if (ability.name == string.Format("{0}(Clone)", abilityToBuffPrefab.name)) {
            abilityToBuff = ability;
            
        }
    }

    virtual protected void UnbindFromAbility(BaseBossAbility ability)
    {
        if (ability.name == string.Format("{0}(Clone)", abilityToBuffPrefab.name))
        {
            abilityToBuff = null;

        }
    }


    private void OnDisable()
    {
        owner.OnAbilityAdded -= BindToAbility;
        owner.OnAbilityRemoved -= UnbindFromAbility;
        GameManager.instance.OnNewEvent -= EvaluateNewGameEvent;
    }
}

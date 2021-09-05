using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Base_BossBuff : MonoBehaviour
{
    protected BaseBossAI owner;

    [SerializeField] protected GameObject abilityToBuffPrefab;
    protected BaseBossAbility abilityToBuff;

    virtual public void InitBuff(BaseBossAI boss)
    {
        owner = boss;
        boss.OnAbilityAdded += BindToAbility;
        boss.OnAbilityRemoved += UnbindFromAbility;
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
    }
}

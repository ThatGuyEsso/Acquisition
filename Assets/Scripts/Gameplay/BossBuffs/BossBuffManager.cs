using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBuffManager : MonoBehaviour, IInitialisable
{
    [SerializeField] private GameObject swordKingBuff;
    [SerializeField] private GameObject elderBuff;
    [SerializeField] private GameObject scholarBuff;

    [SerializeField] private RunTimeData runtimeData;
    private List<Base_BossBuff> currentBuffs = new List<Base_BossBuff>();
    public void Init()
    {
        if (runtimeData)
        {
            if (runtimeData.isKnightDefeated)
            {
                Base_BossBuff buff =null;
                if (swordKingBuff)
                {
                    buff = ObjectPoolManager.Spawn(swordKingBuff, transform).GetComponent<Base_BossBuff>();
                }
                BaseBossAI owner = GetComponent<BaseBossAI>();
                if (owner && buff)
                {
                    buff.InitBuff(owner);
                    currentBuffs.Add(buff);
                }

            }

            if (runtimeData.isScholarDefeated)
            {
                Base_BossBuff buff = null;
                if (scholarBuff)
                {
                    buff = ObjectPoolManager.Spawn(scholarBuff, transform).GetComponent<Base_BossBuff>();
                }
                BaseBossAI owner = GetComponent<BaseBossAI>();
                if (owner && buff)
                {
                    buff.InitBuff(owner);
                    currentBuffs.Add(buff);
                }
            }

            if (runtimeData.isElderDefeated)
            {
                Base_BossBuff buff = null;
                if (elderBuff)
                {
                    buff = ObjectPoolManager.Spawn(elderBuff, transform).GetComponent<Base_BossBuff>();
                }
                BaseBossAI owner = GetComponent<BaseBossAI>();
                if (owner && buff)
                {
                    buff.InitBuff(owner);
                    currentBuffs.Add(buff);
                }
            }
        }
        else
        {
            Debug.LogError("No Boss buff RunTime Data");
        }
       
    }

    private void OnDestroy()
    {
        foreach(Base_BossBuff buff in currentBuffs)
        {
            if (buff) ObjectPoolManager.Recycle(buff);
        }
        currentBuffs.Clear();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Boss Stages")]

public class BossStageData : ScriptableObject
{
    [SerializeField] public List<AttackPatternData> initialStageAbilities = new List<AttackPatternData>();
    [SerializeField] public List<AttackPatternData> middleStageAbilities = new List<AttackPatternData>();
    [SerializeField] public List<AttackPatternData> finalStageAbiilities = new List<AttackPatternData>();

   
}

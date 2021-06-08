using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Boss Stages")]
public class BossStageData : ScriptableObject
{
    [SerializeField] private List<GameObject> initialStageAbilities = new List<GameObject>();
    [SerializeField] private List<GameObject> middleStageAbilities = new List<GameObject>();
    [SerializeField] private List<GameObject> finalStageAbiilities = new List<GameObject>();

    
}

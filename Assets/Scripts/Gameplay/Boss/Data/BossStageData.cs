using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Boss Stages")]
public class BossStageData : ScriptableObject
{
    [SerializeField] public List<GameObject> initialStageAbilities = new List<GameObject>();
    [SerializeField] public List<GameObject> middleStageAbilities = new List<GameObject>();
    [SerializeField] public List<GameObject> finalStageAbiilities = new List<GameObject>();

   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossType
{
    Knight,
    Elder,
    Scholar
};
public class BossDoor : MonoBehaviour
{
    [SerializeField] private GameObject doorMode, wallMode;
    [SerializeField] private BossType bossDoorType;
    public Transform corridorSpawn;

    bool isDoor;
    

    public void SetIsDoor(bool isDoor)
    {
        this.isDoor = isDoor;
        if (isDoor)
        {
            doorMode.gameObject.SetActive(true);
            wallMode.SetActive(false);
        }
        else
        {
            doorMode.gameObject.SetActive(false);
            wallMode.SetActive(true);
        }
    }

    public void OpenDoors()
    {

    }

    public void CloseDoors()
    {

    }
}

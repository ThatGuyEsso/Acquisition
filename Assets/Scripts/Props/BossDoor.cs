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
    [SerializeField] private GameObject spawnVFX;
    [SerializeField] private SlidingDoor bossDoor;
    bool isDoor;
    public string corridorID;

    public void SetIsDoor(bool isDoor)
    {
        this.isDoor = isDoor;
        if (isDoor)
        {
            doorMode.gameObject.SetActive(true);
            wallMode.SetActive(false);
            CamShake.instance.DoScreenShake(0.15f, 2f, 0.1f, 2f, 2f);
            ObjectPoolManager.Spawn(spawnVFX, doorMode.transform.position, Quaternion.identity);
            bossDoor.SetUpDoor();
        }
        else
        {
            doorMode.gameObject.SetActive(false);
            wallMode.SetActive(true);
        }
    }

    public void OpenDoors()
    {
        bossDoor.BeginToOpen();
    }

    public void CloseDoors()
    {
        bossDoor.BeginToClose();
    }
}

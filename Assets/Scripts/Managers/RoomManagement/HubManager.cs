using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    [Header("Room Components")]
    [SerializeField] private BossDoor knightDoor, elderDoor, scholarDoor;
    [SerializeField] private WeaponSpawner[] weaponSpawners;

    [Header("Data")]
    [SerializeField] private RunTimeData runTimeData;
    bool hasTriggered;
    bool isLoadingRoom;
    bool isBound;

    private void Awake()
    {
        BindToGameStateManager();
    }
    public void BindToGameStateManager()
    {
        GameStateManager.instance.OnNewGameState += EvaluateGameState;
        isBound = true;
    }

    [SerializeField] private bool inDebug = false;
    public void EvaluateGameState(GameState newState)
    {
        switch (newState)
        {
            case GameState.HubWorldLoadComplete:
                GameManager.instance.OnNewEvent += EvaluateNewGameEvent;
                break;
        }

    }
    public void SetUpBossDoors()
    {
        StartCoroutine(EvaluateRunTimeData());
    }


    private void EvaluateNewGameEvent(GameEvents newEvent)
    {
        switch (newEvent)
        {
            case GameEvents.WeaponPicked:
                SetUpBossDoors();
                    
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")&&!hasTriggered){
            Debug.Log("Player seen ");
            if (runTimeData.hasWeapon == false)
            {
                Debug.Log("Player has no weapons");
                SpawnWeapons();
                GameManager.instance.BeginNewEvent(GameEvents.WeaponsSpawned);
                hasTriggered = true;
            }
            else
            {
            
                hasTriggered = true;

            }
        }
    }



    public IEnumerator EvaluateRunTimeData()
    {

        if (!runTimeData.isKnightDefeated)
        {
            knightDoor.SetIsDoor(true);
            isLoadingRoom = true;
            RoomManager.instance.OnNewRoomAdded += OnRoomLoadComplete;
            RoomManager.instance.BeginLoadInNewSceneAt(knightDoor.corridorSpawn.position, SceneIndex.BasicCorridor);
            while (isLoadingRoom)
            {
                yield return null;
            }
        }
        else
        {
            knightDoor.SetIsDoor(false);
        }
        if (!runTimeData.isElderDefeated)
        {
            elderDoor.SetIsDoor(true);
            isLoadingRoom = true;
            RoomManager.instance.OnNewRoomAdded += OnRoomLoadComplete;
            RoomManager.instance.BeginLoadInNewSceneAt(elderDoor.corridorSpawn.position, SceneIndex.BasicCorridor);
            while (isLoadingRoom)
            {
                yield return null;
            }
        }
        else
        {
            elderDoor.SetIsDoor(false);
        }
        if (!runTimeData.isScholarDefeated)
        {
            scholarDoor.SetIsDoor(true);
            isLoadingRoom = true;
            RoomManager.instance.OnNewRoomAdded += OnRoomLoadComplete;
            RoomManager.instance.BeginLoadInNewSceneAt(scholarDoor.corridorSpawn.position, SceneIndex.BasicCorridor);
            while (isLoadingRoom)
            {
                yield return null;
            }
        }
        else
        {
            scholarDoor.SetIsDoor(false);
        }

        GameManager.instance.BeginNewEvent(GameEvents.BossRoomsSpawned);
    }

    public void SpawnWeapons()
    {
      
    
        if (weaponSpawners.Length > 0)
        {
            foreach (WeaponSpawner spawner in weaponSpawners)
            {
                spawner.SpawnWeapon();
                spawner.OnWeaponReplaced += EvaluateWeaponReplaced;
            }
        }

       
    }


    public void EvaluateWeaponReplaced(WeaponType weaponType)
    {

        for(int i=0; i< weaponSpawners.Length; i++)
        {
            if( weaponSpawners[i].GetWeaponType()== weaponType)
            {
                weaponSpawners[i].ToggleWeaponAvailable(true);
            }
        }

    }

    public void OnRoomLoadComplete()
    {
        isLoadingRoom = false;
        RoomManager.instance.OnNewRoomAdded -= OnRoomLoadComplete;
    }

    private void OnDestroy()
    {
        GameStateManager.instance.OnNewGameState -= EvaluateGameState;
        GameManager.instance.OnNewEvent -= EvaluateNewGameEvent;
    }
}
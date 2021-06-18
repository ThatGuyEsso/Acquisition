using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    public static HubManager instance;
    [Header("Room Components")]
    [SerializeField] private BossDoor knightDoor, elderDoor, scholarDoor;
    [SerializeField] private WeaponSpawner[] weaponSpawners;

    [Header("Data")]
    [SerializeField] private RunTimeData runTimeData;
    [SerializeField] private float spawnDelay =0.5f;
    bool hasTriggered;
    bool bossRoomsSpawned;
    bool isLoadingRoom;
    bool isBound;


    private void Awake()
    {
        BindToGameStateManager();

        if(instance == false)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        //As world is started and player enters game audio is playerd
        if (AudioManager.instance)
            AudioManager.instance.PlayThroughAudioPlayer("DungeonBoom", transform.position);
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
        if (!bossRoomsSpawned)
        {

            StartCoroutine(EvaluateRunTimeData());
        }
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

    public void BeginBossRoomLoad(BossType boss)
    {
        StartCoroutine(LoadSelectedBossRoom(boss));
    }

    private IEnumerator LoadSelectedBossRoom(BossType boss)
    {
        BossRoomManager manager;
        LevelRoom connectingCorridor;
        switch (boss)
        {
            case BossType.Knight:
                scholarDoor.SetIsDoor(false);
                elderDoor.SetIsDoor(false);
           
                
                RoomManager.instance.RemoveRoom(scholarDoor.corridorID);
                RoomManager.instance.RemoveRoom(elderDoor.corridorID);
                connectingCorridor = RoomManager.instance.GetRoom(knightDoor.corridorID);

                isLoadingRoom = true;
                RoomManager.instance.OnNewRoomAdded += OnRoomLoadComplete;

                RoomManager.instance.BeginLoadInNewSceneAt(connectingCorridor.GetConnectionPoint().position,
                    SceneIndex.BossRoomKnight);

                while (isLoadingRoom) yield return null;

                manager= FindObjectOfType<BossRoomManager>();
                if (manager) manager.Init();
                break;
            case BossType.Elder:
                knightDoor.SetIsDoor(false);
                scholarDoor.SetIsDoor(false);
                RoomManager.instance.RemoveRoom(scholarDoor.corridorID);
                RoomManager.instance.RemoveRoom(knightDoor.corridorID);

                connectingCorridor = RoomManager.instance.GetRoom(elderDoor.corridorID);

                isLoadingRoom = true;
                RoomManager.instance.OnNewRoomAdded += OnRoomLoadComplete;

                RoomManager.instance.BeginLoadInNewSceneAt(connectingCorridor.GetConnectionPoint().position,
                    SceneIndex.BossRoomElder);

                while (isLoadingRoom) yield return null;

                manager = FindObjectOfType<BossRoomManager>();
                if (manager) manager.Init();
                break;
            case BossType.Scholar:
                knightDoor.SetIsDoor(false);
                elderDoor.SetIsDoor(false);
                RoomManager.instance.RemoveRoom(elderDoor.corridorID);
                RoomManager.instance.RemoveRoom(knightDoor.corridorID);

                connectingCorridor = RoomManager.instance.GetRoom(scholarDoor.corridorID);

                isLoadingRoom = true;
                RoomManager.instance.OnNewRoomAdded += OnRoomLoadComplete;

                RoomManager.instance.BeginLoadInNewSceneAt(connectingCorridor.GetConnectionPoint().position,
                    SceneIndex.BossRoomScholar);


                while (isLoadingRoom) yield return null;

                manager = FindObjectOfType<BossRoomManager>();
                if (manager) manager.Init();
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
                StartCoroutine(SpawnWeapons());
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
        bossRoomsSpawned = true;
        if (!runTimeData.isKnightDefeated)
        {
            knightDoor.SetIsDoor(true);
            isLoadingRoom = true;
            RoomManager.instance.OnNewRoomAdded += OnRoomLoadComplete;
            RoomManager.instance.BeginLoadInNewSceneAt(knightDoor.corridorSpawn.position, SceneIndex.LongCorridor);

            while (isLoadingRoom)
            {
                yield return null;
            }
            knightDoor.corridorID = RoomManager.instance.loadedRooms[RoomManager.instance.loadedRooms.Count - 1].ID();
            AudioManager.instance.PlayThroughAudioPlayer("RoomSpawn", knightDoor.gameObject.transform.position); //plays sound when doors are opened
        }
        else
        {
            knightDoor.SetIsDoor(false);
        }

        yield return new WaitForSeconds(spawnDelay);
        if (!runTimeData.isElderDefeated)
        {
            elderDoor.SetIsDoor(true);
            isLoadingRoom = true;
            RoomManager.instance.OnNewRoomAdded += OnRoomLoadComplete;
            RoomManager.instance.BeginLoadInNewSceneAt(elderDoor.corridorSpawn.position, SceneIndex.LongCorridor);
            while (isLoadingRoom)
            {
                yield return null;
            }
            elderDoor.corridorID = RoomManager.instance.loadedRooms[RoomManager.instance.loadedRooms.Count - 1].ID();
            AudioManager.instance.PlayThroughAudioPlayer("RoomSpawn", elderDoor.gameObject.transform.position); //plays sound when doors are opened
        }
        else
        {
            elderDoor.SetIsDoor(false);
        }
        yield return new WaitForSeconds(spawnDelay);
        if (!runTimeData.isScholarDefeated)
        {
            scholarDoor.SetIsDoor(true);
            isLoadingRoom = true;
            RoomManager.instance.OnNewRoomAdded += OnRoomLoadComplete;
            RoomManager.instance.BeginLoadInNewSceneAt(scholarDoor.corridorSpawn.position, SceneIndex.LongCorridor);
            while (isLoadingRoom)
            {
                yield return null;
            }
            scholarDoor.corridorID = RoomManager.instance.loadedRooms[RoomManager.instance.loadedRooms.Count - 1].ID();
            AudioManager.instance.PlayThroughAudioPlayer("RoomSpawn", scholarDoor.gameObject.transform.position); //plays sound when doors are opened
        }
        else
        {
            scholarDoor.SetIsDoor(false);
        }

        GameManager.instance.BeginNewEvent(GameEvents.BossRoomsSpawned);
    }

    public IEnumerator SpawnWeapons()
    {
        if (weaponSpawners.Length > 0)
        {
            foreach (WeaponSpawner spawner in weaponSpawners)
            {
                spawner.SpawnWeapon();
                spawner.OnWeaponReplaced += EvaluateWeaponReplaced;
                yield return new WaitForSeconds(spawnDelay);
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
        if (weaponSpawners.Length > 0)
        {
            foreach (WeaponSpawner spawner in weaponSpawners)
            {
                spawner.OnWeaponReplaced -= EvaluateWeaponReplaced;
            }
        }
    }
}
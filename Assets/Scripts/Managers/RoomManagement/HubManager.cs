using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    public static HubManager instance;
    [Header("Room Components")]
    [SerializeField] private BossDoor knightDoor, elderDoor, scholarDoor;
    [SerializeField] private WeaponSpawner[] weaponSpawners;
    [SerializeField] private RoomDoor tutorialDoor;
    [Header("Data")]
    [SerializeField] private RunTimeData runTimeData;
    [SerializeField] private float spawnDelay =0.5f;
    bool hasTriggered;
    bool bossRoomsSpawned;
    bool isLoadingRoom;
    bool isBound;
    private string tutorialCorridorID;

    private void Awake()
    {
        BindToGameStateManager();

        if(instance == false)
        {
            instance = this;
            if (tutorialDoor) tutorialDoor.gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
        
        //As world is started and player enters game audio is playerd
        if (AudioManager.instance)
        {
            AudioManager.instance.PlayThroughAudioPlayer("DungeonBoom", transform.position);
         
        }
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
            if (tutorialDoor.gameObject.activeInHierarchy)
            {
                elderDoor.gameObject.SetActive(true);
                StartCoroutine(RemoveTutorialDoor());
            }
            StartCoroutine(EvaluateRunTimeData());
        }
  
    }

    public void SetUpTutorialDoor()
    {
       
        if (tutorialDoor)
        {
            if (!tutorialDoor.gameObject.activeInHierarchy)
            {
                if (bossRoomsSpawned) BeginRemoveBossDoors();
                elderDoor.gameObject.SetActive(false);
                tutorialDoor.gameObject.SetActive(true);
                tutorialDoor.Init();
                StartCoroutine(SetUpTutorialDoorPath());
               
            }
        }
    }

    private void EvaluateNewGameEvent(GameEvents newEvent)
    {
        switch (newEvent)
        {
            case GameEvents.WeaponPicked:
                EvaluateWeaponPicked();

                if (!TutorialManager.instance.tutorialdata.isStaffTutorialComplete)
                {
                    WeaponManager.instance.OnWeaponEquipped += EvaluateNewWeaponPicked;
                }
                break;
        }
    }

    public void BeginBossRoomLoad(BossType boss)
    {
        StartCoroutine(LoadSelectedBossRoom(boss));
    }

    public void BeginTutorialRoomLoad( )
    {
        StartCoroutine(LoadTutorialRoom());
    }
    private IEnumerator LoadTutorialRoom()
    {
        isLoadingRoom = true;
        RoomManager.instance.OnNewRoomAdded += OnRoomLoadComplete;
        LevelRoom connectingCorridor = RoomManager.instance.GetRoom(tutorialCorridorID);
        RoomManager.instance.BeginLoadInNewSceneAt(connectingCorridor.GetConnectionPoint().position,
            SceneIndex.TutorialRoom);

        while (isLoadingRoom) yield return null;
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

                if (scholarDoor.corridorID != string.Empty)
                    RoomManager.instance.RemoveRoom(scholarDoor.corridorID);
                if (elderDoor.corridorID != string.Empty)
                    RoomManager.instance.RemoveRoom(elderDoor.corridorID);
                connectingCorridor = RoomManager.instance.GetRoom(knightDoor.corridorID);

                isLoadingRoom = true;
                RoomManager.instance.OnNewRoomAdded += OnRoomLoadComplete;

                RoomManager.instance.BeginLoadInNewSceneAt(connectingCorridor.GetConnectionPoint().position,
                    SceneIndex.BossRoomKnight);

                while (isLoadingRoom) yield return null;

                manager= FindObjectOfType<BossRoomManager>();
                if (manager) manager.SetUpDoors();
                break;
            case BossType.Elder:
                knightDoor.SetIsDoor(false);
                scholarDoor.SetIsDoor(false);
                if (scholarDoor.corridorID != string.Empty) 
                RoomManager.instance.RemoveRoom(scholarDoor.corridorID);
                if (knightDoor.corridorID != string.Empty) 
                RoomManager.instance.RemoveRoom(knightDoor.corridorID);

                connectingCorridor = RoomManager.instance.GetRoom(elderDoor.corridorID);

                isLoadingRoom = true;
                RoomManager.instance.OnNewRoomAdded += OnRoomLoadComplete;

                RoomManager.instance.BeginLoadInNewSceneAt(connectingCorridor.GetConnectionPoint().position,
                    SceneIndex.BossRoomElder);

                while (isLoadingRoom) yield return null;

                manager = FindObjectOfType<BossRoomManager>();
                if (manager) manager.SetUpDoors();
                break;
            case BossType.Scholar:
                knightDoor.SetIsDoor(false);
                elderDoor.SetIsDoor(false);
                if (elderDoor.corridorID != string.Empty)
                    RoomManager.instance.RemoveRoom(elderDoor.corridorID);
                if (knightDoor.corridorID != string.Empty)
                    RoomManager.instance.RemoveRoom(knightDoor.corridorID);

                connectingCorridor = RoomManager.instance.GetRoom(scholarDoor.corridorID);

                isLoadingRoom = true;
                RoomManager.instance.OnNewRoomAdded += OnRoomLoadComplete;

                RoomManager.instance.BeginLoadInNewSceneAt(connectingCorridor.GetConnectionPoint().position,
                    SceneIndex.BossRoomScholar);


                while (isLoadingRoom) yield return null;

                manager = FindObjectOfType<BossRoomManager>();
                if (manager) manager.SetUpDoors();
                break;
        }
    }
    public void TriggerHub( )
    {
        if (MusicManager.instance) MusicManager.instance.BeginSongFadeIn("HubMelody", 0.5f, 10f, 20f);
        if (runTimeData.hasWeapon == false)
        {
            Debug.Log("Player has no weapons");
            StartCoroutine(SpawnWeapons());
            GameManager.instance.BeginNewEvent(GameEvents.WeaponsSpawned);

        }
        else
        {
            SetUpBossDoors();
        }
        if (BossRoomManager.instance)
        {
            RoomManager.instance.RemoveRoom(RoomType.BossRoom);
        }
     
    }


    public IEnumerator SetUpTutorialDoorPath()
    {
        
        isLoadingRoom = true;
        RoomManager.instance.OnNewRoomAdded += OnRoomLoadComplete;
        RoomManager.instance.BeginLoadInNewSceneAt(tutorialDoor.corridorSpawn.position, SceneIndex.LongCorridor);
        while (isLoadingRoom)
        {
            yield return null;
        }

        tutorialCorridorID = RoomManager.instance.loadedRooms[RoomManager.instance.loadedRooms.Count - 1].ID();
        AudioManager.instance.PlayThroughAudioPlayer("RoomSpawn", tutorialDoor.gameObject.transform.position); //plays sound when doors are opened
    }
    public IEnumerator EvaluateRunTimeData()
    {
        bossRoomsSpawned = true;
        if (!runTimeData.isKnightDefeated)
        {
            knightDoor.SetIsDoor(true);
            knightDoor.SetIsInteractable(false);
            isLoadingRoom = true;
            RoomManager.instance.OnNewRoomAdded += OnRoomLoadComplete;
            RoomManager.instance.BeginLoadInNewSceneAt(knightDoor.corridorSpawn.position, SceneIndex.LongCorridorLeft);

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
            elderDoor.SetIsInteractable(false);
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
            scholarDoor.SetIsInteractable(false);
            isLoadingRoom = true;
            RoomManager.instance.OnNewRoomAdded += OnRoomLoadComplete;
            RoomManager.instance.BeginLoadInNewSceneAt(scholarDoor.corridorSpawn.position, SceneIndex.LongCorridorRight);
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
        knightDoor.SetIsInteractable(knightDoor.GetIsDoor());
        elderDoor.SetIsInteractable(elderDoor.GetIsDoor());
        scholarDoor.SetIsInteractable(scholarDoor.GetIsDoor());
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
            foreach (WeaponSpawner spawner in weaponSpawners)
            {
                spawner.SetInteractactable(true);
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


    public void BeginRemoveBossDoors()
    {
        StopAllCoroutines();
        StartCoroutine(RemoveBossDoors());
    }

    public IEnumerator RemoveTutorialDoor()
    {
        if (tutorialDoor.gameObject.activeInHierarchy)
        {
            tutorialDoor.gameObject.SetActive(false);
            isLoadingRoom = true;
            RoomManager.instance.OnRoomRemoved += OnRoomLoadComplete;
            RoomManager.instance.BeginRemoveRoom(tutorialCorridorID);
            while (isLoadingRoom)
            {
                yield return null;
            }
        }
    }

    public IEnumerator RemoveBossDoors()
    {
        
        bossRoomsSpawned = false;
        if (knightDoor.gameObject.activeInHierarchy)
        {
            knightDoor.SetIsDoor(false);
            knightDoor.SetIsInteractable(false);
            isLoadingRoom = true;
            RoomManager.instance.OnRoomRemoved += OnRoomLoadComplete;
            RoomManager.instance.BeginRemoveRoom(knightDoor.corridorID);

            while (isLoadingRoom)
            {
                yield return null;
            }
        }
   
        if (elderDoor.gameObject.activeInHierarchy)
        {
            elderDoor.SetIsDoor(false);
            elderDoor.SetIsInteractable(false);
            isLoadingRoom = true;
            RoomManager.instance.OnRoomRemoved += OnRoomLoadComplete;
            RoomManager.instance.BeginRemoveRoom(elderDoor.corridorID);

            while (isLoadingRoom)
            {
                yield return null;
            }

        }
   
        if (scholarDoor.gameObject.activeInHierarchy)
        {
            scholarDoor.SetIsDoor(false);
            scholarDoor.SetIsInteractable(false);
            isLoadingRoom = true;
            RoomManager.instance.OnRoomRemoved += OnRoomLoadComplete;
            RoomManager.instance.BeginRemoveRoom(scholarDoor.corridorID);

            while (isLoadingRoom)
            {
                yield return null;
            }

        }

    }
    public void EvaluateWeaponPicked()
    {
        if (WeaponManager.instance)
        {
            switch (WeaponManager.instance.equippedWeapon.GetWeaponType())
            {
                case WeaponType.Sword:
                    if (TutorialManager.instance)
                    {
                        if(TutorialManager.instance.tutorialdata.isSwordTutorialComplete)
                        {
                            SetUpBossDoors();
                        }
                        else
                        {
                            SetUpTutorialDoor();
                        }
                    }
                    break;
                case WeaponType.Bow:
                    if (TutorialManager.instance)
                    {
                        if (TutorialManager.instance.tutorialdata.isBowTutorialComplete)
                        {
                            SetUpBossDoors();
                        }
                        else
                        {
                            SetUpTutorialDoor();
                        }
                    }
                    break;
                case WeaponType.Staff:
                    if (TutorialManager.instance)
                    {
                        if (TutorialManager.instance.tutorialdata.isStaffTutorialComplete)
                        {
                            SetUpBossDoors();
                        }
                        else
                        {
                            SetUpTutorialDoor();
                        }
                    }
                    break;
             
            }
        }
    }
    public void EvaluateNewWeaponPicked(WeaponType weapon)
    {
        if (WeaponManager.instance)
        {
            switch (weapon)
            {
                case WeaponType.Sword:
                    if (TutorialManager.instance)
                    {
                        if (TutorialManager.instance.tutorialdata.isSwordTutorialComplete)
                        {
                            SetUpBossDoors();
                        }
                        else
                        {
                            SetUpTutorialDoor();
                        }
                    }
                    break;
                case WeaponType.Bow:
                    if (TutorialManager.instance)
                    {
                        if (TutorialManager.instance.tutorialdata.isBowTutorialComplete)
                        {
                            SetUpBossDoors();
                        }
                        else
                        {
                            SetUpTutorialDoor();
                        }
                    }
                    break;
                case WeaponType.Staff:
                    if (TutorialManager.instance)
                    {
                        if (TutorialManager.instance.tutorialdata.isStaffTutorialComplete)
                        {
                            SetUpBossDoors();
                        }
                        else
                        {
                            SetUpTutorialDoor();
                        }
                    }
                    break;

            }
        }
    }
}
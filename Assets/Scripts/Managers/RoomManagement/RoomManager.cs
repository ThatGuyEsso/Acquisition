using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class RoomManager : MonoBehaviour, IInitialisable, IManager
{

    public static RoomManager instance;

    public SceneIndex[] startingRooms;

     public List<LevelRoom> loadedRooms = new List<LevelRoom>();
    private int roomsLoadedEverCount = 0;
    private bool isAddingRoom;
    private bool isClearingRoom;
    public Action OnNewRoomAdded;
    public Action OnAllRoomsCleared;
    public void BindToGameStateManager()
    {
        GameStateManager.instance.OnNewGameState += EvaluateGameState;
    }

    [SerializeField] private bool inDebug=false;
    public void EvaluateGameState(GameState newState)
    {
        switch (newState)
        {
            case GameState.TitleScreen:
                if (loadedRooms.Count > 0)
                {
                    BeginRoomCleanUp(SceneIndex.MainMenu);


                }
                break;
        }
    }


    public void Start  ()
    {
        if (inDebug)
        {
            if (SceneTransitionManager.instance)
            {
                Init();
                BeginStartingRoomsLoad();

            }
        }
    }

    public void Init()
    {
        if (instance == false)
        {
            instance = this;
            BindToGameStateManager();


        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

    }

    public void BeginStartingRoomsLoad()
    {
        if (startingRooms.Length > 0)
        {
            StopAllCoroutines();

            StartCoroutine(LoadStartingRooms());
        }
    }
    public IEnumerator RoomCleanUp(SceneIndex sceneToIgnore)
    {
        isClearingRoom = true;
        SceneTransitionManager.instance.OnSceneUnLoadComplete += OnRoomClearComplete;
        SceneTransitionManager.instance.BeginClearAllScenesNoLoad(sceneToIgnore);
        while (isClearingRoom)
        {
            yield return null;
        }

        loadedRooms.Clear();
    }
    public void BeginRoomCleanUp(SceneIndex sceneToIgnore)
    {
        StartCoroutine(RoomCleanUp(sceneToIgnore));
    }
    public void BeginLoadInNewSceneAt(Vector3 position,SceneIndex index)
    {

        StartCoroutine(LoadNewSceneAt(position,index));
    }

    private IEnumerator LoadNewSceneAt(Vector3 position, SceneIndex index)
    {
     
  
        isAddingRoom = true;
        SceneTransitionManager.instance.BeginSceneLoad(index);

        while (isAddingRoom)
        {
            yield return null;
        }
     
        loadedRooms[loadedRooms.Count-1].transform.position = position;
      
 
        OnNewRoomAdded?.Invoke();


    }

    public void BeginCreatePathBossToHub(Vector3 position)
    {
        StartCoroutine(CreatePathBossToHub(position));
    }

    public IEnumerator CreatePathBossToHub(Vector3 position)
    {
        isAddingRoom = true;
        SceneTransitionManager.instance.BeginSceneLoad(SceneIndex.LongCorridor);

        while (isAddingRoom)
        {
            yield return null;
        }

        LevelRoom corridor = loadedRooms[loadedRooms.Count - 1];
        corridor.transform.position = position;
        isAddingRoom = true;
        SceneTransitionManager.instance.BeginSceneLoad(SceneIndex.HubRoom);

        while (isAddingRoom)
        {
            yield return null;
        }

        loadedRooms[loadedRooms.Count - 1].transform.position = corridor.GetConnectionPoint().position;

        loadedRooms[loadedRooms.Count - 1].SetUpDoors();

    }


    public void BeginCreatePathToHub(Vector3 position, SceneIndex hub)
    {
        StartCoroutine(CreatePathHub(position, hub));
    }
    public IEnumerator CreatePathHub(Vector3 position,SceneIndex hub)
    {
        isAddingRoom = true;
        SceneTransitionManager.instance.BeginSceneLoad(SceneIndex.LongCorridor);

        while (isAddingRoom)
        {
            yield return null;
        }

        LevelRoom corridor = loadedRooms[loadedRooms.Count - 1];
        corridor.transform.position = position;
        isAddingRoom = true;
        SceneTransitionManager.instance.BeginSceneLoad(hub);

        while (isAddingRoom)
        {
            yield return null;
        }

        loadedRooms[loadedRooms.Count - 1].transform.position = corridor.GetConnectionPoint().position;
        loadedRooms[loadedRooms.Count - 1].SetUpDoors();
    }

    public void OnRoomClearComplete()
    {
        SceneTransitionManager.instance.OnSceneUnLoadComplete-= OnRoomClearComplete;
        isClearingRoom = false;
    }
    public void BeginClearAllRooms()
    {
        StartCoroutine(ClearAllRooms());
    }

    private IEnumerator ClearAllRooms()
    {
        isClearingRoom = true;
        SceneTransitionManager.instance.OnSceneUnLoadComplete += OnRoomClearComplete;
        SceneTransitionManager.instance.BeginClearAllScenes();
        while (isClearingRoom)
        {
            yield return null;
        }

        loadedRooms.Clear();
        OnAllRoomsCleared?.Invoke();
    }

    public void ClearAllRoomNotInSet(List<string> roomsToKeep)
    {

        List<string> roomsToRemove = new List<string>();


        foreach(LevelRoom room in loadedRooms)
        {
            bool addToClearList=true;

            for(int i= 0; i < roomsToKeep.Count; i++)
            {
                if(room.ID() == roomsToKeep[i])
                {
                    addToClearList = false;
                }

            }

            if (addToClearList)
            {
                roomsToRemove.Add(room.ID());
            }
        }


        foreach(string ID in roomsToRemove)
        {
            RemoveRoom(ID);
        }
    }


    public IEnumerator LoadStartingRooms()
    {
        for(int i=0; i < startingRooms.Length; i++)
        {
            isAddingRoom = true;
            SceneTransitionManager.instance.BeginSceneLoad(startingRooms[i]);

            while (isAddingRoom)
            {
                yield return null;
            }
            if(loadedRooms.Count == 1)
            {
                loadedRooms[i].transform.position = Vector3.zero;
                loadedRooms[i].SetUpDoors();
            }
            else
            {
                if (loadedRooms[i - 1].GetConnectionPoint())
                {
                    loadedRooms[i].transform.position = loadedRooms[i - 1].GetConnectionPoint().position;
                    loadedRooms[i - 1].connectedRoom = loadedRooms[i];
                    loadedRooms[i].SetUpDoors();
                }
            }

        }
        GameStateManager.instance.BeginNewState(GameState.HubWorldLoadComplete);

    }

    public void AddNewRoom(LevelRoom room)
    {
        if (room)
        {
            loadedRooms.Add(room);
            room.SetID("Room" + loadedRooms.Count+"_"+ room.GetRoomType().ToString()+ roomsLoadedEverCount.ToString());
            roomsLoadedEverCount++;
            isAddingRoom = false;
        }
    }

    public LevelRoom GetRoom(string roomID)
    {
        LevelRoom room = loadedRooms.Find(room => room.ID() == roomID);
        return room;
    }
    public void RemoveRoom(string roomID)
    {
        LevelRoom room =loadedRooms.Find(room => room.ID() == roomID);
        loadedRooms.Remove(room);
        SceneTransitionManager.instance.BeginSceneUnLoad(room.gameObject.scene);
    }

    public void RemoveRoom(RoomType type)
    {
        LevelRoom room = loadedRooms.Find(room => room.GetRoomType() == type);
        loadedRooms.Remove(room);
        SceneTransitionManager.instance.BeginSceneUnLoad(room.gameObject.scene);
    }


    public Transform GetSpawn()
    {
        LevelRoom spawnRoom = loadedRooms.Find(room => room.GetRoomType() == RoomType.SpawnRoom);
        if (spawnRoom) return spawnRoom.GetSpawnPosition();
        else return null;
    }
    public void OnLoadComplete()
    {

    }


}

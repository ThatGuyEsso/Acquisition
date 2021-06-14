using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class RoomManager : MonoBehaviour, IInitialisable, IManager
{

    public static RoomManager instance;

    public SceneIndex[] startingRooms;

     public List<LevelRoom> loadedRooms = new List<LevelRoom>();

    private bool isAddingRoom;

    public Action OnNewRoomAdded;
    public void BindToGameStateManager()
    {
        GameStateManager.instance.OnNewGameState += EvaluateGameState;
    }

    [SerializeField] private bool inDebug=false;
    public void EvaluateGameState(GameState newState)
    {
        //switch (newState)
        //{
            
        //}
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

           
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void BeginStartingRoomsLoad()
    {
        if (startingRooms.Length > 0)
        {
            StopAllCoroutines();

            StartCoroutine(LoadStartingRooms());
        }
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
            }
            else
            {
                if (loadedRooms[i - 1].GetConnectionPoint())
                {
                    loadedRooms[i].transform.position = loadedRooms[i - 1].GetConnectionPoint().position;
                    loadedRooms[i - 1].connectedRoom = loadedRooms[i];
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
            room.SetID("Room" + loadedRooms.Count);
            isAddingRoom = false;
        }
    }

    public void RemoveRoom(string roomID)
    {
        LevelRoom room =loadedRooms.Find(room => room.ID() == roomID);
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

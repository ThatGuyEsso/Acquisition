using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    SpawnRoom,
    BossRoom,
    HubRoom,
    Corridor
};

public class LevelRoom : MonoBehaviour
{
    [SerializeField] private Transform roomConnectionPoint;
    [SerializeField] private Transform playerSpawnPosition;
    [SerializeField] private RoomDoor roomDoor;
    public LevelRoom connectedRoom;
    [SerializeField] private string roomId;
    [SerializeField] private RoomType roomType;
    public Transform GetConnectionPoint() { return roomConnectionPoint; }
    public Transform GetSpawnPosition() { return playerSpawnPosition; }
    public RoomType GetRoomType() { return roomType; }
    public string ID() { return roomId; }
    public void SetID(string newID) { roomId = newID; }

    public void SetUpDoors()
    {
        if(roomDoor)
            roomDoor.Init();
    }
    public void Awake()
    {
        if (RoomManager.instance)
        {
            RoomManager.instance.AddNewRoom(this);
        }
    }
}

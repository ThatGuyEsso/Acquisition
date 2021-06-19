using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class BossRoomManager : MonoBehaviour,IManager,IInitialisable
{
    public static BossRoomManager instance;

    [SerializeField] private RoomDoor entranceDoor, exitDoor;
    [SerializeField] private BaseBossAI Boss;
    [SerializeField] private CinemachineVirtualCamera cutsceneCamera;
    public void BindToGameStateManager()
    {
        GameStateManager.instance.OnNewGameState += EvaluateGameState;
        GameManager.instance.OnNewEvent += EvaluateNewGameEvent;

    }

    public void EvaluateGameState(GameState newState)
    {
        //
    }

    private void EvaluateNewGameEvent(GameEvents newEvent)
    {
        switch (newEvent)
        {
            case GameEvents.BossFightStarts:
               //

                break;
        }
    }
    public void Init()
    {
        if(instance == false)
        {
            instance = this;
            BindToGameStateManager();
            entranceDoor.Init();
            exitDoor.Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void OnDestroy()
    {
        GameStateManager.instance.OnNewGameState -= EvaluateGameState;
        GameManager.instance.OnNewEvent -= EvaluateNewGameEvent;
    }
}

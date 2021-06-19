using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;
using UnityEngine.AI;
public class BossRoomManager : MonoBehaviour,IManager,IInitialisable
{
    public static BossRoomManager instance;

    [SerializeField] private RoomDoor entranceDoor, exitDoor;
    [SerializeField] private BaseBossAI Boss;
    [SerializeField] private CinemachineVirtualCamera cutsceneCamera;
    [SerializeField] private LevelRoom bossRoom;
    public Transform player;
    [SerializeField] private PlayableDirector director;
    [SerializeField] private PlayableAsset bossIntro;

    [SerializeField] private NavMeshSurface2d navMesh;
    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
        director.enabled = false;

    }
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

            case GameEvents.PlayerDefeat:
                Boss.OnNewState(AIState.Idle);

                break;
        }
    }

    public void SetUpDoors()
    {
        entranceDoor.Init();
        exitDoor.Init();

    }
    public void Init()
    {
        if(instance == false)
        {
            instance = this;
            BindToGameStateManager();
            if (RoomManager.instance)
            {
                List<string> roomsToKeep = new List<string>();
                roomsToKeep.Add(bossRoom.ID());

                RoomManager.instance.ClearAllRoomNotInSet(roomsToKeep);
            }
            entranceDoor.ToggleLock(true);
            InitRoom();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitRoom()
    {
        Boss.target = player;
        if (GameManager.instance)
            GameManager.instance.BeginNewEvent(GameEvents.BossInit);

        StartBossIntro();

 
    }

    public IEnumerator BuildNavMesh()
    {
       AsyncOperation navBuild= navMesh.BuildNavMeshAsync();
        while (!navBuild.isDone) yield return null;

    

        InitRoom();
    }

    public void AwakenBoss()
    {
        Boss.Init();
        Boss.OnAwakened += StartFight;

    }

    public void StartFight()
    {
        Boss.OnAwakened -= StartFight;
        Boss.BeginFight();

        CamShake.instance.gameObject.SetActive(true);
        cutsceneCamera.gameObject.SetActive(false);
        director.enabled = false;
        CamShake.instance.DoScreenShake(0.15f, 3f, 0f, 0.5f, 2f);
        GameManager.instance.BeginNewEvent(GameEvents.BossFightStarts);
    }
    public void StartBossIntro()
    {
        CamShake.instance.gameObject.SetActive(false);
        cutsceneCamera.gameObject.SetActive(true);
        director.enabled = true;
        director.playableAsset = bossIntro;
        director.time = 0f;
        director.Play();
    }
    private void OnDestroy()
    {
        GameStateManager.instance.OnNewGameState -= EvaluateGameState;
        GameManager.instance.OnNewEvent -= EvaluateNewGameEvent;
    }
}

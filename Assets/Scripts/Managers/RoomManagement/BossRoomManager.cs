using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;
using UnityEngine.AI;
public class BossRoomManager : MonoBehaviour,IManager,IInitialisable
{
    public static BossRoomManager instance;
    [SerializeField] private BossType bossRoomType;
    [SerializeField] private RoomDoor entranceDoor, exitDoor;
    [SerializeField] private BaseBossAI Boss;
    [SerializeField] private CinemachineVirtualCamera cutsceneCamera;
    [SerializeField] private LevelRoom bossRoom;
    public Transform player;
    [SerializeField] private PlayableDirector director;
    [SerializeField] private PlayableAsset bossIntro;
    [SerializeField] private Vector2 roomHalfSize;
    [SerializeField] private Transform roomCentre;

    [SerializeField] private GameObject spawnVFX;
    [SerializeField] private List<SkillOrbPickUp> pickUps = new List<SkillOrbPickUp>();

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

            case GameEvents.RespawnPlayer:
                if (Boss)
                    ObjectPoolManager.Recycle(Boss.gameObject);

                break;

            case GameEvents.PlayerDefeat:
                Boss.OnNewState(AIState.Idle);
                Boss.UI.gameObject.SetActive(false);
                break;

            case GameEvents.BossDefeated:

         
                if (Boss)
                    ObjectPoolManager.Recycle(Boss.gameObject);
                if (CamShake.instance)
                {
                    CamShake.instance.DoScreenShake(0.5f, 2f, 0.1f, 0.25f, 3f);
                }
                if (!GameStateManager.instance.runtimeData.IsGameClear())
                {
                    foreach (SkillOrbPickUp orb in pickUps)
                    {
                        if (orb)
                        {
                            orb.DisplayOrb();
                            orb.OnSkillSelect += EvaluateSkillOrbCollected;
                        }
                    }

                    if (AudioManager.instance) AudioManager.instance.PlayThroughAudioPlayer("ItemBoom", roomCentre.position);
                }else
                {
                    UnlockAccensionHub();
                }
        
                break;
        }
    }

    public void EvaluateSkillOrbCollected(SkillOrbPickUp pickedOrb)
    {
        pickUps.Remove(pickedOrb);
        pickedOrb.OnSkillSelect -= EvaluateSkillOrbCollected;
        foreach (SkillOrbPickUp orb in pickUps)
        {
            if (orb)
            {
                orb.BeginToHidePickUp();
                orb.OnSkillSelect -= EvaluateSkillOrbCollected;
            }
        }
        UnlockBossRoom();
    }
    public void UnlockBossRoom()
    {
        if (CamShake.instance)
        {
            CamShake.instance.DoScreenShake(0.5f, 4f, 0.1f, 0.25f, 3f);
        }
        exitDoor.ToggleLock(false);

        RoomManager.instance.BeginCreatePathBossToHub(exitDoor.corridorSpawn.position);
        if (AudioManager.instance) AudioManager.instance.PlayThroughAudioPlayer("RoomSpawn", roomCentre.position);

    }


    public void UnlockAccensionHub()
    {
    
        exitDoor.ToggleLock(false);

        RoomManager.instance.BeginCreatePathToHub(exitDoor.corridorSpawn.position,SceneIndex.AccensionHub);
        if (AudioManager.instance) AudioManager.instance.PlayThroughAudioPlayer("RoomSpawn", roomCentre.position);
        if (MusicManager.instance) MusicManager.instance.OnFadeComplete += PlayAccensionMusic;
        if (MusicManager.instance) MusicManager.instance.BeginSongFadeOut(5f);
    }

    public void PlayAccensionMusic()
    {
        if (MusicManager.instance) MusicManager.instance.OnFadeComplete -= PlayAccensionMusic;
        if (MusicManager.instance) MusicManager.instance.BeginSongFadeIn("AccensionRoomScore", 2f, 10f, 20f, 3f);
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


    public void AwakenBoss()
    {
        Boss.Init();

        Boss.OnAwakened += StartFight;
        foreach(SkillOrbPickUp orb in pickUps)
        {
            if (orb)
            {
                orb.BeginToHidePickUp();
            }
        }
    }

    public void StartFight()
    {
        Boss.OnAwakened -= StartFight;
        Boss.BeginFight();

        CamShake.instance.gameObject.SetActive(true);
        cutsceneCamera.gameObject.SetActive(false);
        if(director)
            director.enabled = false;
        if (CamShake.instance)
        {

            CamShake.instance.DoScreenShake(0.15f, 3f, 0f, 0.5f, 2f);
        }
        EvaluateSongBossSong();
        GameManager.instance.BeginNewEvent(GameEvents.BossFightStarts);
     
    }

    public void EvaluateSongBossSong()
    {
        switch (bossRoomType)
        {
            case BossType.Knight:
                if (MusicManager.instance) MusicManager.instance.BeginSongFadeIn("SwordKingBattle", 0.5f, 5f, 10f);
                break;
            case BossType.Elder:
                if (MusicManager.instance) MusicManager.instance.BeginSongFadeIn("ElderBattle", 0.5f, 5f, 10f);
                break;
            case BossType.Scholar:
                if (MusicManager.instance) MusicManager.instance.BeginSongFadeIn("PhoenixBattle", 0.5f, 5f, 10f);
                break;
        }
    }
    public void StartBossIntro()
    {
        CamShake.instance.gameObject.SetActive(false);
        cutsceneCamera.gameObject.SetActive(true);
        director.enabled = true;
        director.playableAsset = bossIntro;
        director.time = 0f;
        director.Play();
        if (MusicManager.instance) MusicManager.instance.OnFadeComplete -= PlayAccensionMusic;
    }
    private void OnDestroy()
    {
        if (MusicManager.instance) MusicManager.instance.OnFadeComplete -= PlayAccensionMusic;
        GameStateManager.instance.OnNewGameState -= EvaluateGameState;
        GameManager.instance.OnNewEvent -= EvaluateNewGameEvent;
    }


    public Vector2 GetRoomHalfSize()
    {
        return roomHalfSize;
    }
    public Vector2 GetRoomCentrePoint()
    {
        return roomCentre.position;
    }
    public BaseBossAI GetBoss()
    {
        return Boss;
    }



}

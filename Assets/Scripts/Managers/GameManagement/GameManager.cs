using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;
public enum GameEvents
{

   WeaponsSpawned,
   WeaponPicked,
   BossRoomsSpawned,
   BossInit,
   BossFightStarts,
   PlayerDefeat,
   DeathMaskComplete,
   DeathAnimationComplete,
   RespawnPlayer,
   PlayerRespawned,
   BossDefeated,
   ExitGame,
   BeginGameComplete

};
public class GameManager : MonoBehaviour,IInitialisable,IManager
{
    public static GameManager instance;
    private bool isSceneLoading;
    private bool isFading;
    private bool isRoomClearing;
 
    private Transform spawn;
    private GameObject playerObject;
  
    private bool isBound;
    public GameEvents lastEvent;
    public Action<GameEvents> OnNewEvent;
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
                BeginInitiateGame();
                break;
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
    public void BeginInitiateGame()
    {
        StopAllCoroutines();
        StartCoroutine(InitiateGame());
    }
   

    private IEnumerator InitiateGame()
    {
        isSceneLoading = true;
        SceneTransitionManager.instance.OnSceneLoadComplete += OnSceneLoadComplete;
        SceneTransitionManager.instance.BeginSceneLoad(SceneIndex.PlayerScene);

        while (isSceneLoading)
        {
            yield return null;
        }
        SceneTransitionManager.instance.OnSceneLoadComplete -= OnSceneLoadComplete;

        playerObject = GameObject.FindGameObjectWithTag("Player");
        if (!playerObject)
        {
            StopAllCoroutines();
            Debug.LogError("No spawn Found");
        }
        if ((spawn= RoomManager.instance.GetSpawn()))
        {
            playerObject.transform.position = spawn.position;
        }
        else
        {
            StopAllCoroutines();
            Debug.LogError("No spawn Found");
        }
        isFading = true;

        LoadingScreen.instance.OnFadeComplete += OnFadeComplete;

        LoadingScreen.instance.BeginFadeOut();
        while (isFading) yield return null;

        if(GameStateManager.instance.currentGameState == GameState.GameRunning)
        {
            BeginNewEvent(GameEvents.PlayerRespawned);
        }
        else
        {
            GameStateManager.instance.BeginNewState(GameState.GameRunning);
        }

    } 

    private void OnSceneLoadComplete()
    {
        SceneTransitionManager.instance.OnSceneLoadComplete -= OnSceneLoadComplete;
        isSceneLoading = false;
    }

    private void OnFadeComplete()
    {
        isFading = false;
        LoadingScreen.instance.OnFadeComplete -= OnFadeComplete;
    }

    private void OnDestroy()
    {
        if(isBound)
            GameStateManager.instance.OnNewGameState -= EvaluateGameState;
    }


    public void BeginNewEvent(GameEvents newEvents)
    {
        lastEvent = newEvents;

        switch (lastEvent)
        {
            case GameEvents.WeaponsSpawned:
     

                OnNewEvent?.Invoke(lastEvent);
         
                break;
            case GameEvents.WeaponPicked:
                OnNewEvent?.Invoke(lastEvent);
                break;
            case GameEvents.BossRoomsSpawned:
     
                OnNewEvent?.Invoke(lastEvent);

                break;
            case GameEvents.BossFightStarts:
                OnNewEvent?.Invoke(lastEvent);
                break;
            case GameEvents.PlayerDefeat:
                WeaponManager.instance.RemoveWeapon();
                if (MusicManager.instance) MusicManager.instance.StopMusic();
                OnNewEvent?.Invoke(lastEvent);
                break;

            case GameEvents.BossDefeated:
                if (MusicManager.instance) MusicManager.instance.BeginSongFadeOut(5f);
                OnNewEvent?.Invoke(lastEvent);

                break;

            case GameEvents.DeathMaskComplete:
             
                OnNewEvent?.Invoke(lastEvent);
                break;
            case GameEvents.RespawnPlayer:
                BeginResetLevel();
                OnNewEvent?.Invoke(lastEvent);

                break;

            case GameEvents.BossInit:
      
                OnNewEvent?.Invoke(lastEvent);

                break;
            case GameEvents.ExitGame:

                OnNewEvent?.Invoke(lastEvent);
                if (MusicManager.instance) MusicManager.instance.BeginSongFadeOut(8f);

                break;

            case GameEvents.BeginGameComplete:

                OnNewEvent?.Invoke(lastEvent);

                break;
        }
    }



    public void BeginResetLevel()
    {
        StartCoroutine(ResetingLevel());
    }

    public void OnRoomsCleared()
    {
        RoomManager.instance.OnAllRoomsCleared -= OnRoomsCleared;
        isRoomClearing = false;
    }
    public IEnumerator ResetingLevel()
    {
        yield return new WaitForSeconds(2.0f);
        isRoomClearing = true;
        RoomManager.instance.OnAllRoomsCleared += OnRoomsCleared;
        RoomManager.instance.BeginClearAllRooms();

        while (isRoomClearing) yield return null;

        GameStateManager.instance.runtimeData.ResetData();

        RoomManager.instance.BeginStartingRoomsLoad();

    }
}

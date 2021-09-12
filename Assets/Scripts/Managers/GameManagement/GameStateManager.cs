using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GameState
{
    InitManagers,
    StartGame,
    TitleScreen,
    LoadingHubWorld,
    HubWorldLoadComplete,
    GameRunning,
    CreditScene,
    Transition,
};

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;
    [SerializeField] private GameObject[] managersToInit;
    [SerializeField] private GameObject roomManagerPrefab;
    [SerializeField] private GameObject GameManagerPrefab;
    public RunTimeData runtimeData;

    public TutorialData tutorialData;
    public Action<GameState> OnNewGameState;
    public GameState currentGameState;
    private AudioPlayer campFireSFXPlayer;
    public string saveName = "/tutorial";

    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
            runtimeData.ResetData();

            if (SerialisationManager.Load(Application.persistentDataPath + "/tutorialData" + saveName + ".saveData") == null)
                SaveData.Current = new SaveData();
            else
                SaveData.Current = (SaveData)SerialisationManager.Load(Application.persistentDataPath + "/tutorialData" + saveName + ".saveData");

            tutorialData.LoadTutorialData();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        BeginNewState(GameState.InitManagers);
    }


    private void InitManagers()
    {
        foreach (GameObject manager in managersToInit)
        {
            GameObject currManager = Instantiate(manager, Vector3.zero, Quaternion.identity);
            IInitialisable init = currManager.GetComponent<IInitialisable>();
            if (init != null) init.Init();

            IManager iManager = currManager.GetComponent<IManager>();
            if (iManager != null) { iManager.BindToGameStateManager(); }
        }
        OnNewGameState(GameState.StartGame);
        if (AudioManager.instance)
            AudioManager.instance.PlayThroughAudioPlayer("DungeonEcho", Vector3.zero);

        if(MusicManager.instance) MusicManager.instance.BeginSongFadeIn("TitleScreenSong", 0.1f, 10f, 20f);
    }
    public void BeginNewState(GameState newState)
    {
        currentGameState = newState;

        switch (currentGameState)
        {
            case GameState.InitManagers:
                InitManagers();
                OnNewGameState?.Invoke(currentGameState);
                break;
            case GameState.StartGame:
                OnNewGameState?.Invoke(currentGameState);
                break;
            case GameState.CreditScene:
                break;
            case GameState.Transition:
                break;

            case GameState.TitleScreen:
                LoadingScreen.instance.BeginFadeOut();
                if (AudioManager.instance) campFireSFXPlayer = AudioManager.instance.PlayUISound("CampFireSFX", Vector3.zero);
                OnNewGameState?.Invoke(currentGameState);
                break;
            case GameState.LoadingHubWorld:
                if(!RoomManager.instance)
                    InitManager(roomManagerPrefab);
                Cursor.visible = false;
                if (campFireSFXPlayer) campFireSFXPlayer.BeginFadeOut();
                OnNewGameState?.Invoke(currentGameState);
                break;

            case GameState.HubWorldLoadComplete:

                if(!GameManager.instance)
                    InitManager(GameManagerPrefab);
                OnNewGameState?.Invoke(currentGameState);
                break;

        }
    }
    public void InitManager(GameObject managerPrefab)
    {

            GameObject manager = Instantiate(managerPrefab, Vector3.zero, Quaternion.identity);
            IInitialisable init = manager.GetComponent<IInitialisable>();
            if (init != null) init.Init();

            IManager iManager = manager.GetComponent<IManager>();
            if (iManager != null) { iManager.BindToGameStateManager(); }
       
    
    }
    
}
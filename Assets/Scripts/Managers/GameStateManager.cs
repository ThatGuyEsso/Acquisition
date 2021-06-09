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
    Paused,
    Transition,
};

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;
    [SerializeField] private GameObject[] managersToInit;
    public Action<GameState> OnNewGameState;
    public GameState currentGameState;
    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
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
            case GameState.Paused:
                break;
            case GameState.Transition:
                break;

            case GameState.TitleScreen:
                LoadingScreen.instance.BeginFadeOut();
                OnNewGameState?.Invoke(currentGameState);
                break;
        }
    }
    public void Init()
    {

    }
    
}
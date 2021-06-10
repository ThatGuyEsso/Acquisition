using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SceneTransitionManager : MonoBehaviour, IManager, IInitialisable
{
    public static SceneTransitionManager instance;
    public static SceneIndex currentScene;
    private List<AsyncOperation> sceneLoading = new List<AsyncOperation>();
    private bool isFading;


    public Action OnSceneLoadComplete;
    public Action OnSceneUnLoadComplete;
    public void BindToGameStateManager()
    {
        GameStateManager.instance.OnNewGameState += EvaluateGameState;
    }

    public void EvaluateGameState(GameState newState)
    {
        switch (newState)
        {
            case GameState.StartGame:
                BeginLoadMenuScreen(SceneIndex.MainMenu);
            break;
            case GameState.LoadingHubWorld:
                BeginHubLoad();
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
        DontDestroyOnLoad(gameObject);
    }

    private Scene[] GetAllActiveScenes()
    {
        //Get all number of scenes loaded
        int countLoaded = SceneManager.sceneCount;

        //create array of respective size
        Scene[] loadedScenes = new Scene[countLoaded];

        //get all loaded scnes
        for (int i = 0; i < countLoaded; i++)
        {
            loadedScenes[i] = (SceneManager.GetSceneAt(i));
        }
        //retun loaded scenes
        return loadedScenes;
    }
    public void BeginSceneLoad(SceneIndex sceneIndex)
    {
        StartCoroutine( LoadScene(sceneIndex));
    }
    public void BeginSceneUnLoad(Scene scene)
    {
        StartCoroutine( UnLoadScene(scene));
    }
    private IEnumerator LoadScene(SceneIndex sceneIndex)
    {
        AsyncOperation sceneLoad = SceneManager.LoadSceneAsync((int)sceneIndex, LoadSceneMode.Additive);
        while (!sceneLoad.isDone)
        {
            yield return null;
        }
        OnSceneLoadComplete?.Invoke();
    }

    private IEnumerator UnLoadScene(Scene scene)
    {
        AsyncOperation sceneUnLoad = SceneManager.UnloadSceneAsync(scene);
        while (!sceneUnLoad.isDone)
        {
            yield return null;
        }
        OnSceneUnLoadComplete?.Invoke();
    }


    public void BeginLoadMenuScreen(SceneIndex menuSceneIndex)
    {
        StopAllCoroutines();
        StartCoroutine(LoadMenuScreen(menuSceneIndex));    
    }
    public void BeginHubLoad()
    {
        StopAllCoroutines();
        StartCoroutine(LoadLevelStartingZOne());
    }
    private IEnumerator LoadLevelStartingZOne()
    {
       
        if (!LoadingScreen.instance.IsLoadingScreenOn())
        {
            isFading = true;
            LoadingScreen.instance.OnFadeComplete += OnFadeComplete;
            LoadingScreen.instance.BeginFadeIn();
            while (isFading)
            {
                yield return null;
            }
        }
        yield return new WaitForSeconds(0.5f);
        sceneLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndex.MainMenu));
   

        //wait until every scene has unloaded
        for (int i = 0; i < sceneLoading.Count; i++)
        {
            while (!sceneLoading[i].isDone)
            {
                yield return null;
            }
        }

        //clear scens loading
        sceneLoading.Clear();
        if (RoomManager.instance) RoomManager.instance.BeginStartingRoomsLoad();
        else
        {
            Debug.LogError("RoomManager Doesn't Exist");
        }

    }
    private IEnumerator LoadMenuScreen(SceneIndex menuSceneIndex)
    {
        Scene[] loadedScenes = GetAllActiveScenes();

        if (!LoadingScreen.instance.IsLoadingScreenOn())
        {
            isFading = true;
            LoadingScreen.instance.OnFadeComplete += OnFadeComplete;
            LoadingScreen.instance.BeginFadeIn();
            while (isFading)
            {
                yield return null;
            }
        }
        yield return new WaitForSeconds(0.5f);
        //add and unload operations
        foreach (Scene scene in loadedScenes)
        {
            if (scene.buildIndex != (int)SceneIndex.RootScene)
                sceneLoading.Add(SceneManager.UnloadSceneAsync(scene));
        }

        //wait until every scene has unloaded
        for (int i = 0; i < sceneLoading.Count; i++)
        {
            while (!sceneLoading[i].isDone)
            {
                yield return null;
            }
        }

        //clear scens loading
        sceneLoading.Clear();

        //begin loading title screen
        AsyncOperation menuScene = SceneManager.LoadSceneAsync((int)menuSceneIndex, LoadSceneMode.Additive);

        while (!menuScene.isDone)
        {
            yield return null;

        }

        currentScene = menuSceneIndex;
        EvaluateSceneLoaded(currentScene);
    }

    public void EvaluateSceneLoaded(SceneIndex scene)
    {
        switch (scene)
        {
            case SceneIndex.MainMenu:
                GameStateManager.instance.BeginNewState(GameState.TitleScreen);
     
                break;

        }
    }

    public void OnFadeComplete()
    {
        isFading = false;
        LoadingScreen.instance.OnFadeComplete -= OnFadeComplete;
    }

   
}

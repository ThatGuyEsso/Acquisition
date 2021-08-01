using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedButton;
    [SerializeField] private FadeOutImages imageFade;
    [SerializeField] private GraphicRaycaster raycaster;
    bool isMusicPlaying;

    private void Awake()
    {
        if (!raycaster)
        {
            raycaster = GetComponent<GraphicRaycaster>();
           
        }

    }
    private void OnEnable()
    {
        if (imageFade) {
            imageFade.BeginFadeIn(5f);
            raycaster.enabled = false;
            imageFade.OnFadeComplete += EnableButtons;
        }
        
        if (UIManager.instance)
            UIManager.instance.eventSystem.SetSelectedGameObject(firstSelectedButton);
        if (firstSelectedButton.activeInHierarchy)
            firstSelectedButton.GetComponent<Button>().Select();

        if (!Cursor.visible) Cursor.visible = true;

        if (MusicManager.instance)
        {
            if (!MusicManager.instance.IsPlaying())
            {
                if (MusicManager.instance) MusicManager.instance.BeginSongFadeIn("TitleScreenSong", 5f, 10f, 20f,5f);
            }
        }
     
    }
    

    public void EnableButtons()
    {
        if (imageFade) imageFade.OnFadeComplete -= EnableButtons;
        if (raycaster) raycaster.enabled = true;

        if (UIManager.instance)
        {
            StartCoroutine(WaitToSelectGameObject());
        }
    }



    public IEnumerator WaitToSelectGameObject()
    {
        if (UIManager.instance)
        {
            UIManager.instance.eventSystem.SetSelectedGameObject(null);
            yield return null;
            UIManager.instance.eventSystem.SetSelectedGameObject(firstSelectedButton);
        }
    }

    
    public void Play()
    {
        if (raycaster) raycaster.enabled = false;
        if (imageFade) imageFade.BeginFadeOut(5f);
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        LoadingScreen.instance.SetLoadingScreenColour(Color.black);

        GameStateManager.instance.runtimeData.ResetData();

        if (!MusicManager.instance)
        {
            BeginStartGame();
        }
        else if(!MusicManager.instance.IsPlaying()||MusicManager.instance.IsMusicOff())
        {
            MusicManager.instance.StopMusic();
            BeginStartGame();
        }
        else
        {

            MusicManager.instance.BeginSongFadeOut(2f);
            MusicManager.instance.OnFadeComplete += BeginStartGame;

        }


    }

    public void BeginStartGame()
    {
        StartCoroutine(StartGameDelay());
    }
    public IEnumerator StartGameDelay()
    {
        yield return new WaitForSeconds(0.5f);
        StartGame();
    }
    public void StartGame()
    {
        GameStateManager.instance.BeginNewState(GameState.LoadingHubWorld);
        UIManager.instance.SwitchUI(UIType.GameUI);

        if(MusicManager.instance) MusicManager.instance.OnFadeComplete -= BeginStartGame;
    }

  

    public void Credits()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        UIManager.instance.SwitchUI(UIType.Credits);
    }

    public void Settings()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        UIManager.instance.SwitchUI(UIType.Settings);
    }
    public void Quit()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        Application.Quit();
        Debug.Log("Quitting");

            
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}

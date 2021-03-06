using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : Base_UI
{
    [SerializeField] private GameObject firstSelectedButton;
    public void OnResume()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        uiManager.SetGameToPause(false);

        ControllerManager.instance.GetActiveCursor().gameObject.SetActive(true);
        UIManager.instance.SwitchUI(UIType.GameUI);
        if (Cursor.visible) Cursor.visible = false;
        UI_Input.isPaused = false;

    }

    public void OnOptions()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        uiManager.SwitchUI(UIType.Settings);
    }

    public void OnMainMenu()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        uiManager.SetGameToPause(false);
        SceneTransitionManager.instance.BeginLoadMenuScreen(SceneIndex.MainMenu,UIType.MainMenu);
        
    }
    public void OnEnable()
    {
        if(UIManager.instance)
            StartCoroutine(WaitToSelectGameObject());


        if (!Cursor.visible) Cursor.visible = true;


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

    public override void InitUI(UIType uiType, UIType preUI)
    {
        base.InitUI(uiType, preUI);
        //DontDestroyOnLoad(gameObject);

     
    }

}

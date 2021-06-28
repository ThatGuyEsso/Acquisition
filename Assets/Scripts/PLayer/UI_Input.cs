using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Input : MonoBehaviour, IInitialisable
{
    [SerializeField] private GameObject virtualCursor;
    [SerializeField] private bool inDebug = false;

    private Controls inputActions;
    private bool isPaused = false;

    private void Awake()
    {
        if (inDebug)
            Init();
    }

    public void Init()
    {
        inputActions = new Controls();
        inputActions.Enable();

        inputActions.UI.Pause.performed += ctx => TogglePauseMenu();
        UIManager.instance.SetGamePaused += OnPaused;
    }


    virtual protected void OnDisable()
    {
        if(inputActions!=null)
            inputActions.Disable();
        if (UIManager.instance)
            UIManager.instance.SetGamePaused -= OnPaused;
    }
    virtual protected void OnEnable()
    {
        if (inputActions != null)
            inputActions.Enable();
        if(UIManager.instance)
            UIManager.instance.SetGamePaused += OnPaused;
    }
    private void TogglePauseMenu()
    {
        if (isPaused)
        {
            isPaused = false;
            UIManager.instance.SetGameToPause(false);

        }
        else if (!isPaused)
        {
            isPaused = true;
            UIManager.instance.SetGameToPause(true);
        }
    }

    private void OnPaused(bool paused)
    {
        if (paused)
        {
            isPaused = true;
            virtualCursor.SetActive(false);
            UIManager.instance.SwitchUI(UIType.PauseMenu);

        }
        else if (!paused)
        {
            isPaused = false;
            virtualCursor.SetActive(true);
            UIManager.instance.SwitchUI(UIType.GameUI);
        }
    }




}

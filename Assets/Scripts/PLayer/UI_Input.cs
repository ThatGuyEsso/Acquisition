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
    }

    private void TogglePauseMenu()
    {
        if(isPaused)
        {
            isPaused = false;
            virtualCursor.SetActive(true);
            UIManager.instance.SwitchUI(UIType.GameUI);
        }
        else if(!isPaused)
        {
            isPaused = true;
            virtualCursor.SetActive(false);
            UIManager.instance.SwitchUI(UIType.PauseMenu);
        }
    }


}

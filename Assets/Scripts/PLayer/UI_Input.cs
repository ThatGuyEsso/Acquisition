using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Input : MonoBehaviour, IInitialisable
{

    [SerializeField] private bool inDebug = false;

    private Controls inputActions;
    public static bool isPaused = false;

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


    virtual protected void OnDisable()
    {
        if(inputActions!=null)
            inputActions.Disable();
    }
    virtual protected void OnEnable()
    {
        if (inputActions != null)
            inputActions.Enable();

    }
    private void OnDestroy()
    {
        if (inputActions != null)
            inputActions.Disable();
 
    }
    private void TogglePauseMenu()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
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
        OnPaused(isPaused);
    }

    private void OnPaused(bool paused)
    {
        if (paused)
        {
            isPaused = true;
            ControllerManager.instance.GetActiveCursor().gameObject.SetActive(false);
            UIManager.instance.SwitchUI(UIType.PauseMenu);

        }
        else if (!paused)
        {
            isPaused = false;
            ControllerManager.instance.GetActiveCursor().gameObject.SetActive(true);
            UIManager.instance.SwitchUI(UIType.GameUI);
            if (Cursor.visible) Cursor.visible = false;


        }
    }




}

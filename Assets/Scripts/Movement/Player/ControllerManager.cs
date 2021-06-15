using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerManager : MonoBehaviour, IInitialisable
{
    [SerializeField] private GamepadMoveCursor gamepadCursor;
    [SerializeField] private MouseMoveCursor mouseCursor;

    Controls input;

    bool isInitialised;

    [SerializeField] private bool inDebug = false;

    private void Awake()
    {
        if (inDebug) Init();
    }

    public void Init()
    {
        input = new Controls();
        input.Enable();

        input.Aiming.Aim.performed += _ =>SwitchToGampadControls();
        input.MouseActivity.Move.performed += _ => SwitchToMouseControls();
        isInitialised = true;
    }



    public void SwitchToGampadControls()
    {
        if (mouseCursor.enabled)
        {
            gamepadCursor.enabled = true;
            mouseCursor.enabled = false;
        }
   
    }


    public void SwitchToMouseControls()
    {
        if (gamepadCursor.enabled)
        {
            gamepadCursor.enabled = false;
            mouseCursor.enabled = true;
        }
    }

    private void OnEnable()
    {
        if (isInitialised)
        {
            input.Enable();
           
        }
 
    }
    private void OnDisable()
    {
        if (isInitialised)
        {
            input.Disable();

        }
    }

    private void OnDestroy()
    {
        if (isInitialised)
        {
            input.Disable();
           
        }
    }
}

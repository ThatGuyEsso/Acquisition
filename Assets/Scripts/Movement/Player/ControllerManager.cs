using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerManager : MonoBehaviour, IInitialisable
{
    [SerializeField] private GamepadMoveCursor gamepadCursor;
    [SerializeField] private MouseMoveCursor mouseCursor;
    private FaceCursor faceCursor;
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
        faceCursor = GetComponent<FaceCursor>();
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
            if (faceCursor) faceCursor.enabled = false;
        }
   
    }


    public void SwitchToMouseControls()
    {
        if (gamepadCursor.enabled)
        {
            gamepadCursor.enabled = false;
            mouseCursor.enabled = true;
            if (faceCursor) faceCursor.enabled = true;
        }
    }

    private void OnEnable()
    {
        if (isInitialised)
        {
            if(input!=null)
                input.Enable();
           
        }
 
    }
    private void OnDisable()
    {
        if (isInitialised)
        {
            if (input != null)
                input.Disable();

        }
    }

    private void OnDestroy()
    {
        if (isInitialised)
        {
            if (input != null)
                input.Disable();
           
        }
    }
}

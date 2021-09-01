using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerManager : MonoBehaviour, IInitialisable
{
    [SerializeField] private GamepadMoveCursor gamepadCursor;
    [SerializeField] private MouseMoveCursor mouseCursor;
    public static ControllerManager instance;
    private FaceCursor faceCursor;
    Controls input;
    public bool rotateToCursor;

    bool isInitialised;

    [SerializeField] private bool inDebug = false;

    private void Awake()
    {
        if (inDebug) Init();
    }

    public void Init()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        input = new Controls();
        input.Enable();
        faceCursor = GetComponent<FaceCursor>();
        input.Aiming.Aim.performed += _ =>SwitchToGampadControls();
        input.MouseActivity.Move.performed += _ => SwitchToMouseControls();
        isInitialised = true;
    }

    public bool IsUsingMouse() { return mouseCursor.enabled; }

    public Transform GetActiveCursor()
    {
        if (mouseCursor.enabled)
        {
           return mouseCursor.GetCursor();
        }
 
        return null;
    }

    public void SwitchToGampadControls()
    {
     
        gamepadCursor.enabled = true;
        mouseCursor.enabled = false;
        if (faceCursor) faceCursor.enabled = false;
        rotateToCursor = false;
  
   
    }


    public void SwitchToMouseControls()
    {
        if (gamepadCursor.enabled)
        {
            gamepadCursor.enabled = false;
            mouseCursor.enabled = true;
            if (faceCursor) faceCursor.enabled = true;
            rotateToCursor = true;
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

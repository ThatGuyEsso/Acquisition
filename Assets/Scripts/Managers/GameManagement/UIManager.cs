using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum UIType
{
    None,
    MainMenu,
    PauseMenu,
    GameUI,
    Settings, 
    Credits
}

[System.Serializable]
public struct UIPool
{
    public UIType type;
    public GameObject obj;
}


public class UIManager : MonoBehaviour, IInitialisable
{
    public delegate void IsPaused(bool ispaused);
    public event IsPaused SetGamePaused;
    public EventSystem eventSystem;
    public static UIManager instance { get; private set; }

    [SerializeField] private UIType startingUI;
    [SerializeField] private List<UIPool> uiPool;
    [SerializeField] private bool inDebug = false;

    private Dictionary<UIType, GameObject> UIDic;
    private UIType previousUI = UIType.None;
    private UIType currentActive;

    private void Awake()
    {
        if (inDebug)
            Init();
    }

    public void Init()
    {
        if (instance != null) // Creates this a a singleton
            Destroy(this);
        else
            instance = this;
        
        DontDestroyOnLoad(this);

        UIDic = new Dictionary<UIType, GameObject>();

        foreach(UIPool pool in uiPool)
        {
            GameObject ui = Instantiate(pool.obj);
            ui.SetActive(false);
            UIDic.Add(pool.type, ui);
        }

        SwitchUI(startingUI);
    }

    public void SwitchUI(UIType type) //changes the UI displayed
    {
        if (type == currentActive)
            return;

        previousUI = currentActive;
        UnloadCurrent();

        GameObject outObj;
        currentActive = type;

        if (UIDic.TryGetValue(type, out outObj))
        {
            outObj.SetActive(true);
        }
        else if (outObj == false)
        {
            Debug.LogError("UIManager can't find gameObject"); // if gameobject of UI does not exits then error is thrown
            return;
        }

        I_UI ui = outObj.GetComponent<I_UI>();

        if (ui != null) // Initalizes UI
            ui.InitUI(type, previousUI);


    
    }

    public void UnloadCurrent() //Unloades the current displayed UI
    {
        if(currentActive != UIType.None)
        {
            GameObject currentObj;
            if (UIDic.TryGetValue(currentActive, out currentObj))
            {
                if (currentObj != false) //Null check on gameobject
                {
                    currentObj.SetActive(false);
                    I_UI ui = currentObj.GetComponent<I_UI>();
                    
                    if(ui != null)
                        ui.ExitUI();
                }
            }
            else
            {
                currentActive = UIType.None;
                return;
            }
            
        }
    }

    public void SetGameToPause(bool pause)
    {
        if (SetGamePaused == null)
            return;

        SetGamePaused(pause);

        if (pause == true)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public UIType GetPrevUI() { return previousUI; }

}

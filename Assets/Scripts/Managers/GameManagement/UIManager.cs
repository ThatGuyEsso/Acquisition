using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public static UIManager instance { get; private set; }

    [SerializeField] private UIType currentActive = UIType.MainMenu;
    [SerializeField] private List<UIPool> uiPool;
    [SerializeField] private bool inDebug = false;

    private Dictionary<UIType, GameObject> UIDic;
    private UIType previousUI = UIType.None;

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
        
    }

    public void SwitchUI(UIType type)
    {
        previousUI = currentActive;
        UnloadCurrent();

        GameObject outObj;
        currentActive = type;

        if (UIDic.TryGetValue(type, out outObj))
            outObj.SetActive(true);

        I_UI ui = outObj.GetComponent<I_UI>();

        if (ui != null)
            ui.InitUI(type, previousUI);
    }

    public void UnloadCurrent()
    {
        if(currentActive != UIType.None)
        {
            GameObject currentObj;

            if (UIDic.TryGetValue(currentActive, out currentObj))
            {
                if (currentObj != false)
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

    public void SetMainMenu(GameObject menu)
    {
        if(menu.tag == "MainMenu")
            UIDic.Add(UIType.MainMenu, menu);
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

}

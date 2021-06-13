using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Base_UI : MonoBehaviour, I_UI
{
    protected UIType thisType;
    protected UIType previousUI; 
    protected UIManager uiManager;

    public virtual void InitUI(UIType uiType, UIType preUI)
    {
        thisType = uiType;
        previousUI = preUI;
        uiManager = UIManager.instance;
    }

    public virtual void ExitUI()
    {
        
    }


}

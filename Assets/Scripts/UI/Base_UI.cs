using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Base_UI : MonoBehaviour, I_UI
{
    protected UIType thisType;
    protected UIManager uiManager;

    public virtual void InitUI(UIType uiType)
    {
        thisType = uiType;
        uiManager = UIManager.instance;
    }

    public virtual void ExitUI()
    {
        
    }

}

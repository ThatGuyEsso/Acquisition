using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour, IInitialisable
{
    [SerializeField] private bool inDebug;
    [SerializeField] private List<Component> playerComponentsToInit = new List<Component>();
    public void Awake()
    {
        if (inDebug) Init();
    }
    public void Init()
    {

        IInitialisable[] playerComps = GetComponents<IInitialisable>();

        foreach(IInitialisable compInit in playerComps)
        {
            if(compInit != this)
                compInit.Init();
        }
        if (playerComponentsToInit.Count > 0)
        {
            for(int i=0; i < playerComponentsToInit.Count; i++)
            {
                IInitialisable init = playerComponentsToInit[i].GetComponent<IInitialisable>();
   
                if (init != null) init.Init();
            }

        }
    }
}

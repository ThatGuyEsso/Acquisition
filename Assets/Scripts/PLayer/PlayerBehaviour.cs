using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour, IInitialisable
{
    [SerializeField] private bool inDebug;
    [SerializeField] private GameObject topGFX;
    [SerializeField] private GameObject bottomGFX;
    [SerializeField] private List<Component> playerComponentsToInit = new List<Component>();
    private List<ICharacterComponents> charComps = new List<ICharacterComponents>();
    bool isBound;
    public void Awake()
    {
        if (inDebug) Init();
    }


    public void BindToManagers()
    {
        if (GameManager.instance)
        {
            GameManager.instance.OnNewEvent += EvaluateNewEvent;
            isBound = true;
        }
      

    }


    public void EvaluateNewEvent(GameEvents gameEvents)
    {
        switch (gameEvents)
        {
           
            case GameEvents.PlayerDefeat:
                DisableCharacterComponents();
                bottomGFX.SetActive(false);
                topGFX.SetActive(false);


                break;

            case GameEvents.DeathMaskComplete:
                GameManager.instance.BeginNewEvent(GameEvents.RespawnPlayer);
                break;
            case GameEvents.RespawnPlayer:
            
                break;
            case GameEvents.PlayerRespawned:
                bottomGFX.SetActive(true);
                topGFX.SetActive(true);
                ResetCharacterComponents();
                break;
            case GameEvents.BossInit:
                DisableCharacterComponents();
                WeaponManager.instance.ToggleWeapon(false);
                break;

            case GameEvents.BossFightStarts:
                EnableCharacterComponents();
                WeaponManager.instance.ToggleWeapon(true);
                break;

            case GameEvents.BeginGameComplete:
                DisableCharacterComponents();
                break;
        }
    }
    public void Init()
    {
        BindToManagers();
        IInitialisable[] playerComps = GetComponents<IInitialisable>();

        foreach (IInitialisable compInit in playerComps)
        {
            if (compInit != this)
                compInit.Init();
        }
        if (playerComponentsToInit.Count > 0)
        {
            for (int i = 0; i < playerComponentsToInit.Count; i++)
            {
                IInitialisable init = playerComponentsToInit[i].GetComponent<IInitialisable>();

                if (init != null) init.Init();
            }

        }

    }

    public void DisableCharacterComponents()
    {
        if (charComps.Count == 0)
        {
            ICharacterComponents[] comps = GetComponents<ICharacterComponents>();
            for(int i=0; i < comps.Length; i++)
            {
               
                charComps.Add(comps[i]);
            }
        }


        if (charComps.Count > 0)
        {
            foreach(ICharacterComponents comp in charComps)
            {
                if (comp!=null)
                {
                    comp.DisableComponent();
                }

            }
        }

    }
    public void EnableCharacterComponents()
    {
        if (charComps.Count == 0)
        {
            ICharacterComponents[] comps = GetComponents<ICharacterComponents>();
            for (int i = 0; i < comps.Length; i++)
            {
                charComps.Add(comps[i]);
            }
        }


        if (charComps.Count > 0)
        {
            foreach (ICharacterComponents comp in charComps)
            {
                if (comp != null)
                {
                    comp.EnableComponent();
                }

            }
        }
        if (ControllerManager.instance)
        {
            if (!ControllerManager.instance.rotateToCursor)
                ControllerManager.instance.SwitchToGampadControls();
        }
    }

    public void ResetCharacterComponents()
    {
        if (charComps.Count == 0)
        {
            ICharacterComponents[] comps = GetComponents<ICharacterComponents>();
            for (int i = 0; i < comps.Length; i++)
            {
                charComps.Add(comps[i]);
            }
        }


        if (charComps.Count > 0)
        {
            foreach (ICharacterComponents comp in charComps)
            {
                if (comp != null)
                {
                    comp.ResetComponent();
                }

            }
        }

    }

    private void OnDestroy()
    {
        if(isBound)
             GameManager.instance.OnNewEvent -= EvaluateNewEvent;
    }
}
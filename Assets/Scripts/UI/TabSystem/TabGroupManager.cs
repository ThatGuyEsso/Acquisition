using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class TabGroupManager : MonoBehaviour, Controls.ITabsActions
{
    public List<Tab> tabs;
    [SerializeField] private TabSettings settings;
    [SerializeField] private List<GameObject> objectsToSwap;

    private Controls input;
    private Tab selectedTab;

    private void Awake()
    {
        if(tabs[0] != null)
            selectedTab = tabs[0];

        input = new Controls();
        input.Tabs.SetCallbacks(this);
        input.Enable();
        RestTabs();
    }

    public void Subscribe(Tab button)
    {
        if(tabs == null)
        {
            tabs = new List<Tab>();
        }

        tabs.Add(button);
    }

    public void OnTabEnter(Tab button)
    {
        RestTabs();
        if (selectedTab == null || button != selectedTab)
        {
            button.background.color = settings.tabHovered;
            if (AudioManager.instance) AudioManager.instance.PlayUISound("ButtonHover", Vector3.zero, true);
        }
    }

    public void OnTabExit(Tab button)
    {
        RestTabs();
    }

    public void OnTabSelected(Tab button)
    {
        selectedTab = button;
        RestTabs();
        button.background.color = settings.tabSelected;
        button.background.transform.localScale = settings.selectedTabScaleValue;
        
        int index = button.transform.GetSiblingIndex();
        for(int i =0; i < objectsToSwap.Count; i++)
        {
            if(i == index)
                objectsToSwap[i].SetActive(true);
            else
                objectsToSwap[i].SetActive(false);
        }


    }

    public void RestTabs()
    {
        foreach(Tab buttons in tabs)
        {
            if (selectedTab != null && selectedTab == buttons)
                continue;

            buttons.background.color = settings.tabIdle;
            buttons.background.transform.localScale = new Vector3(1, 1, 1);
            
        }
    }

    public void OnNextTab(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (AudioManager.instance) AudioManager.instance.PlayUISound("ButtonHover", Vector3.zero, true);
            int currentIndex = GetCurrentTabindex();

            currentIndex++;
            if (currentIndex >= tabs.Count) currentIndex = 0;

            OnTabSelected(tabs[currentIndex]);
        }

    }

    public void OnPrevTab(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (AudioManager.instance) AudioManager.instance.PlayUISound("ButtonHover", Vector3.zero, true);
            int currentIndex = GetCurrentTabindex();

            currentIndex--;
            if (currentIndex < 0) currentIndex = tabs.Count - 1;

            OnTabSelected(tabs[currentIndex]);
        }

    }

    public int GetCurrentTabindex()
    {
        for(int i=0; i < tabs.Count; i++)
        {
            if (tabs[i] == selectedTab) return i;
        }
         return 0;
    }
    public void OnEnable()
    {
        if (input != null) input.Enable();
    }


    public void OnDisable()
    {
        if (input != null) input.Disable();
    }

    public void OnDestroy()
    {
        if (input != null) input.Disable();
    }
}

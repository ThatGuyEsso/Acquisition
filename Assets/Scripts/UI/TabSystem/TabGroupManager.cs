using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroupManager : MonoBehaviour
{
    public List<Tab> tabs;
    [SerializeField] private TabSettings settings;
    [SerializeField] private List<GameObject> objectsToSwap;

    private Tab selectedTab;

    private void Awake()
    {
        if(tabs[0] != null)
            selectedTab = tabs[0];

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
}

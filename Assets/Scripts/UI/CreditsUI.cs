using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CreditsUI : Base_UI
{
    [SerializeField] private GameObject firstSelectedElement;

    public void OnBackButton()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        uiManager.SwitchUI(previousUI);
    }



    public void OnEnable()
    {
        if (UIManager.instance)
        {
            UIManager.instance.eventSystem.SetSelectedGameObject(firstSelectedElement);


        }
    }
}

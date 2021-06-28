using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedButton;
    private void OnEnable()
    {
        if (UIManager.instance)
            UIManager.instance.eventSystem.SetSelectedGameObject(firstSelectedButton);
        if (firstSelectedButton.activeInHierarchy)
            firstSelectedButton.GetComponent<Button>().Select();

        if (!Cursor.visible) Cursor.visible = true;
    }
    public void Play()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        GameStateManager.instance.BeginNewState(GameState.LoadingHubWorld);
        UIManager.instance.SwitchUI(UIType.GameUI);
    }

    public void Credits()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        UIManager.instance.SwitchUI(UIType.Credits);
    }

    public void Settings()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        UIManager.instance.SwitchUI(UIType.Settings);
    }
    public void Quit()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        Application.Quit();
        Debug.Log("Quitting");

            
    }
}

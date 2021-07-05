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
        if(previousUI == UIType.GameUI) uiManager.SwitchUI(UIType.MainMenu);
        else
            uiManager.SwitchUI(previousUI);
    }



    public void OnEnable()
    {
        if (UIManager.instance)
        {
            UIManager.instance.eventSystem.SetSelectedGameObject(firstSelectedElement);

            if (!Cursor.visible) Cursor.visible = true;

            if (MusicManager.instance)
            {
                if (!MusicManager.instance.IsPlaying())
                {
                     MusicManager.instance.BeginSongFadeIn("TitleScreenSong", 2f, 10f, 20f);
                }
            }
        }
    }
}

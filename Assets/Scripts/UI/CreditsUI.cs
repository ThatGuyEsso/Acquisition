using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CreditsUI : Base_UI
{
    [SerializeField] private GameObject nextPageButton;
    [SerializeField] private GameObject previousPageButton;
    [SerializeField] private GameObject titleScreenButton;
    [SerializeField] private List<GameObject> pages = new List<GameObject>();
    int currentPageIndex = 0;
    public void OnBackButton()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        if(previousUI == UIType.GameUI) uiManager.SwitchUI(UIType.MainMenu);
        else
            uiManager.SwitchUI(previousUI);
    }


    public void NextPage()
    {
        currentPageIndex++;
        if (currentPageIndex >= pages.Count - 1) currentPageIndex = pages.Count - 1;
        EvaluatePage();
    }
    public void PreviousPage()
    {
        currentPageIndex--;
        if (currentPageIndex <= 0) currentPageIndex = 0;
        EvaluatePage();
    }


    public void EvaluatePage()
    {
        for(int i =0; i < pages.Count; i++)
        {
            if (i == currentPageIndex) pages[i].SetActive(true);
            else pages[i].SetActive(false);
        }
        if (currentPageIndex == 0)
        {
            previousPageButton.SetActive(false);
            nextPageButton.SetActive(true);
            StartCoroutine(WaitToSelectGameObject(nextPageButton));
        }else if(currentPageIndex >= pages.Count - 1)
        {
            nextPageButton.SetActive(false);
            previousPageButton.SetActive(true);
            StartCoroutine(WaitToSelectGameObject(titleScreenButton));
        }
        else
        {
            nextPageButton.SetActive(true);
            previousPageButton.SetActive(true);
            StartCoroutine(WaitToSelectGameObject(nextPageButton));
        }
    }
    public void OnEnable()
    {
        if (UIManager.instance)
        {
            EvaluatePage();

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

    public IEnumerator WaitToSelectGameObject(GameObject button)
    {
        if (UIManager.instance)
        {
            UIManager.instance.eventSystem.SetSelectedGameObject(null);
            yield return null;
            UIManager.instance.eventSystem.SetSelectedGameObject(button);
        }
    }
}

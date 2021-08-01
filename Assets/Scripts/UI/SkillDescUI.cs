using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillDescUI : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedElement;
    
    [SerializeField] private GraphicRaycaster raycaster;
    public PlayerBehaviour playerRef;
    private FadeOutImages fadeControl;

    private void Awake()
    {
        fadeControl = GetComponentInChildren<FadeOutImages>();
        raycaster = GetComponent<GraphicRaycaster>();
    }

 
    public void OK()
    {
        ButtonPressSFX();
        raycaster.enabled = false;
        if (fadeControl)
        {
            fadeControl.OnFadeComplete += OnFadeOutComplete;
            fadeControl.BeginFadeOut(8f);
        }
    }

    public void ButtonPressSFX()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
    }
    public void OnEnable()
    {
        if (fadeControl)
        {
            raycaster.enabled = false;
            fadeControl.OnFadeComplete += OnFadeInComplete;
            fadeControl.BeginFadeIn(5f);
        }
        else
        {
            if (UIManager.instance)
            {
                StartCoroutine(WaitToSelectGameObject());

            }
        }
 
    }

    public void OnFadeInComplete()
    {
        raycaster.enabled = true;
        fadeControl.OnFadeComplete -= OnFadeInComplete;
        if (UIManager.instance)
        {
            StartCoroutine(WaitToSelectGameObject());

        }
    }
    public void OnFadeOutComplete()
    {
   
        fadeControl.OnFadeComplete -= OnFadeOutComplete;
        if (playerRef) playerRef.EnableCharacterComponents();
        if (ObjectPoolManager.instance) ObjectPoolManager.Recycle(gameObject);
    }
    private void OnDisable()
    {
        if (fadeControl)
        {
            fadeControl.OnFadeComplete -= OnFadeInComplete;
            fadeControl.OnFadeComplete -= OnFadeOutComplete;
        }
    }

    public IEnumerator WaitToSelectGameObject()
    {
        if (UIManager.instance)
        {
            UIManager.instance.eventSystem.SetSelectedGameObject(null);
            yield return null;
            UIManager.instance.eventSystem.SetSelectedGameObject(firstSelectedElement);
        }
    }
}

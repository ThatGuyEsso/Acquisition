using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class Tab : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public TabGroupManager tabManager;
    public Image background;

    public void Awake()
    {
        background = GetComponent<Image>();
        tabManager.Subscribe(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabManager.OnTabSelected(this);
        if(AudioManager.instance) AudioManager.instance.PlayUISound("ButtonPress", Vector3.zero, true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabManager.OnTabEnter(this);
       
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabManager.OnTabExit(this);
    }
}

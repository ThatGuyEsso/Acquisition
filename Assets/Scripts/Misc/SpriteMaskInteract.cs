using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteMaskInteract : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> renderers =new List<SpriteRenderer>();
    [SerializeField] private SpriteMaskInteraction defaultMaskInteractMode;
    [SerializeField] private SpriteMaskInteraction activeMaskInteractMode;


    public bool resetOnDisable = false;
    public void Awake()
    {
        SpriteRenderer[] childRenderers = GetComponentsInChildren<SpriteRenderer>();

        if (childRenderers.Length > 0)
        {
            for(int i=0; i < childRenderers.Length; i++)
            {
                renderers.Add(childRenderers[i]);

            }
        }
        SpriteRenderer parentRender = GetComponent<SpriteRenderer>();

        if (parentRender) renderers.Add(parentRender);

        if (GameManager.instance)
        {
            GameManager.instance.OnNewEvent += EvaluateNewEvent;
        }
        
    }

    public void EvaluateNewEvent(GameEvents events)
    {
        switch (events)
        {

            case GameEvents.PlayerDefeat:
                ActiveInteract();
                break;
            case GameEvents.RespawnPlayer:
              
                break;
            case GameEvents.PlayerRespawned:
                DeactiveInteract();
                break;
  
        }
    }


    private void OnEnable()
    {
        if (resetOnDisable) DeactiveInteract();
    }
    public void ActiveInteract()
    {
        if (renderers.Count > 0)
        {
            foreach(SpriteRenderer render in renderers)
            {
                render.maskInteraction = activeMaskInteractMode;
            }
        }
    }
    public void DeactiveInteract()
    {
        if (renderers.Count > 0)
        {
            foreach (SpriteRenderer render in renderers)
            {
                render.maskInteraction = defaultMaskInteractMode;
            }
        }
    }


    private void OnDestroy()
    {
        if(GameManager.instance)
            GameManager.instance.OnNewEvent -= EvaluateNewEvent;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class TileMaskInteract : MonoBehaviour
{
    [SerializeField] private List<TilemapRenderer> renderers =new List<TilemapRenderer>();
    [SerializeField] private SpriteMaskInteraction defaultMaskInteractMode;
    [SerializeField] private SpriteMaskInteraction activeMaskInteractMode;
    public void Awake()
    {
        TilemapRenderer[] childRenderers = GetComponentsInChildren<TilemapRenderer>();

        if (childRenderers.Length > 0)
        {
            for(int i=0; i < childRenderers.Length; i++)
            {
                renderers.Add(childRenderers[i]);

            }
        }
        TilemapRenderer parentRender = GetComponent<TilemapRenderer>();

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
    public void ActiveInteract()
    {
        if (renderers.Count > 0)
        {
            foreach(TilemapRenderer render in renderers)
            {
                render.maskInteraction = activeMaskInteractMode;
            }
        }
    }
    public void DeactiveInteract()
    {
        if (renderers.Count > 0)
        {
            foreach (TilemapRenderer render in renderers)
            {
                render.maskInteraction = defaultMaskInteractMode;
            }
        }
    }


    private void OnDestroy()
    {
        GameManager.instance.OnNewEvent -= EvaluateNewEvent;
    }
}
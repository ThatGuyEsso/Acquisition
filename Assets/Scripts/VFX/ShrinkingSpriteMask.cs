using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkingSpriteMask : MonoBehaviour
{
    [SerializeField] private float initialSize;
    [SerializeField] private float targetSize;
    [SerializeField] private float shrinkRate;


    bool isShrinking;
    bool isBound;
    

    public void Update()
    {
        if (isShrinking)
        {
            transform.localScale -= Vector3.one * Time.deltaTime * shrinkRate;
            if(transform.localScale.x <=targetSize)
            {
                transform.localScale = Vector3.one * targetSize;
                isShrinking = false;

                if (GameManager.instance)
                    GameManager.instance.BeginNewEvent(GameEvents.DeathMaskComplete);
            }
        }
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.one * initialSize;
        isShrinking = true;

        if (GameManager.instance)
        {
            GameManager.instance.OnNewEvent += EvaluateNewEvent;
            isBound = true;
        }
    }

    public void EvaluateNewEvent(GameEvents gameEvents)
    {
        switch (gameEvents)
        {

            case GameEvents.PlayerRespawned:
                if (gameObject)
                    ObjectPoolManager.Recycle(gameObject);
                break;

        }
    }

    private void OnDisable()
    {

        if (isBound)
        {
            GameManager.instance.OnNewEvent -= EvaluateNewEvent;
            isBound = false;
        }
    }

    private void OnDestroy()
    {
        if (isBound)
        {
            GameManager.instance.OnNewEvent -= EvaluateNewEvent;
            isBound = false;
        }
    }
}

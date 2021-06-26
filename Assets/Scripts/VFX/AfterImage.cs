using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void StartAfterImage(Sprite sprite, float lifeTime)
    {
        if (spriteRenderer)
        {
            spriteRenderer.sprite = sprite;
            StartCoroutine(WaitTillClear(lifeTime));
        }
    }



    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator WaitTillClear(float time)
    {
        yield return new WaitForSeconds(time);

        if (ObjectPoolManager.instance) ObjectPoolManager.Recycle(gameObject);
    }
}

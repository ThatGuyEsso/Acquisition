using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutSprite : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float fadeOutRate;
    private float fadeInRate;
    bool isFadingIn;
    bool isFadingOut;
    public System.Action OnFadeComplete;
    public void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void ShowSprite()
    {
        isFadingIn = false;
        isFadingOut = false;
        spriteRenderer.color = new Vector4(spriteRenderer.color.r,
         spriteRenderer.color.g, spriteRenderer.color.b, 1f);
    }
    public void BeginFadeOut(float rate)
    {
        fadeOutRate = rate;
        isFadingIn = false;
        isFadingOut = true;
    }
    public void BeginFadeIn(float rate)
    {
        fadeInRate = rate;
        isFadingOut = false;
        isFadingIn = true;
    }


    public void DoFadeOut()
    {
        spriteRenderer.color = Vector4.Lerp(spriteRenderer.color, new Vector4(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, 0f),Time.deltaTime*fadeOutRate);

        if(spriteRenderer.color.a <= 0.05)
        {
            spriteRenderer.color = new Vector4(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, 0f);
            isFadingOut = false;
            OnFadeComplete?.Invoke();
        }
    }
    public void DoFadeIn()
    {
        spriteRenderer.color = Vector4.Lerp(spriteRenderer.color, new Vector4(spriteRenderer.color.r,
          spriteRenderer.color.g, spriteRenderer.color.b, 1f), Time.deltaTime * fadeInRate);

        if (spriteRenderer.color.a >= 0.95)
        {
            spriteRenderer.color = new Vector4(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, 1f);
            isFadingIn = false;
            OnFadeComplete?.Invoke();
        }
    }


    public void Update()
    {
        if(isFadingOut && !isFadingIn)
        {
            DoFadeOut();
        }
        else if(!isFadingOut && isFadingIn)
        {
            DoFadeIn();
        }
    }
}

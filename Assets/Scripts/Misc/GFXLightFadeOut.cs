using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
public class GFXLightFadeOut : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float fadeOutRate;
    private float fadeInRate;
    bool isFadingIn;
    bool isFadingOut;
    public System.Action OnFadeComplete;
    private Light2D light2D;
    private float maxIntensity;
    public void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        light2D = GetComponentInChildren<Light2D>();
        if (light2D) maxIntensity = light2D.intensity;
    }

    public void ShowSprite()
    {
        if (!light2D || !spriteRenderer) return;

        isFadingIn = false;
        isFadingOut = false;
        spriteRenderer.color = new Vector4(spriteRenderer.color.r,
         spriteRenderer.color.g, spriteRenderer.color.b, 1f);
        light2D.intensity = maxIntensity;
    }
    public void BeginFadeOut(float rate)
    {
        if (!light2D || !spriteRenderer) return;
        fadeOutRate = rate;
        isFadingIn = false;
        isFadingOut = true;
    }
    public void BeginFadeIn(float rate)
    {
        if (!light2D || !spriteRenderer) return;
        fadeInRate = rate;
        isFadingOut = false;
        isFadingIn = true;
    }


    public void DoFadeOut()
    {
        if (!light2D || !spriteRenderer) return;
        spriteRenderer.color = Vector4.Lerp(spriteRenderer.color, new Vector4(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, 0f),Time.deltaTime*fadeOutRate);
        light2D.intensity = spriteRenderer.color.a;
        if (spriteRenderer.color.a <= 0.05)
        {
            light2D.intensity = spriteRenderer.color.a;
            spriteRenderer.color = new Vector4(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, 0f);
            isFadingOut = false;
            OnFadeComplete?.Invoke();
        }
    }
    public void DoFadeIn()
    {
        if (!light2D || !spriteRenderer) return;
        spriteRenderer.color = Vector4.Lerp(spriteRenderer.color, new Vector4(spriteRenderer.color.r,
          spriteRenderer.color.g, spriteRenderer.color.b, 1f), Time.deltaTime * fadeInRate);
        light2D.intensity = spriteRenderer.color.a;
        if (spriteRenderer.color.a >= 0.95)
        {
            light2D.intensity = spriteRenderer.color.a;
            spriteRenderer.color = new Vector4(spriteRenderer.color.r,
            spriteRenderer.color.g, spriteRenderer.color.b, 1f);
            isFadingIn = false;
            OnFadeComplete?.Invoke();
        }
    }


    public void Update()
    {
        if (!light2D || !spriteRenderer) return;
        if (isFadingOut && !isFadingIn)
        {
            DoFadeOut();
        }
        else if(!isFadingOut && isFadingIn)
        {
            DoFadeIn();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class ImageGroup : MonoBehaviour
{
    [SerializeField] private float bgOppacity;
    [SerializeField] private float fadeRate;
    [SerializeField] private Image background;
    [SerializeField] private List<Image> elements = new List<Image>();

    bool isFadingIn;
    bool isFadingOut;
    float currAlpha;
    public Action OnFadeEnd;
    public void Update()
    {
        if (isFadingIn && !isFadingOut)
        {
            DoFadeIn();
        }
        else if(!isFadingIn&&isFadingOut)
        {
            DoFadeOut();
        }
    }

    public void Hide()
    {
        if (elements != null && elements.Count > 0)
        {
            foreach (Image image in elements)
            {

                image.color = new Vector4(image.color.r,
                 image.color.g, image.color.b, 0f);
            }
        }

        background.color = new Vector4(background.color.r,
              background.color.g, background.color.b, 0f);
    }

    public void BeginFadeIn()
    {
        if (elements != null && elements.Count > 0)
        {
            foreach (Image image in elements)
            {

                image.color = new Vector4(image.color.r,
                 image.color.g, image.color.b, 0f);
            }
        }

        background.color = new Vector4(background.color.r,
              background.color.g, background.color.b, 0f);

        isFadingOut = false;
        isFadingIn = true;
    }

    public void BeginFadeOut()
    {

        isFadingOut = true;
        isFadingIn = false;
    }
    public void DoFadeIn()
    {
        currAlpha = Mathf.Lerp(currAlpha, 1f, Time.deltaTime * fadeRate);
        if (currAlpha >= 0.95f)
        {
            currAlpha = 1f;
            foreach (Image image in elements)
            {

                image.color = new Vector4(image.color.r,
                    image.color.g, image.color.b, currAlpha);


            }
            background.color = new Vector4(background.color.r,
                     background.color.g, background.color.b, bgOppacity);

            isFadingIn = false;
            OnFadeEnd?.Invoke();
        }
        else
        {
            foreach (Image image in elements)
            {

                image.color = new Vector4(image.color.r,
                    image.color.g, image.color.b, currAlpha);


            }
            if(currAlpha<= bgOppacity)
                background.color = new Vector4(background.color.r,
                         background.color.g, background.color.b, currAlpha);
        }

       

    }
    public void DoFadeOut()
    {
        currAlpha = Mathf.Lerp(currAlpha, 0f, Time.deltaTime * fadeRate);
        if (currAlpha <= 0.05f)
        {
            currAlpha = 0f;
            foreach (Image image in elements)
            {

                image.color = new Vector4(image.color.r,
                    image.color.g, image.color.b, currAlpha);


            }
            background.color = new Vector4(background.color.r,
                     background.color.g, background.color.b, currAlpha);

            isFadingOut = false;
            OnFadeEnd?.Invoke();
        }
        else
        {
            foreach (Image image in elements)
            {

                image.color = new Vector4(image.color.r,
                    image.color.g, image.color.b, currAlpha);


            }
            if (currAlpha <= bgOppacity)
                background.color = new Vector4(background.color.r,
                         background.color.g, background.color.b, currAlpha);
        }



    }

}

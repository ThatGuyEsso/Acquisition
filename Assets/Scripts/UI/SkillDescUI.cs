using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SkillDescUI : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedElement;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private float fadeInRate;
    [SerializeField] private float fadeOutRate;
    [SerializeField] private bool fadeInOnEnable=true;
    [SerializeField] private float targetBackgroundOppacity = 0.45f;
    public PlayerBehaviour playerRef;

    bool isFadingIn;
    bool isFadingOut;

    List<Image> imageElements = new List<Image>();
    TextMeshProUGUI[] textElements;
    private void Awake()
    {
        Image[] currentImages = GetComponentsInChildren<Image>();

        for(int i =0; i < currentImages.Length; i++)
        {
            if (currentImages[i] != backgroundImage) imageElements.Add(currentImages[i]);
        }
        textElements =  GetComponentsInChildren<TextMeshProUGUI>();
        raycaster = GetComponent<GraphicRaycaster>();
    }

 
    public void OK()
    {
        ButtonPressSFX();
        BeginFadeOut();
        raycaster.enabled = false;



    }

    public void ButtonPressSFX()
    {
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
    }
    public void OnEnable()
    {

        if (fadeInOnEnable)
        {
            BeginFadeIn();
        }
        else
        {

            if (UIManager.instance)
            {
                StartCoroutine(WaitToSelectGameObject());

            }
        }


        
 
    }


    

    public void BeginFadeIn()
    {
        backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, 0f);
        foreach (Image image in imageElements)
        {

            image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
        }
        foreach (TextMeshProUGUI text in textElements)
        {

            text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
        }
        isFadingOut = false;
        isFadingIn = true;
    }
    public void BeginFadeOut()
    {
        isFadingIn = false;
        isFadingOut = true;
    }

    public void OnFadeInComplete()
    {
        raycaster.enabled = true;
        if (!Cursor.visible) Cursor.visible = true;
        if (UIManager.instance)
        {
            StartCoroutine(WaitToSelectGameObject());

        }
    }
    public void OnFadeOutComplete()
    {

        if (Cursor.visible) Cursor.visible = false;
        if (playerRef) playerRef.EnableCharacterComponents();
        if (ObjectPoolManager.instance) ObjectPoolManager.Recycle(gameObject);
    }
    private void Update()
    {
        if (isFadingIn && !isFadingOut)
        {
            if (backgroundImage.color.a < targetBackgroundOppacity)
            {
                backgroundImage.color = Vector4.Lerp(backgroundImage.color, new Vector4(backgroundImage.color.r,
                    backgroundImage.color.g, backgroundImage.color.b, targetBackgroundOppacity), Time.deltaTime * fadeInRate);
                if (Mathf.Abs(targetBackgroundOppacity - backgroundImage.color.a) >= 0.05f)
                {
                    backgroundImage.color = new Vector4(backgroundImage.color.r,
                    backgroundImage.color.g, backgroundImage.color.b, targetBackgroundOppacity);


                }
            }
            else
            {
                float alpha = 0f;
                foreach (Image image in imageElements)
                {
                    image.color = Vector4.Lerp(image.color, new Vector4(image.color.r,
                    image.color.g, image.color.b, 1f), Time.deltaTime * fadeInRate);
                    alpha = image.color.a;
                }
                foreach (TextMeshProUGUI text in textElements)
                {
                    text.color = Vector4.Lerp(text.color, new Vector4(text.color.r,
                    text.color.g, text.color.b, 1f), Time.deltaTime * fadeInRate);
                    alpha = text.color.a;
                }
                if (alpha >= 0.95)
                {
                    alpha = 1f;
                    foreach (Image image in imageElements)
                    {

                        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                    }
                    foreach (TextMeshProUGUI text in textElements)
                    {
                        text.color = Vector4.Lerp(text.color, new Vector4(text.color.r,
                        text.color.g, text.color.b, 1f), Time.deltaTime * fadeInRate);
                        alpha = text.color.a;
                    }
                    isFadingIn = false;
                    OnFadeInComplete();
                }

            }
        }
        else if (!isFadingIn && isFadingOut)
        {
            if (imageElements[0].color.a > 0)
            {
                float alpha = 0f;
                foreach (Image image in imageElements)
                {
                    image.color = Vector4.Lerp(image.color, new Vector4(image.color.r,
                    image.color.g, image.color.b, 0f), Time.deltaTime * fadeOutRate);
                    alpha = image.color.a;
                }
                foreach (TextMeshProUGUI text in textElements)
                {
                    text.color = Vector4.Lerp(text.color, new Vector4(text.color.r,
                    text.color.g, text.color.b, 0f), Time.deltaTime * fadeOutRate);
                    alpha = text.color.a;
                }
                if (alpha <= 0.05)
                {
                    alpha = 0f;
                    foreach (Image image in imageElements)
                    {

                        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                    }
                    foreach (TextMeshProUGUI text in textElements)
                    {
                        text.color = Vector4.Lerp(text.color, new Vector4(text.color.r,
                        text.color.g, text.color.b, 1f), Time.deltaTime * fadeOutRate);
                        alpha = text.color.a;
                    }
                }
                else
                {

                    backgroundImage.color = Vector4.Lerp(backgroundImage.color, new Vector4(backgroundImage.color.r,
                    backgroundImage.color.g, backgroundImage.color.b, 0f), Time.deltaTime * fadeOutRate);
                    if (backgroundImage.color.a <= 0.05f)
                    {
                        backgroundImage.color = new Vector4(backgroundImage.color.r,
                        backgroundImage.color.g, backgroundImage.color.b, 0f);

                        isFadingOut = false;
                        OnFadeOutComplete();
                    }


                    
                   
                }
            }
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

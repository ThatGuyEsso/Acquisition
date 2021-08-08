using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public enum TutorialType
{
    Movement,
    Aiming,
    Dodging,
    Attack
}
public class BaseTutorial : MonoBehaviour
{
    [SerializeField] protected TutorialType type;
    [SerializeField] protected TextMeshProUGUI labelText;
    [SerializeField] protected Image labelImage;
    [SerializeField] protected float fadeRate =5f;
    [SerializeField] protected List<ImageGroup> tutorialPrompts;

    [SerializeField] protected Color inCompleteTextColour;
    [SerializeField] protected Color completeTextColour;

    protected float labelAlpha;
    bool isLabelFading;
    bool isLabelFadingOut;
    float currAlpha =0f;
 
    
    virtual public void InitTutorial()
    {
        if (labelImage) labelAlpha = labelImage.color.a;
        labelText.color = new Vector4(inCompleteTextColour.r,
                 inCompleteTextColour.g, inCompleteTextColour.b, 0f); 

        labelImage.color = new Vector4(labelImage.color.r,
                 labelImage.color.g, labelImage.color.b, 0f);

        if (tutorialPrompts.Count>0)
        {
            foreach (ImageGroup prompt in tutorialPrompts)
            {
                if (prompt) 
                    prompt.Hide();
            }
        }

        isLabelFading = true;
    }

    virtual public void BeginTutorialDisplay()
    {

    }

    virtual protected void DisableTutorial()
    {

    }

    virtual public void BeginHideTutorial()
    {
        tutorialPrompts[tutorialPrompts.Count - 1].OnFadeEnd += BeginLabelFadeOut;
        foreach (ImageGroup prompt in tutorialPrompts)
        {
            prompt.BeginFadeOut();
        }
    }
    virtual public void BeginLabelFadeOut() {
        if(tutorialPrompts[tutorialPrompts.Count - 1])
            tutorialPrompts[tutorialPrompts.Count - 1].OnFadeEnd -= BeginLabelFadeOut;
        isLabelFadingOut = true;
    }
    virtual public void StartTutorial()
    {
        if (tutorialPrompts[tutorialPrompts.Count - 1])
            tutorialPrompts[tutorialPrompts.Count - 1].OnFadeEnd -= StartTutorial;
    }

    virtual public void CompleteTutorial()
    {
        labelText.color = completeTextColour;
        Invoke("BeginHideTutorial", 2.5f);
    }
    virtual protected void FadeInLabel()
    {
        currAlpha = Mathf.Lerp(currAlpha, 1f, Time.deltaTime * fadeRate);
        if (currAlpha >= 0.95f)
        {
            currAlpha = 1f;
       
                labelImage.color = new Vector4(labelImage.color.r,
                    labelImage.color.g, labelImage.color.b, labelAlpha);
            labelText.color = new Vector4(labelText.color.r,
                  labelText.color.g, labelText.color.b, currAlpha);

            isLabelFading = false;
            tutorialPrompts[tutorialPrompts.Count - 1].OnFadeEnd += StartTutorial;
            foreach(ImageGroup prompt in tutorialPrompts)
            {
                prompt.BeginFadeIn();
            }
        }
        else
        {
            labelText.color = new Vector4(labelText.color.r,
              labelText.color.g, labelText.color.b, currAlpha);
            if (currAlpha <= labelAlpha)
                labelImage.color = new Vector4(labelImage.color.r,
                         labelImage.color.g, labelImage.color.b, currAlpha);
        }

    }
    virtual protected void FadeOutLabel()
    {
        currAlpha = Mathf.Lerp(currAlpha, 0f, Time.deltaTime * fadeRate);
        if (currAlpha <= 0.05f)
        {
            currAlpha = 0f;

            labelImage.color = new Vector4(labelImage.color.r,
                labelImage.color.g, labelImage.color.b, currAlpha);
            labelText.color = new Vector4(labelText.color.r,
                  labelText.color.g, labelText.color.b, currAlpha);

            isLabelFadingOut = false;
        
        }
        else
        {
            labelText.color = new Vector4(labelText.color.r,
              labelText.color.g, labelText.color.b, currAlpha);
            if (currAlpha <= labelAlpha)
                labelImage.color = new Vector4(labelImage.color.r,
                         labelImage.color.g, labelImage.color.b, currAlpha);
        }

    }
    virtual protected void Update()
    {
        if (isLabelFading)
        {
            FadeInLabel();
        }
        else if (isLabelFadingOut) FadeOutLabel();
    }
}

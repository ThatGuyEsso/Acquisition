using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class AttackTutorial : InputTutorial
{
    [SerializeField] protected bool isPrimary;

    [SerializeField] protected Image attackDescBG;
    [SerializeField] protected TextMeshProUGUI attackDescText;

    public override void CompleteTutorial()
    {
        base.CompleteTutorial();
        inputs.Disable();
    }

    public override void StartTutorial()
    {
        base.StartTutorial();
        if (isPrimary)
        {
            inputs.Attack.PrimaryAttack.performed += _ => CompleteTutorial();
        }
        else
        {
            inputs.Attack.SecondaryAttack.performed += _ => CompleteTutorial();
        }

        inputs.Enable();
    }


    protected override void FadeInLabel()
    {
        currAlpha = Mathf.Lerp(currAlpha, 1f, Time.deltaTime * fadeRate);
        if (currAlpha >= 0.95f)
        {
            currAlpha = 1f;

            labelImage.color = new Vector4(labelImage.color.r,
                labelImage.color.g, labelImage.color.b, labelAlpha);
            attackDescBG.color = new Vector4(attackDescBG.color.r,
               attackDescBG.color.g, attackDescBG.color.b, labelAlpha);

            labelText.color = new Vector4(labelText.color.r,
                  labelText.color.g, labelText.color.b, currAlpha);
            attackDescText.color = new Vector4(attackDescText.color.r,
            attackDescText.color.g, attackDescText.color.b, currAlpha);

            isLabelFading = false;
            tutorialPrompts[tutorialPrompts.Count - 1].OnFadeEnd += StartTutorial;
            foreach (ImageGroup prompt in tutorialPrompts)
            {
                prompt.BeginFadeIn();
            }
        }
        else
        {
            labelText.color = new Vector4(labelText.color.r,
              labelText.color.g, labelText.color.b, currAlpha);
            attackDescText.color = new Vector4(attackDescText.color.r,
                attackDescText.color.g, attackDescText.color.b, currAlpha);
            if (currAlpha <= labelAlpha)
            {
                labelImage.color = new Vector4(labelImage.color.r,
                         labelImage.color.g, labelImage.color.b, currAlpha);
                attackDescBG.color = new Vector4(attackDescBG.color.r,
                    attackDescBG.color.g, attackDescBG.color.b, currAlpha);
            }
        }
    }
    override protected void FadeOutLabel()
    {
        currAlpha = Mathf.Lerp(currAlpha, 0f, Time.deltaTime * fadeRate);
        if (currAlpha <= 0.05f)
        {
            currAlpha = 0f;

            labelImage.color = new Vector4(labelImage.color.r,
                labelImage.color.g, labelImage.color.b, currAlpha);

            attackDescBG.color = new Vector4(attackDescBG.color.r,
                attackDescBG.color.g, attackDescBG.color.b, currAlpha);

            labelText.color = new Vector4(labelText.color.r,
                  labelText.color.g, labelText.color.b, currAlpha);
            attackDescText.color = new Vector4(attackDescText.color.r,
                attackDescText.color.g, attackDescText.color.b, currAlpha);


            isLabelFadingOut = false;

        }
        else
        {
            labelText.color = new Vector4(labelText.color.r,
              labelText.color.g, labelText.color.b, currAlpha);

            attackDescText.color = new Vector4(attackDescText.color.r,
                attackDescText.color.g, attackDescText.color.b, currAlpha);


            if (currAlpha <= labelAlpha)
            {
                labelImage.color = new Vector4(labelImage.color.r,
                    labelImage.color.g, labelImage.color.b, currAlpha);
                attackDescBG.color = new Vector4(attackDescBG.color.r,
                     attackDescBG.color.g, attackDescBG.color.b, currAlpha);
            }

        }

    }

}

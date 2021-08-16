using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DodgeRollTutorial : InputTutorial
{
    [SerializeField] private int maxDodgerollCount;
    [SerializeField] private CounterUI counter;
    [SerializeField] private Image counterBG;
    [SerializeField] private TextMeshProUGUI cunterLabel;
    [SerializeField] private TextMeshProUGUI counterDisplay;
    private int currCount;

    public override void InitTutorial()
    {
        base.InitTutorial();
        currCount = maxDodgerollCount;
        counter.gameObject.SetActive(false);
    }

    public override void StartTutorial()
    {
        base.StartTutorial();
        inputs.DodgeRoll.Roll.performed += _ => DecrementCount();
        inputs.Enable();
    }


    public void DecrementCount()
    {
        currCount--;
        if (currCount <= 0) CompleteTutorial();
        if (!counter.gameObject.activeInHierarchy)
        {
            counter.gameObject.SetActive(true);
            counter.UpdateDisplay(currCount);
            if (currCount > 1) counter.UpdateLabel("Times");
            else counter.UpdateLabel("Time");
        }
        else
        {
            counter.UpdateDisplay(currCount);
            if (currCount > 1) counter.UpdateLabel("Times");
            else counter.UpdateLabel("Time");
        }

    }
    public override void CompleteTutorial()
    {
        base.CompleteTutorial();
        inputs.Disable();
    }

    override protected void FadeOutLabel()
    {
        currAlpha = Mathf.Lerp(currAlpha, 0f, Time.deltaTime * fadeRate);
        if (currAlpha <= 0.05f)
        {
            currAlpha = 0f;

            labelImage.color = new Vector4(labelImage.color.r,
                labelImage.color.g, labelImage.color.b, currAlpha);
            labelText.color = new Vector4(labelText.color.r,
                  labelText.color.g, labelText.color.b, currAlpha);
            cunterLabel.color = new Vector4(cunterLabel.color.r,
                cunterLabel.color.g, cunterLabel.color.b, currAlpha);

            counterDisplay.color = new Vector4(counterDisplay.color.r,
                counterDisplay.color.g, counterDisplay.color.b, currAlpha);
            counterBG.color = new Vector4(counterBG.color.r,
                counterBG.color.g, counterBG.color.b, currAlpha);
            isLabelFadingOut = false;

        }
        else
        {
            labelText.color = new Vector4(labelText.color.r,
              labelText.color.g, labelText.color.b, currAlpha);

            cunterLabel.color = new Vector4(cunterLabel.color.r,
                cunterLabel.color.g, cunterLabel.color.b, currAlpha);

            counterDisplay.color = new Vector4(counterDisplay.color.r,
                counterDisplay.color.g, counterDisplay.color.b, currAlpha);

            if (currAlpha <= labelAlpha)
            {
                labelImage.color = new Vector4(labelImage.color.r,
                    labelImage.color.g, labelImage.color.b, currAlpha);
                counterBG.color = new Vector4(counterBG.color.r,
                     counterBG.color.g, counterBG.color.b, currAlpha);
            }
  
        }

    }
}
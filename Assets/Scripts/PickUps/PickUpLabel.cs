using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PickUpLabel : MonoBehaviour
{
    [SerializeField] private string displayText;
    [SerializeField] private Color displayTextColour =Color.white;
    [SerializeField] private Vector2 padding;

    [SerializeField] private GameObject labelObject;


    [SerializeField] private bool InitOnAwake =true;
    private TextMeshProUGUI label;
    private Image labelBackground;

    private bool isActive;

    private bool inRange;
    public void SetIsActive(bool active)
    {
        if (active)
        {
            isActive = true;
            if (inRange) ShowText();
        }
        else
        {
            isActive = false;
            HideText();
        }
    }

    private void Awake()
    {
        if (InitOnAwake) Init();
    }

    public void Init()
    {
        label = GetComponentInChildren<TextMeshProUGUI>();
        labelBackground = GetComponentInChildren<Image>();
        UpdateDisplayText(displayText, displayTextColour);
        HideText();

    }
    public void ShowText()
    {
        if (!labelObject) return;
        labelObject.SetActive(true);
        label.text = displayText;
        label.ForceMeshUpdate();
        Vector2 renderBounds = label.GetRenderedValues(false);
        if(labelBackground)
            labelBackground.rectTransform.sizeDelta = renderBounds + padding;
    }
    public void UpdateDisplayText(string textToDisplay,Color textColour)
    {
        displayText = textToDisplay;
        if(label)
            label.color = textColour;
        ShowText();
    }

    public void HideText()
    {
        if (!labelObject) return;
        labelObject.SetActive(false);

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.gameObject.CompareTag("Player")){
            inRange = true;
            if(isActive)
                ShowText();
        }
    }

    //public void OnTriggerStay2D(Collider2D other)




    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")){
            inRange = false;
            HideText();
        }
    }
}


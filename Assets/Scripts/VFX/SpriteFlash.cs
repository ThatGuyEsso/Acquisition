using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpriteFlash : MonoBehaviour, IInitialisable
{
    [SerializeField] private Color flashColour;
    [SerializeField] private float flashSpeed = 0.5f;
    private SpriteRenderer[] spriteRenderers;
    private bool isFlashing = false;
    private Color currentFlashColour;
    private Color eColour;
    private bool begining = true;
    private bool isEnding;
    public void Init()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        foreach(SpriteRenderer sr in spriteRenderers)
        {
            sr.material.SetColor("_Tint", new Color(1.0f,1.0f,1.0f,0f));
        }
        currentFlashColour = flashColour;

    }

    public void Flash()
    {
        isFlashing = true;
        isEnding = false;
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.material.SetColor("_Tint", flashColour);
        }
    }

    public void EndFlash()
    {
        isEnding = true;
    }

    private void FlashToEndColour()
    {
        currentFlashColour = Vector4.Lerp(currentFlashColour, new Vector4(currentFlashColour.r, currentFlashColour.b, currentFlashColour.g,0.0f), flashSpeed * Time.deltaTime);

        foreach (SpriteRenderer sr in spriteRenderers)
            sr.material.SetColor("_Tint", currentFlashColour);

        if (Mathf.Abs(currentFlashColour.a) <=0.05f)
        {
            isEnding = false;
            isFlashing = false;
            foreach (SpriteRenderer sr in spriteRenderers)
            {
                sr.material.SetColor("_Tint", currentFlashColour);
            }
            currentFlashColour = flashColour;
        }
         
    }

    private void FlashToBegining()
    {
        eColour = Color.Lerp(eColour, currentFlashColour, flashSpeed * Time.deltaTime);

        foreach (SpriteRenderer sr in spriteRenderers)
            sr.material.SetColor("_Tint", eColour);

        if(eColour == currentFlashColour)
        {
            begining = true;
            isFlashing = false;
            currentFlashColour = currentFlashColour;
        
        }
    }

    private void Update()
    {

        if(isFlashing)
        {
            if (isEnding == true)
            {
                FlashToEndColour();
            }
        }
    }


}

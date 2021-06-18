using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpriteFlash : MonoBehaviour, IInitialisable
{
    [SerializeField] private Color beginColour;
    [SerializeField] private Color endColour;
    [SerializeField] private float flashSpeed = 0.5f;

    private SpriteRenderer[] spriteRenderers;
    private bool isFlashing = false;
    private Color bColour;
    private Color eColour;
    private bool begining = true;
    public void Init()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        foreach(SpriteRenderer sr in spriteRenderers)
        {
            sr.material.SetColor("_Tint", beginColour);
        }
        bColour = beginColour;
        eColour = endColour;
    }

    public void Flash()
    {
        isFlashing = true;
    }

    private void FlashToEndColour()
    {
        bColour = Color.Lerp(bColour, endColour, flashSpeed * Time.deltaTime);

        foreach (SpriteRenderer sr in spriteRenderers)
            sr.material.SetColor("_Tint", bColour);

        if (bColour == endColour)
            begining = false;
    }

    private void FlashToBegining()
    {
        eColour = Color.Lerp(eColour, beginColour, flashSpeed * Time.deltaTime);

        foreach (SpriteRenderer sr in spriteRenderers)
            sr.material.SetColor("_Tint", eColour);

        if(eColour == beginColour)
        {
            begining = true;
            isFlashing = false;
            bColour = beginColour;
            eColour = endColour;
        }
    }

    private void Update()
    {

        if(isFlashing)
        {
            if (begining == true)
                FlashToEndColour();
            else if (begining == false)
                FlashToBegining();
        }
    }


}

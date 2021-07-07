using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : MonoBehaviour
{
    public void OnBack()
    {
        if (!UIManager.instance) return;
        AudioManager.instance.PlayUISound("ButtonPress", transform.position);
        UIManager.instance.SwitchUI(UIManager.instance.GetPrevUI());
    }
}

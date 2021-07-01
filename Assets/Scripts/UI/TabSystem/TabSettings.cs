using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Tab Settings", menuName = "Tabs")]
public class TabSettings : ScriptableObject
{
    public Color tabIdle;
    public Color tabHovered;
    public Color tabSelected;
    public Vector3 selectedTabScaleValue;
}

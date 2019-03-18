using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUDropdown : BaseGUI
{
    public override GUIType GetUIType { get { return GUIType.Dropdown; } }

    public UnityEngine.UI.Dropdown dropdown;
}

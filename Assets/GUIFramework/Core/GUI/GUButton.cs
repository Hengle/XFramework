using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUButton : BaseGUI
{
    public override GUIType GetUIType { get { return GUIType.Button; } }

    public UnityEngine.UI.Button button;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUInputField : BaseGUI
{
    public override GUIType GetUIType { get { return GUIType.InputField; } }

    public UnityEngine.UI.InputField inputField;
}

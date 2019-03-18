using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGUI : MonoBehaviour
{
    public virtual GUIType GetUIType { get { return GUIType.None; } }
}

public enum GUIType
{
    None,
    Image,
    Button,
    Toggle,
    Dropdown,
    Text,
    ScrollRect,
    InputField,
}
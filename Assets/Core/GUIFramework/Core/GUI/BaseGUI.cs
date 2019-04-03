using UnityEngine;

namespace XDEDZL.UI
{
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
}
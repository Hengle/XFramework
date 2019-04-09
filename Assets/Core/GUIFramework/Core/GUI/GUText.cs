﻿
using UnityEngine.UI;

namespace XDEDZL.UI
{
    [UnityEngine.RequireComponent(typeof(UnityEngine.UI.Text))]
    public class GUText : BaseGUI
    {
        public Text text;
        private void Reset()
        {
            text = transform.GetComponent<Text>();
        }
    }
}
﻿using UnityEngine.UI;
using UnityEngine;

namespace XFramework.UI
{
    [RequireComponent(typeof(Scrollbar))]
    public class GUScrollbar : BaseGUI
    {
        public Scrollbar scrollbar;

        private void Reset()
        {
            scrollbar = GetComponent<Scrollbar>();
        }

        public void AddListener(UnityEngine.Events.UnityAction<float> call)
        {
            scrollbar.onValueChanged.AddListener(call);
        }
    }
}

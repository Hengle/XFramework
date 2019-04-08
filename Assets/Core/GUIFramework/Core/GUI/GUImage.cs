using UnityEngine;

namespace XDEDZL.UI
{
    [UnityEngine.RequireComponent(typeof(UnityEngine.UI.Image))]
    public class GUImage : BaseGUI
    {
        public override GUIType GetUIType { get { return GUIType.Image; } }

        public UnityEngine.UI.Image image;

        private void Reset()
        {
            image = transform.GetComponent<UnityEngine.UI.Image>();
        }
    }
}
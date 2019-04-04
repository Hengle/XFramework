using UnityEngine;

namespace XDEDZL.UI
{
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
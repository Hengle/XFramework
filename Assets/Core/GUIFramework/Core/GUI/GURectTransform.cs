namespace XDEDZL.UI
{
    public class GURectTransform : BaseGUI
    {
        public UnityEngine.RectTransform rect;

        public override GUIType GetUIType { get { return GUIType.RectTransform; } }

        private void Reset()
        {
            rect = transform.GetComponent<UnityEngine.RectTransform>();
        }
    }

}
namespace XDEDZL.UI
{
    [UnityEngine.RequireComponent(typeof(UnityEngine.RectTransform))]
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
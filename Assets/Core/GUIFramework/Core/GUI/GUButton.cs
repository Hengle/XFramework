namespace XDEDZL.UI
{
    public class GUButton : BaseGUI
    {
        public override GUIType GetUIType { get { return GUIType.Button; } }

        public UnityEngine.UI.Button button;

        private void Reset()
        {
            button = transform.GetComponent<UnityEngine.UI.Button>();
        }
    }
}
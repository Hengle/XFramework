
namespace XDEDZL.UI
{
    public class GUText : BaseGUI
    {
        public override GUIType GetUIType { get { return GUIType.Text; } }

        public UnityEngine.UI.Text text;
        private void Reset()
        {
            text = transform.GetComponent<UnityEngine.UI.Text>();
        }
    }
}
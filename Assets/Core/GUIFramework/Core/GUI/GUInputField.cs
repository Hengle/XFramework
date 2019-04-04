
namespace XDEDZL.UI
{
    public class GUInputField : BaseGUI
    {
        public override GUIType GetUIType { get { return GUIType.InputField; } }

        public UnityEngine.UI.InputField inputField;

        private void Reset()
        {
            inputField = transform.GetComponent<UnityEngine.UI.InputField>();
        }
    }
}
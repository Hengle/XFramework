
namespace XDEDZL.UI
{
    [UnityEngine.RequireComponent(typeof(UnityEngine.UI.Image))]
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
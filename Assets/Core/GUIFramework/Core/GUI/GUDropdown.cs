
namespace XDEDZL.UI
{
    public class GUDropdown : BaseGUI
    {
        public override GUIType GetUIType { get { return GUIType.Dropdown; } }

        public UnityEngine.UI.Dropdown dropdown;
        private void Reset()
        {
            dropdown = transform.GetComponent<UnityEngine.UI.Dropdown>();
        }
    }
}
using UnityEngine.UI;

namespace XDEDZL.UI
{
    [UnityEngine.RequireComponent(typeof(ScrollRect))]
    public class GUScrollRect : BaseGUI
    {
        public ScrollRect scrollRect;

        private void Reset()
        {
            scrollRect = GetComponent<ScrollRect>();
        }
    }
}
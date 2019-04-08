using UnityEngine;

namespace XDEDZL.UI
{
    [RequireComponent(typeof(LongOrDoubleBtn))]
    public class GULongOrDoubleBtn : BaseGUI
    {
        public LongOrDoubleBtn longOrDoubleBtn;

        private void Reset()
        {
            longOrDoubleBtn = GetComponent<LongOrDoubleBtn>();
        }
    }
}
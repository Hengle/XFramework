using UnityEngine.Events;

namespace XDEDZL.UI
{
    [UnityEngine.RequireComponent(typeof(Draggable))]
    public class GUDragable : BaseGUI
    {
        public Draggable draggable;

        private void Reset()
        {
            draggable = GetComponent<Draggable>();
        }

        public void AddOnBegainDrag(UnityAction call)
        {
            draggable.onBeginDrag.AddListener(call);
        }

        public void AddOnDrag(UnityAction call)
        {
            draggable.onDrag.AddListener(call);
        }

        public void AddOnEndDrag(UnityAction call)
        {
            draggable.onEndDrag.AddListener(call);
        }
    }
}
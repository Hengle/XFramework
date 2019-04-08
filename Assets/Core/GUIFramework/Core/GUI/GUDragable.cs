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
    }
}
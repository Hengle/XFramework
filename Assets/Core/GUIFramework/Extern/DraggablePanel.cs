using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 在场景中可拖拽的面板继承此类
/// </summary>
public class DraggablePanel : BasePanel,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    /// <summary>
    /// 鼠标在UI坐标上的位置
    /// </summary>
    private Vector3 globalMousePos;
    /// <summary>
    /// 鼠标落在面板上的位置和面板位置差
    /// </summary>
    private Vector3 differ;

    protected delegate void Action();
    protected event Action BeginDrag;
    protected event Action EndDrag;

    /// <summary>
    /// 拖拽开始
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out globalMousePos);
        differ = globalMousePos - rect.position;

        BeginDrag?.Invoke();
    }

    /// <summary>
    ///  拖拽中
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        //并将拖拽时的坐标给予被拖拽对象的代替品
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            rect.position = globalMousePos - differ;
        }
    }

    /// <summary>
    /// 拖拽结束
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        EndDrag?.Invoke();
    }
}
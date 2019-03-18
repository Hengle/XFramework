using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 可长按按钮
/// </summary>
public class LongPressBtn : UnityEngine.UI.Button
{
    /// <summary>
    /// 长按最大时间，超过后系数为1
    /// 单位 秒
    /// </summary>
    public float maxTime;
    public LongClickEvent onLongClick = new LongClickEvent();

    private float startTime;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        startTime = Time.time;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (maxTime <= 0)
            throw new System.Exception("时间初始值不得小于或等于0");
        onLongClick.Invoke(Mathf.Min(1, (Time.time - startTime) / maxTime));
    }

    public class LongClickEvent : UnityEvent<float> { }
}
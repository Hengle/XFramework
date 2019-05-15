using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 对Button进行扩展
/// </summary>
public class ButtonExt : Button
{
    /// <summary>
    /// 鼠标悬停事件 
    /// </summary>
    private UnityAction OnHighlighted;
    /// <summary>
    /// 鼠标离开事件
    /// </summary>
    private UnityAction OnNormal;
    /// <summary>
    /// 鼠标悬停时是否需要改变大小
    /// </summary>
    public bool isChangeSize = true;

    private RectTransform rect;
    private Vector2 rectSize;
    
    public int CurrentBtnState
    {
        get { return (int)currentSelectionState; }
    }

    protected override void Start()
    {
        base.Start();
        rect = GetComponent<RectTransform>();
        rectSize = rect.sizeDelta;
        if (isChangeSize)
        {
            AddHightLighted(() => { ChangeSize(rect, new Vector2(209, 93)); });
            AddNormal(() => { ChangeSize(rect, rectSize); });
        }
    }

    /// <summary>
    /// 添加悬停事件，方便外部调用
    /// </summary>
    /// <param name="call"></param>
    public void AddHightLighted(UnityAction call)
    {
        OnHighlighted += call;
    }

    /// <summary>
    /// 添加鼠标移出事件，方便外部调用
    /// </summary>
    /// <param name="call"></param>
    public void AddNormal(UnityAction call)
    {
        OnNormal += call;
    }

    /// <summary>
    /// 状态切换时被调用
    /// </summary>
    protected override void DoStateTransition(SelectionState state, bool instant)
    {

        base.DoStateTransition(state, instant);

        switch (state)
        {
            case SelectionState.Highlighted:
                OnHighlighted?.Invoke();
                break;
            case SelectionState.Normal:
                OnNormal?.Invoke();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 改变按钮的大小
    /// </summary>
    protected void ChangeSize(RectTransform btnRect, Vector2 size)
    {
        btnRect.DOSizeDelta(size, 0.2f);
    }
}

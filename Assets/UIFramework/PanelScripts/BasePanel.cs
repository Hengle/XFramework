using UnityEngine;
using System.Collections;
using DG.Tweening;

public enum UILevel
{
    One,
    Two,
    Three,
    Ten,
}

public class BasePanel : MonoBehaviour {

    /// <summary>
    /// UI层级
    /// </summary>
    [HideInInspector] public UILevel level = UILevel.One;
    protected CanvasGroup canvasGroup;

    protected RectTransform rect;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
    }

    /// <summary>
    /// 面板初始化，只会执行一次，在Awake后start前执行
    /// </summary>
    public virtual void Init()
    {
        Vector3 rectSize = rect.localScale;
        rectSize.y = 0;
        rect.localScale = rectSize;
    }


    private void Update() { }

    /// <summary>
    /// 界面被显示出来
    /// </summary>
    public virtual void OnEnter()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        rect.DOScaleY(0.7f, 0.1f);
        canvasGroup.interactable = true;
        transform.SetAsLastSibling();
    }

    /// <summary>
    /// 界面不显示,退出这个界面，界面被关闭
    /// </summary>
    public virtual void OnExit()
    {
        rect.DOScaleY(0, 0.1f);
        canvasGroup.interactable = false;
    }
}

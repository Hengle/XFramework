using UnityEngine;
using System.Collections;
using DG.Tweening;

public enum UILevel
{
    One,
    Two,
    Three,
}

public class BasePanel : MonoBehaviour {

    /// <summary>
    /// UI层级
    /// </summary>
    public UILevel level = UILevel.One;
    protected CanvasGroup canvasGroup;

    protected RectTransform rect;

    protected virtual void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
    }

    /// <summary>
    /// 界面被显示出来
    /// </summary>
    public virtual void OnEnter()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        rect.DOScaleY(1, 0.5f);
        canvasGroup.interactable = true;
        transform.SetAsLastSibling();
    }

    /// <summary>
    /// 界面不显示,退出这个界面，界面被关系
    /// </summary>
    public virtual void OnExit()
    {
        rect.DOScaleY(0, 0.1f);
        canvasGroup.interactable = false;
    }
}

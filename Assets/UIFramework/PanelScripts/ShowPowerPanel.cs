using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShowPowerPanel : BasePanel {

    protected override void Awake()
    {
        base.Awake();
        level = UILevel.Two;
    }

    /// <summary>
    /// 界面被显示出来
    /// </summary>
    public override void OnEnter()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        rect.DOScaleY(1.0f, 0.1f);
        canvasGroup.interactable = true;
        transform.SetAsLastSibling();
    }

    /// <summary>
    /// 界面不显示,退出这个界面，界面被关闭
    /// </summary>
    public override void OnExit()
    {
        rect.DOScaleY(0, 0.1f);
        canvasGroup.interactable = false;
    }
}

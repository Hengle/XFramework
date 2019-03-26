using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AdjustPanel : BasePanel {

	public override void Init(GameObject _gameObject)
    {
        base.Init(_gameObject);
        level = UILevel.Two;
    }

    /// <summary>
    /// 界面被显示出来
    /// </summary>
    public override void OnEnter()
    {
        if (canvasGroup == null)
            canvasGroup = transform.GetComponent<CanvasGroup>();
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

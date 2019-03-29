using UnityEngine;
using DG.Tweening;

/// <summary>
/// 十八队界面
/// </summary>
public class TeamPanel : BasePanel {

    private Vector2 rectSize;

    public override void Init(GameObject _gameObject)
    {
        base.Init(_gameObject);
        level = 3;
        rectSize = rect.sizeDelta;
        CreatePanel createPanel = (CreatePanel)UIManager.Instance.GetPanel(UIName.Create);
        // 设父物体以及自己在子物体中的顺序
        transform.SetParent(createPanel.teamBtn.transform.parent, true);
        transform.SetSiblingIndex(createPanel.teamBtn.transform.GetSiblingIndex() + 1);
        Vector2 size = rect.sizeDelta;
        size.y = 1.5f;
        rect.sizeDelta = size;
    }

    public override void OnEnter()
    {
        if (canvasGroup == null)
            canvasGroup = transform.GetComponent<CanvasGroup>();
        rect.DOSizeDelta(rectSize, 0.3f); // 进场动画
        canvasGroup.interactable = true;
    }

    public override void OnExit()
    {
        rect.DOSizeDelta(new Vector2(rectSize.x, 1.5f), 0.3f); // 退出动画
        canvasGroup.interactable = false;
    }
}

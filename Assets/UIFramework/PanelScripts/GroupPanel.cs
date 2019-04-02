using UnityEngine;
using DG.Tweening;

/// <summary>
/// 五群界面
/// </summary>
public class GroupPanel : BasePanel {

    private Vector2 rectSize;
    protected CanvasGroup canvasGroup;

    public override void Reg()
    {
        Level = 3;
        rectSize = rect.sizeDelta;
        
        CreatePanel createPanel = (CreatePanel)UIHelper.Instance.GetPanel(UIName.Create);
        // 设父物体以及自己在子物体中的顺序
        transform.SetParent(createPanel.groupBtn.transform.parent, true);
        transform.SetSiblingIndex(createPanel.groupBtn.transform.GetSiblingIndex() + 1);
        Vector2 size = rect.sizeDelta;
        size.y = 1.5f;
        rect.sizeDelta = size;
    }

    /// <summary>
    /// 进入该按钮状态
    /// </summary>
    public override void OnEnter()
    {
        if (canvasGroup == null)
            canvasGroup = transform.GetComponent<CanvasGroup>();
        rect.DOSizeDelta(rectSize, 0.3f); // 进场动画
        canvasGroup.interactable = true;
    }

    /// <summary>
    /// 退出该按钮状态
    /// </summary>
    public override void OnExit()
    {
        rect.DOSizeDelta(new Vector2(rectSize.x, 1.5f), 0.3f); // 退出动画
        canvasGroup.interactable = false;
    }
}

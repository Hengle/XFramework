using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using RCXC;

public class CommandPostPanel : BasePanel {

    private Vector2 rectSize;

    // Use this for initialization
    protected override void Awake () {
        base.Awake();
        level = UILevel.Three;
        rectSize = rect.sizeDelta;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 鼠标点击事件
    /// </summary>
    /// <param name="modelName"> 名字 </param>
    private void OnClick(string modelName)
    {

    }

    public override void Init()
    {
        CreatePanel createPanel = (CreatePanel)UIManager.Instance.GetPanel(UIPanelType.Create);
        // 设父物体以及自己在子物体中的顺序
        transform.SetParent(createPanel.commandPostBtn.transform.parent, true);
        transform.SetSiblingIndex(createPanel.commandPostBtn.transform.GetSiblingIndex() + 1);
        Vector2 size = rect.sizeDelta;
        size.y = 1.5f;
        rect.sizeDelta = size;
    }

    public override void OnEnter()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        rect.DOSizeDelta(rectSize, 0.3f); // 进场动画
        canvasGroup.interactable = true;
    }

    public override void OnExit()
    {
        rect.DOSizeDelta(new Vector2(rectSize.x, 1.5f), 0.3f); // 退出动画
        canvasGroup.interactable = false;
    }
}

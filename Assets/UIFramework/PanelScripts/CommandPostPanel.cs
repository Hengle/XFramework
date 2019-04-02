using UnityEngine;
using DG.Tweening;

public class CommandPostPanel : BasePanel {

    private Vector2 rectSize;
    protected CanvasGroup canvasGroup;

    // Use this for initialization
    public override void Reg()
    {
        Level = 3;
        rectSize = rect.sizeDelta;

        CreatePanel createPanel = (CreatePanel)UIHelper.Instance.GetPanel(UIName.Create);
        // 设父物体以及自己在子物体中的顺序
        transform.SetParent(createPanel.commandPostBtn.transform.parent, true);
        transform.SetSiblingIndex(createPanel.commandPostBtn.transform.GetSiblingIndex() + 1);
        Vector2 size = rect.sizeDelta;
        size.y = 1.5f;
        rect.sizeDelta = size;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    /// <summary>
    /// 鼠标点击事件
    /// </summary>
    /// <param name="modelName"> 名字 </param>
    private void OnClick(string modelName)
    {

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

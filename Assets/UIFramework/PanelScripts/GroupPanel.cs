using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 五群界面
/// </summary>
public class GroupPanel : BasePanel {

    // 五群的按钮
    private Button QYZGQ_M;
    private Button QYZGQ_A;
    private Button ZSGJQ;
    private Button PBQ;
    private Button ZSBZQ;

    // 五群对应的预制体的名字，用于对象池的创建
    private const string name_QYZGQ_M = "Tank_96A";
    private const string name_QYZGQ_A = "Armortruck04";
    private const string name_ZSGJQ = "cn_whe_08wheeled";
    private const string name_PBQ = "cn_whe_ZZH09_122";
    private const string name_ZSBZQ = "cn_tra_zbd05z";

    private Vector2 rectSize;

    protected override void Awake ()
    {
        base.Awake();
        level = UILevel.Three;
        rectSize = rect.sizeDelta;
        // 按钮赋值
        QYZGQ_M = transform.Find("Viewport/Content/QYZGQ_M").GetComponent<Button>();
        QYZGQ_A = transform.Find("Viewport/Content/QYZGQ_A").GetComponent<Button>();
        ZSGJQ = transform.Find("Viewport/Content/ZSGJQ").GetComponent<Button>();
        PBQ = transform.Find("Viewport/Content/PBQ").GetComponent<Button>();
        ZSBZQ = transform.Find("Viewport/Content/ZSBZQ").GetComponent<Button>();

        // 监听点击事件
        QYZGQ_M.onClick.AddListener(() => { OnClick(name_QYZGQ_M); });
        QYZGQ_A.onClick.AddListener(() => { OnClick(name_QYZGQ_A); });
        ZSGJQ.onClick.AddListener(() => { OnClick(name_ZSGJQ); });
        PBQ.onClick.AddListener(() => { OnClick(name_PBQ); });
        ZSBZQ.onClick.AddListener(() => { OnClick(name_ZSBZQ); });
    }

    /// <summary>
    /// 鼠标点击事件
    /// </summary>
    /// <param name="modelName"> 名字 </param>
    private void OnClick(string modelName)
    {
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public override void Init()
    {
        CreatePanel createPanel = (CreatePanel)Singleton<UIManager>.Instance.GetPanel(UIPanelType.Create);
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
            canvasGroup = GetComponent<CanvasGroup>();
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

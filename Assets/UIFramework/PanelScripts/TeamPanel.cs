using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 十八队界面
/// </summary>
public class TeamPanel : BasePanel {

    // 申明十八队按钮
    private Button ZDYBD;
    private Button QBZCD;
    private Button FKBD;
    private Button FZJD;
    private Button TXBZD;
    private Button HFFWD;
    private Button JJZDD;
    private Button TZZDD;
    private Button DZDKD;
    private Button XLZD;
    private Button ZASZD;
    private Button ZAPCD;
    private Button YDBZD;
    private Button GZYBD;
    private Button FHBZD;
    private Button CCYHZDD;
    private Button KZHLTJD;
    private Button XQZDD;

    // 十八队对应的预制体的名字，用于对象池的创建
    private const string modelName = "Tank_96A";

    private Vector2 rectSize;

    // Use this for initialization
    protected override void Awake () {

        base.Awake();
        level = UILevel.Three;
        rectSize = rect.sizeDelta;

        // 按钮赋值
        ZDYBD = transform.Find("Viewport/Content/ZDYBD").GetComponent<Button>();
        QBZCD = transform.Find("Viewport/Content/QBZCD").GetComponent<Button>();
        FKBD = transform.Find("Viewport/Content/FKBD").GetComponent<Button>();
        FZJD = transform.Find("Viewport/Content/FZJD").GetComponent<Button>();
        TXBZD = transform.Find("Viewport/Content/TXBZD").GetComponent<Button>();
        HFFWD = transform.Find("Viewport/Content/HFFWD").GetComponent<Button>();
        JJZDD = transform.Find("Viewport/Content/JJZDD").GetComponent<Button>();
        TZZDD = transform.Find("Viewport/Content/TZZDD").GetComponent<Button>();
        DZDKD = transform.Find("Viewport/Content/DZDKD").GetComponent<Button>();
        XLZD = transform.Find("Viewport/Content/XLZD").GetComponent<Button>();
        ZASZD = transform.Find("Viewport/Content/ZASZD").GetComponent<Button>();
        ZAPCD = transform.Find("Viewport/Content/ZAPCD").GetComponent<Button>();
        YDBZD = transform.Find("Viewport/Content/YDBZD").GetComponent<Button>();
        GZYBD = transform.Find("Viewport/Content/GZYBD").GetComponent<Button>();
        FHBZD = transform.Find("Viewport/Content/FHBZD").GetComponent<Button>();
        CCYHZDD = transform.Find("Viewport/Content/CCYHZDD").GetComponent<Button>();
        KZHLTJD = transform.Find("Viewport/Content/KZHLTJD").GetComponent<Button>();
        XQZDD = transform.Find("Viewport/Content/XQZDD").GetComponent<Button>();

        // 注册按钮的点击事件
        ZDYBD.onClick.AddListener(() => { OnClick(modelName); });
        QBZCD.onClick.AddListener(() => { OnClick(modelName); });
        FKBD.onClick.AddListener(() => { OnClick(modelName); });
        FZJD.onClick.AddListener(() => { OnClick(modelName); });
        TXBZD.onClick.AddListener(() => { OnClick(modelName); });
        HFFWD.onClick.AddListener(() => { OnClick(modelName); });
        JJZDD.onClick.AddListener(() => { OnClick(modelName); });
        TZZDD.onClick.AddListener(() => { OnClick(modelName); });
        DZDKD.onClick.AddListener(() => { OnClick(modelName); });
        XLZD.onClick.AddListener(() => { OnClick(modelName); });
        ZASZD.onClick.AddListener(() => { OnClick(modelName); });
        ZAPCD.onClick.AddListener(() => { OnClick(modelName); });
        YDBZD.onClick.AddListener(() => { OnClick(modelName); });
        GZYBD.onClick.AddListener(() => { OnClick(modelName); });
        FHBZD.onClick.AddListener(() => { OnClick(modelName); });
        CCYHZDD.onClick.AddListener(() => { OnClick(modelName); });
        KZHLTJD.onClick.AddListener(() => { OnClick(modelName); });
        XQZDD.onClick.AddListener(() => { OnClick(modelName); });
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
        transform.SetParent(createPanel.teamBtn.transform.parent, true);
        transform.SetSiblingIndex(createPanel.teamBtn.transform.GetSiblingIndex() + 1);
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

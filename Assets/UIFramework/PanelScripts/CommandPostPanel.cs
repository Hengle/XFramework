﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CommandPostPanel : BasePanel {

    private Button basicCommandBtn;    // 基本指挥所
    private Button advanceCommandBtn;  // 前进指挥所
    private Button rearCommandBtn;     // 后方指挥所

    private const string modelName = "Tank_96A";

    private Vector2 rectSize;

    // Use this for initialization
    protected override void Awake () {
        base.Awake();
        level = UILevel.Three;
        rectSize = rect.sizeDelta;

        // 按钮赋值
        basicCommandBtn = transform.Find("Viewport/Content/BasicBtn").GetComponent<Button>();
        advanceCommandBtn = transform.Find("Viewport/Content/AdvanceBtn").GetComponent<Button>();
        rearCommandBtn = transform.Find("Viewport/Content/RearBtn").GetComponent<Button>();

        // 注册按钮事件
        basicCommandBtn.onClick.AddListener(() => { OnClick(modelName); });
        advanceCommandBtn.onClick.AddListener(() => { OnClick(modelName); });
        rearCommandBtn.onClick.AddListener(() => { OnClick(modelName); });
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
        // 在创建单位状态创建对应单位
        if (MouseEvent.Instance.CurrentStateType == MouseStateType.CreateArmyState)
        {
            // 当前有选中单位时，将其取消
            (MouseEvent.Instance.CurrentState as MouseCreateArmyState)?.gameObj?.SetActive(false);    // 取消需要创建的单位

            // 重新实例化一个单位
            //GameObject obj = Singleton<GameObjectFactory>.Instance.Instantiate(modelName);
            //MouseEvent.Instance.CurrentState.OnActive(obj);
        }
        else
        {
            // 非创建单位状态
            //GameObject obj = Singleton<GameObjectFactory>.Instance.Instantiate(modelName);
            //MouseEvent.Instance.ChangeState(MouseStateType.CreateArmyState, obj);
        }
    }

    public override void Init()
    {
        CreatePanel createPanel = (CreatePanel)Singleton<UIManager>.Instance.GetPanel(UIPanelType.Create);
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

        (MouseEvent.Instance.CurrentState as MouseCreateArmyState)?.gameObj?.SetActive(false);    // 取消需要创建的单位

    }
}

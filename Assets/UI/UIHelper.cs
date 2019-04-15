﻿using System;
using UnityEngine;
using UnityEngine.UI;
using XDEDZL.UI;

/// <summary>
/// UI管理助手
/// </summary>
public class UIHelper : Singleton<UIHelper>
{
    /// <summary>
    /// UI管理器
    /// </summary>
    private IUIManager UImanager;

    private Transform canvasTransform;
    private Transform CanvasTransform
    {
        get
        {
            if (canvasTransform == null)
            {
                canvasTransform = GameObject.Find("Canvas").transform;
            }
            return canvasTransform;
        }
    }

    /// <summary>
    /// 提示
    /// </summary>
    private RectTransform tipRect;
    /// <summary>
    /// 提示
    /// </summary>
    private Text tipText;

    /// <summary>
    /// 确认操作的委托
    /// </summary>
    private Action VerifyOperate;
    /// <summary>
    /// 确认面板的显示文字
    /// </summary>
    private Text VerifyText;

    private UIHelper()
    {
        UImanager = new UIMgrDicType();
        InitTip();
    }

    /// <summary>
    /// 打开面板
    /// </summary>
    public void OpenPanel(string name)
    {
        UImanager.OpenPanel(name);
    }

    /// <summary>
    /// 关闭面板
    /// </summary>
    /// <param name="name"></param>
    public void ClosePanel(string name)
    {
        UImanager.ClosePanel(name);
    }

    /// <summary>
    /// 获取面板
    /// </summary>
    public BasePanel GetPanel(string name)
    {
        return UImanager.GetPanel(name);
    }

    /// <summary>
    /// 关闭最近打开的面板
    /// </summary>
    public void CloseTopPanel()
    {
        UImanager.CloseTopPanel();
    }

    /// <summary>
    /// 初始化提示
    /// </summary>
    private void InitTip()
    {
        tipRect = CanvasTransform.Find("Tip").GetComponent<RectTransform>();
        tipText = tipRect.GetComponent<Text>();
    }

    /// <summary>
    /// 显示提示
    /// </summary>
    public void ShowTip(string content)
    {
        tipRect.localPosition = Vector3.zero;
        tipText.color = Color.red;
        tipText.text = content;
    }

    private void InitVerify()
    {
    }

    /// <summary>
    /// 执行确认委托
    /// 除了VerifyPanel面板 不要调用
    /// </summary>
    public void ExecuteVerifyOperate()
    {
        VerifyOperate?.Invoke();
    }

    /// <summary>
    /// 打开确认面板
    /// </summary>
    /// <param name="showText"></param>
    /// <param name="action"></param>
    public void OpenVerifyOperateTip(string showText, Action action)
    {
        VerifyOperate = action;
        VerifyText.text = showText;
        OpenPanel(UIName.Verify);
    }
}
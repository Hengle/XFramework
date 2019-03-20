using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;
using System;
/// <summary>
/// 创建单位按钮
/// </summary>
public class CreatePanel : BasePanel {

    [HideInInspector] public ButtonExt groupBtn;        // 五群按钮
    [HideInInspector] public ButtonExt teamBtn;         // 十八队按钮
    [HideInInspector] public ButtonExt commandPostBtn;  // 三所按钮
     
    private RectTransform groupRect;
    private RectTransform teamRect;
    private RectTransform comandPostRect;

    private const float groupHeight = 420;
    private const float teamHeight = 533;
    private const float comandPosHeight = 336;

    protected override void Awake()
    {
        base.Awake();
        level = UILevel.Two;
        // 按钮赋值
        groupBtn = transform.Find("Viewport/Content/GroupBtn").GetComponent<ButtonExt>();
        teamBtn = transform.Find("Viewport/Content/TeamBtn").GetComponent<ButtonExt>();
        commandPostBtn = transform.Find("Viewport/Content/CommandPostBtn").GetComponent<ButtonExt>();

        groupRect = groupBtn.GetComponent<RectTransform>();
        teamRect = teamBtn.GetComponent<RectTransform>();
        comandPostRect = commandPostBtn.GetComponent<RectTransform>();

        // 注册鼠标点击事件
        groupBtn.onClick.AddListener(() => { OnClick(UIPanelType.Group); });
        teamBtn.onClick.AddListener(() => { OnClick(UIPanelType.Team); });
        commandPostBtn.onClick.AddListener(() => { OnClick(UIPanelType.CommandPost); });

        groupBtn.AddNormal(OutButton);
        teamBtn.AddNormal(OutButton);
        commandPostBtn.AddNormal(OutButton);
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

    /// <summary>
    /// 点击事件
    /// </summary>
    /// <param name="panelType"></param>
    private void OnClick(UIPanelType panelType)
    {
        UIManager.Instance.PushPanel(panelType);
        

        Vector2 rectSize = rect.sizeDelta;
        switch (panelType)
        {
            case UIPanelType.Group:
                rectSize.y = groupHeight;
                break;
            case UIPanelType.Team:
                rectSize.y = teamHeight;
                break;
            case UIPanelType.CommandPost:
                rectSize.y = comandPosHeight;
                break;
            default:
                break;
        }
        rect.sizeDelta = rectSize;
    }

    private void InButton(RectTransform rectTran, string str_1,string str_2 = "") => DelayIn(rectTran, str_1,str_2);
    private void OutButton() => DelayOut();

    /// <summary>
    /// 延迟播放显示动画，延迟时间跟按钮变大时间一样
    /// </summary>
    private async void DelayIn(RectTransform rectTran, string str_1, string str_2)
    {
        await Task.Delay(TimeSpan.FromSeconds(0.2f));
        if (rectTran.GetComponent<ButtonExt>().CurrentBtnState == 1)
        {
            float y = rectTran.position.y;
            UIManager.Instance.ShowDescribe(y, str_1, str_2);
        }
    }

    /// <summary>
    /// 延迟播放消失动画
    /// </summary>
    private async void DelayOut()
    {
        await Task.Delay(TimeSpan.FromSeconds(0.2f));
        UIManager.Instance.CloseDescribe();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : BasePanel {

    Button createBtn;
    Button powerBtn;
    Button AdjustBtn;

    protected override void Awake()
    {
        base.Awake();
        level = UILevel.One;
        createBtn = transform.Find("CreateBtn").GetComponent<Button>();
        powerBtn = transform.Find("PowerBtn").GetComponent<Button>();
        AdjustBtn = transform.Find("AdjustBtn").GetComponent<Button>();
        createBtn.onClick.AddListener(() => { OnClick(UIPanelType.Create); });
        powerBtn.onClick.AddListener(() => { OnClick(UIPanelType.ShowPower); });
        AdjustBtn.onClick.AddListener(() => { OnClick(UIPanelType.AdjustPosture); });
    }

    /// <summary>
    /// 处理各个按钮的点击事件
    /// </summary>
    /// <param name="_type"></param>
    private void OnClick(UIPanelType _type)
    {
        Singleton<UIManager>.Instance.PushPanel(_type);
        if (MouseEvent.Instance.CurrentStateType != MouseStateType.DefaultState)
        {
            MouseEvent.Instance.ChangeState(MouseStateType.DefaultState);
        }
    }
}

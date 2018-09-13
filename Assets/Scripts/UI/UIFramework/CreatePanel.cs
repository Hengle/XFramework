using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatePanel : BasePanel {

    private Button groupBtn;        // 五群按钮
    private Button teamBtn;         // 十八队按钮
    private Button commandPostBtn;  // 三所按钮

    protected override void Start()
    {
        base.Start();
        // 按钮赋值
        groupBtn = transform.Find("Viewport/Content/GroupBtn").GetComponent<Button>();
        teamBtn = transform.Find("Viewport/Content/TeamBtn").GetComponent<Button>();
        commandPostBtn = transform.Find("Viewport/Content/CommandPostBtn").GetComponent<Button>();

        // 注册鼠标事件
        groupBtn.onClick.AddListener(() => { OnClick(UIPanelType.Group); });
        teamBtn.onClick.AddListener(() => { OnClick(UIPanelType.Team); });
        commandPostBtn.onClick.AddListener(() => { OnClick(UIPanelType.CommandPost); });
    }

    //点击事件
    private void OnClick(UIPanelType panelType)
    {
        UIManager.Instance.PushPanel(panelType);
    }
}

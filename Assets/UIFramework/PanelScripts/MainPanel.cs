using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : BasePanel {

    Button createBtn;
    Button powerBtn;
    Button AdjustBtn;

    public override void Init(GameObject _gameobject)
    {
        base.Init(_gameobject);
        level = UILevel.One;
        createBtn = transform.Find("CreateBtn").GetComponent<Button>();
        powerBtn = transform.Find("PowerBtn").GetComponent<Button>();
        AdjustBtn = transform.Find("AdjustBtn").GetComponent<Button>();
        createBtn.onClick.AddListener(() => { OnClick(UIName.Create); });
        powerBtn.onClick.AddListener(() => { OnClick(UIName.ShowPower); });
        AdjustBtn.onClick.AddListener(() => { OnClick(UIName.Adjust); });
    }

    /// <summary>
    /// 处理各个按钮的点击事件
    /// </summary>
    /// <param name="_type"></param>
    private void OnClick(string _type)
    {
        UIManager.Instance.PushPanel(_type);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : BasePanel {

    Button createBtn;

    protected override void Start()
    {
        base.Start();
        createBtn = transform.Find("CreateBtn").GetComponent<Button>();
        createBtn.onClick.AddListener(OnCreateClick);
    }

    private void OnCreateClick()
    {
        UIManager.Instance.PushPanel(UIPanelType.Create);
    } 
}

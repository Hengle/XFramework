using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AdjustPanel : BasePanel {

    private ButtonExt attackBtn;       // 攻击按钮
    private ButtonExt moveBtn;         // 移动按钮

    private RectTransform attackRect;
    private RectTransform moveRect;
    
	protected override void Awake ()
    {
        base.Awake();
        level = UILevel.Two;
        // 按钮赋值
        attackBtn = transform.Find("Viewport/Content/AttackBtn").GetComponent<ButtonExt>();
        moveBtn = transform.Find("Viewport/Content/MoveBtn").GetComponent<ButtonExt>();
        attackRect = attackBtn.GetComponent<RectTransform>();
        moveRect = moveBtn.GetComponent<RectTransform>();

        // 注册鼠标事件
        attackBtn.onClick.AddListener(() => { OnClick(MouseStateType.AttackState); });
        moveBtn.onClick.AddListener(() => { OnClick(MouseStateType.MoveState); });
    }

    //点击事件
    private void OnClick(MouseStateType _type)
    {
    }
}

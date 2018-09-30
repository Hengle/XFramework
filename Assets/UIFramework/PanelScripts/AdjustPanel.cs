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

#if false
        // 注册按钮高亮状态事件
        attackBtn.AddHightLighted(() => { ChangeSize(attackRect, 209, 93); });
        moveBtn.AddHightLighted(() => { ChangeSize(moveRect, 209, 93); });

        // 注册按钮常态事件
        attackBtn.AddNormal(() => { ChangeSize(attackRect, 169.31f, 53.41333f); });
        moveBtn.AddNormal(() => { ChangeSize(moveRect, 169.31f, 53.41333f); });
#endif
    }

    //点击事件
    private void OnClick(MouseStateType _type)
    {
        MouseEvent.Instance.ChangeState(_type);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShowPowerPanel : BasePanel {

    private ButtonExt airDefenceBtn;        // 防空火力范围
    private ButtonExt attackRangeBtn;       // 攻击范围
    private ButtonExt artilleryBtn;         // 炮兵火力范围
    private ButtonExt clearAllBtn;          // 清空所有范围的显示

    //private RectTransform airDefenceRect;
    //private RectTransform attackRangeRect;
    //private RectTransform artilleryRect;
    //private RectTransform clearAllRect;

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();
        level = UILevel.Two;
        // 按钮赋值
        airDefenceBtn = transform.Find("Viewport/Content/AirDefenceBtn").GetComponent<ButtonExt>();
        attackRangeBtn = transform.Find("Viewport/Content/AttackRangeBtn").GetComponent<ButtonExt>();
        artilleryBtn = transform.Find("Viewport/Content/ArtilleryBtn").GetComponent<ButtonExt>();
        clearAllBtn = transform.Find("Viewport/Content/ClearAllBtn").GetComponent<ButtonExt>();

        //airDefenceRect = airDefenceBtn.GetComponent<RectTransform>();
        //attackRangeRect = attackRangeBtn.GetComponent<RectTransform>();
        //artilleryRect = artilleryBtn.GetComponent<RectTransform>();
        //clearAllRect = clearAllBtn.GetComponent<RectTransform>();

        // 按钮事件注册
        airDefenceBtn.onClick.AddListener(() => { OnClick(MouseStateType.AirDefenceState); });
        attackRangeBtn.onClick.AddListener(() => { OnClick(MouseStateType.AttackRangeState); });
        artilleryBtn.onClick.AddListener(() => { OnClick(MouseStateType.ArtilleryRangeState); });
        clearAllBtn.onClick.AddListener(OnClearAllClick);

#if false
        //// 注册按钮高亮状态事件
        //airDefenceBtn.AddHightLighted(() => { ChangeSize(airDefenceRect, 209, 93); });
        //attackRangeBtn.AddHightLighted(() => { ChangeSize(attackRangeRect, 209, 93); });
        //artilleryBtn.AddHightLighted(() => { ChangeSize(artilleryRect, 209, 93); });
        //clearAllBtn.AddHightLighted(() => { ChangeSize(clearAllRect, 209, 93); });

        //// 注册按钮常态事件
        //airDefenceBtn.AddNormal(() => { ChangeSize(airDefenceRect, 169.31f, 53.41333f); });
        //attackRangeBtn.AddNormal(() => { ChangeSize(attackRangeRect, 169.31f, 53.41333f); });
        //artilleryBtn.AddNormal(() => { ChangeSize(artilleryRect, 169.31f, 53.41333f); });
        //clearAllBtn.AddNormal(() => { ChangeSize(clearAllRect, 169.31f, 53.41333f); });
#endif
    }

    private void OnClick(MouseStateType _type)
    {
        MouseEvent.Instance.ChangeState(_type);
    }

    private void OnClearAllClick()
    {
        //Singleton<RangeManger>.Instance.ClearRange();
        MouseEvent.Instance.ChangeState(MouseStateType.DefaultState);
    }
}

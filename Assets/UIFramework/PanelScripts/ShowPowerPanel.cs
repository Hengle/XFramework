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
    }

    private void OnClick(MouseStateType _type)
    {
    }

    private void OnClearAllClick()
    {
        //Singleton<RangeManger>.Instance.ClearRange();
    }
}

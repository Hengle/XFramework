// ==========================================
// 描述： 
// 作者： HAK
// 时间： 2018-11-26 09:08:29
// 版本： V 1.0
// ==========================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TerrainModifierPanel : BasePanel
{

    /// <summary>
    /// 范围的滑动器输入框组合
    /// </summary>
    public SliderMixInput rangeMix { get; private set; }
    /// <summary>
    /// 力度的滑动器输入框组合
    /// </summary>
    public SliderMixInput opticalMix { get; private set; }
    /// <summary>
    /// 地形修改模式
    /// </summary>
    public ModifierType modifierType { get; private set; }

    /// <summary>
    /// 创建路
    /// </summary>
    private Button creatRoadBtn;
    private Button deleteRoadBtn;

    protected override void Awake()
    {
        rect = GetComponent<RectTransform>();

        // 初始化组件
        rangeMix = transform.Find("Range").GetComponent<SliderMixInput>();
        opticalMix = transform.Find("Optical").GetComponent<SliderMixInput>();

    }

    private void Start()
    {
        rangeMix.SetMinMax(10, 1000);
        opticalMix.SetMinMax(1, 10);
        transform.Find("Dropdown").GetComponent<Dropdown>().onValueChanged.AddListener((a) =>
        {
            modifierType = (ModifierType)a;
            switch (modifierType)
            {
                case ModifierType.Up:
                case ModifierType.Down:
                    opticalMix.SetMinMax(1, 10);
                    SetShow(true);
                    break;
                case ModifierType.Smooth:
                    opticalMix.SetMinMax(0.5f, 1.5f);
                    SetShow(true);
                    break;
                case ModifierType.AddTree:
                    opticalMix.SetMinMax(1, 100);
                    SetShow(true);
                    break;
                case ModifierType.AddDetial:
                    opticalMix.SetMinMax(1, 10);
                    SetShow(true);
                    break;
                case ModifierType.BuildBridge:
                case ModifierType.BuildRoad:
                    SetShow(false);
                    break;
                default:
                    break;
            }

            // 通过类型修改投影的显示
            MouseEvent.Instance.GetState<MouseTerrainModifierState>(MouseStateType.TerrainModifier).OnDropDownChange(a);
        });
    }

    public override void OnEnter()
    {
        rect.DOScaleY(1.0f, 0.1f);
    }

    public override void OnExit()
    {
        rect.DOScaleY(0f, 0.1f);

        // 这个是临时的
        MouseEvent.Instance.GetState<MouseTerrainModifierState>(MouseStateType.TerrainModifier).SetProjectorShow(false);
    }

    public enum ModifierType
    {
        Up,          // 上升高度
        Down,        // 降低高度
        Smooth,      // 平滑地面 
        AddTree,     // 种树
        AddDetial,   // 种草
        BuildBridge, // 建桥
        BuildRoad,   // 建路
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isSlider"></param>
    public void SetShow(bool isSlider)
    {
        rangeMix.gameObject.SetActive(isSlider);
        opticalMix.gameObject.SetActive(isSlider);
    }
}

/// <summary>
/// 用于地形修改的鼠标状态
/// </summary>
public class MouseTerrainModifierState : MouseState
{
    private bool isAdd = true;

    /// <summary>
    /// 对应的地形修改面板
    /// </summary>
    private TerrainModifierPanel panel;
    private TerrainModifierPanel Panel
    {
        get
        {
            if (panel == null)
                panel = UIManager.Instance.GetPanel(UIPanelType.TerrainModifier) as TerrainModifierPanel;
            return panel;
        }
    }


    /// <summary>
    /// 投影
    /// </summary>
    private Projector projector;
    private Projector Projection
    {
        get
        {
            if (projector == null)
            {
                GameObject projectorObj = new GameObject("TerrainProjector");
                projectorObj.transform.localEulerAngles = new Vector3(90, 0, 0);
                projector = projectorObj.AddComponent<Projector>();
                projector.orthographic = true;
                projector.material = Resources.Load("Materials/ProjectorMat") as Material;
            }
            return projector;
        }
    }

    /// <summary>
    /// 初始化状态
    /// </summary>
    public override void OnInit()
    {
        Panel.rangeMix.onValueChange.AddListener((a) =>
        {
            Projection.orthographicSize = a;
        });
    }

    /// <summary>
    /// 下拉框状态改变
    /// </summary>
    /// <param name="index"></param>
    public void OnDropDownChange(int index)
    {
        if (index > 4)
        {
            Projection.enabled = false;
        }
        else
        {
            Projection.enabled = true;
        }
    }

    /// <summary>
    /// 状态开始
    /// </summary>
    /// <param name="para"></param>
    /// <param name="args"></param>
    public override void OnActive(object para = null, params object[] args)
    {
        Projection.enabled = true;
    }

    /// <summary>
    /// 左键按住
    /// </summary>
    public override void OnLeftButtonHold()
    {
        hitInfo = Utility.SendRay(LayerMask.GetMask("Terrain"));
        if (hitInfo.Equals(default(RaycastHit)))
            return;

        Terrain terrain = hitInfo.collider.GetComponent<Terrain>();
        switch (Panel.modifierType)
        {
            case TerrainModifierPanel.ModifierType.Up:
                TerrainUtility.ChangeHeight(hitInfo.point, (int)Panel.rangeMix.Value, Panel.opticalMix.Value);
                break;
            case TerrainModifierPanel.ModifierType.Down:
                TerrainUtility.ChangeHeight(hitInfo.point, (int)Panel.rangeMix.Value, Panel.opticalMix.Value, false);
                break;
            case TerrainModifierPanel.ModifierType.Smooth:
                TerrainUtility.Smooth(hitInfo.point, (int)Panel.rangeMix.Value, Panel.opticalMix.Value);
                break;
            case TerrainModifierPanel.ModifierType.AddDetial:
                if (isAdd)
                    TerrainUtility.AddDetial(terrain, hitInfo.point, Panel.rangeMix.Value, (int)(Panel.opticalMix.Value));
                else
                    TerrainUtility.RemoveDetial(terrain, hitInfo.point, Panel.rangeMix.Value);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 不同的状态对应不同的点击事件
    /// </summary>
    public override void OnLeftButtonUp()
    {
        hitInfo = Utility.SendRay(LayerMask.GetMask("Terrain"));
        if (hitInfo.Equals(default(RaycastHit)))
            return;

        Terrain terrain = hitInfo.collider.GetComponent<Terrain>();
        switch (Panel.modifierType)
        {
            case TerrainModifierPanel.ModifierType.AddTree:
                if (isAdd)
                    TerrainUtility.CreatTree(terrain, hitInfo.point, (int)(Panel.opticalMix.Value), (int)Panel.rangeMix.Value);
                else
                    TerrainUtility.RemoveTree(terrain, hitInfo.point, (int)Panel.rangeMix.Value);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 按键操作
    /// </summary>
    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            isAdd = false;

        if (Input.GetKeyUp(KeyCode.LeftShift))
            isAdd = true;


        // 更新投影的位置
        hitInfo = Utility.SendRay(LayerMask.GetMask("Terrain"));
        if (!hitInfo.Equals(default(RaycastHit)))
        {
            Projection.transform.position = hitInfo.point + Vector3.up * 50;
        }
    }

    /// <summary>
    /// 状态结束
    /// </summary>
    public override void OnDisactive()
    {
        Projection.enabled = false;
    }

    /// <summary>
    /// 设置投影的显示
    /// </summary>
    /// <param name="isShow"></param>
    public void SetProjectorShow(bool isShow = false)
    {
        Projection.enabled = isShow;
    }

    public void SetShow(bool isSlider)
    {
    }
}
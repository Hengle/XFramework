#define Unit

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MouseStateType
{
    /// <summary>
    /// 默认空状态
    /// </summary>
    DefaultState,
    /// <summary>
    /// 创建地面单位状态
    /// </summary>
    CreateArmyState,
    /// <summary>
    /// 创建空中单位状态
    /// </summary>
    CreateAirForceState,
    /// <summary>
    /// 坦克火力范围
    /// </summary>
    AttackRangeState,
    /// <summary>
    /// 防空火力范围
    /// </summary>
    AirDefenceState,
    /// <summary>
    /// 炮兵火力打击范围
    /// </summary>
    ArtilleryRangeState,
    /// <summary>
    /// 空中单位火力范围
    /// </summary>
    AirForceRangeState,
    /// <summary>
    /// 地面单位移动
    /// </summary>
    MoveState,
    /// <summary>
    /// 地面单位攻击
    /// </summary>
    AttackState,
    /// <summary>
    /// 空中单位移动
    /// </summary>
    AirMoveState,
    /// <summary>
    /// 空中单位攻击
    /// </summary>
    AirAttackState,
    /// <summary>
    /// 框选
    /// </summary>
    SelectObjs,
    /// <summary>
    /// 地图测距
    /// </summary>
    TerrainRangingState
}

public class MouseState
{
    /// <summary>
    /// 检测射线信息
    /// </summary>
    protected RaycastHit hitInfo;

    /// <summary>
    /// 状态激活时
    /// </summary>
    /// <param name="para"></param>
    public virtual void OnActive(object para = null, params object[] args) { }
    /// <summary>
    /// 状态结束时
    /// </summary>
    public virtual void OnDisactive() { }
    /// <summary>
    /// 左键按下
    /// </summary>
    public virtual void OnLeftButtonDown() { }
    /// <summary>
    /// 左键保持按下状态
    /// </summary>
    public virtual void OnLeftButtonHold() { }
    /// <summary>
    /// 左键抬起
    /// </summary>
    public virtual void OnLeftButtonUp() { }
    /// <summary>
    /// 右键按下
    /// </summary>
    public virtual void OnRightButtonDown() { }
    /// <summary>
    /// 右键抬起
    /// </summary>
    public virtual void OnRightButtonUp() { }
    /// <summary>
    /// 每帧调用
    /// </summary>
    public virtual void Update() { }

    /// <summary>
    /// 发射一次射线更新hitInfo并返回当前鼠标接触到的物体
    /// </summary>
    /// <param name="layer">射线层级</param>
    /// <returns></returns>
    protected GameObject SendRay(int layer)
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, float.MaxValue, layer))
        {
            return hitInfo.collider.gameObject;
        }
        else
        {
            return null;
        }
    }
}

/// <summary>
/// 默认鼠标状态
/// </summary>
public class MouseDefaultState : MouseState { }
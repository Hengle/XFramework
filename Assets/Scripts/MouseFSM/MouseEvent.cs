using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 鼠标事件
/// </summary>
public class MouseEvent : Singleton<MouseEvent>
{
    /// <summary>
    /// 当前鼠标状态
    /// </summary>
    public MouseState CurrentState { get; private set; }
    /// <summary>
    /// 当前模块的默认鼠标状态
    /// </summary>
    private MouseState DefaultState;

    /// <summary>
    /// 鼠标在上一帧的位置
    /// </summary>
    private Vector3 lastPosition;
    /// <summary>
    /// 鼠标是否移动
    /// </summary>
    public bool MouseMove { get; private set; }

    public MouseEvent()
    {
        MonoEvent.Instance.UPDATE += Update;
    }

    void Update()
    {
        //处理鼠标事件 当点击UI面板时不处理
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                CurrentState.OnLeftButtonDown();
            }
            if (Input.GetMouseButton(0))
            {
                CurrentState.OnLeftButtonHold();
            }
            if (Input.GetMouseButton(1))
            {
                CurrentState.OnRightButtonHold();
            }
            if (Input.GetMouseButtonUp(0))
            {
                CurrentState.OnLeftButtonUp();
            }

            if (Input.GetMouseButtonDown(1))
            {
                CurrentState.OnRightButtonDown();
            }
            if (Input.GetMouseButtonUp(1))
            {
                CurrentState.OnRightButtonUp();
            }

            if (Input.GetMouseButtonDown(2))
            {
                //OnMouseRollDown();
            }
        }

        CurrentState.Update();

        if (Input.mousePosition != lastPosition)
        {
            lastPosition = Input.mousePosition;
            MouseMove = true;
        }
        else
        {
            MouseMove = false;
        }
    }

    /// <summary>
    /// 改变当前鼠标状态(带参数: 实体单位)
    /// </summary>
    /// <param name="state"></param>
    /// <param name="para"></param>
    public void ChangeState(MouseState state, object para = null, params object[] args)
    {
        // 状态未改变
        if (CurrentState == state)
        {
            CurrentState.OnEnable(para, args);
            return;
        }

        CurrentState.OnDisable();

        if (!CurrentState.isInited)
            CurrentState.OnInit();
        CurrentState.OnEnable(para, args);
    }
}
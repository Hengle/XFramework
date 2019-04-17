using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 鼠标事件
/// </summary>
public class MouseFsm : Fsm<MouseState>
{
    /// <summary>
    /// 当前鼠标状态
    /// </summary>
    public MouseState currentMouseState { get; private set; }
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

    public MouseFsm()
    {

    }

    public override void OnUpdate()
    {
        //处理鼠标事件 当点击UI面板时不处理
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                currentMouseState.OnLeftButtonDown();
            }
            else if (Input.GetMouseButton(0))
            {
                currentMouseState.OnLeftButtonHold();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                currentMouseState.OnLeftButtonUp();
            }

            else if (Input.GetMouseButtonDown(1))
            {
                currentMouseState.OnRightButtonDown();
            }
            else if (Input.GetMouseButton(1))
            {
                currentMouseState.OnRightButtonHold();
            }
            else if (Input.GetMouseButtonUp(1))
            {
                currentMouseState.OnRightButtonUp();
            }
        }

        currentMouseState.OnUpdate();

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

    public override FsmState ChangeState<T>()
    {
        currentMouseState = (MouseState)base.ChangeState<T>();
        return currentMouseState;
    }
}
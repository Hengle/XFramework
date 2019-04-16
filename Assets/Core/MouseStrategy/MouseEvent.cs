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
    public BaseMouseState CurrentState { get; private set; }
    /// <summary>
    /// 当前模块的默认鼠标状态
    /// </summary>
    private BaseMouseState DefaultState;

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
        CurrentState = new BaseMouseState();
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
            else if (Input.GetMouseButton(0))
            {
                CurrentState.OnLeftButtonHold();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                CurrentState.OnLeftButtonUp();
            }

            else if (Input.GetMouseButtonDown(1))
            {
                CurrentState.OnRightButtonDown();
            }
            else if (Input.GetMouseButton(1))
            {
                CurrentState.OnRightButtonHold();
            }
            else if (Input.GetMouseButtonUp(1))
            {
                CurrentState.OnRightButtonUp();
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
    /// 改变当前鼠标状态
    /// </summary>
    /// <param name="state"></param>
    public void ChangeState(BaseMouseState state, params object[] args)
    {
        // 状态未改变
        if (CurrentState == state)
        {
            CurrentState.OnEnable();
            return;
        }

        CurrentState.OnDisable();
        CurrentState = state;

        if (!CurrentState.isInited)
            CurrentState.OnInit();
        CurrentState.OnEnable();
    }

    /// <summary>
    /// 激活/注销
    /// </summary>
    public void SetActive(bool isActive)
    {
        if (isActive)
        {
            MonoEvent.Instance.UPDATE -= Update;
            MonoEvent.Instance.UPDATE += Update;
        }
        else
            MonoEvent.Instance.UPDATE -= Update;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseEvent : MonoSingleton<MouseEvent>
{
    /// <summary>
    /// 当前鼠标状态
    /// </summary>
    public MouseState CurrentState { get; private set; } 

    /// <summary>
    /// 存储状态枚举和状态类对应关系的字典
    /// </summary>
    private Dictionary<MouseStateType, MouseState> stateDic;

    public MouseStateType CurrentStateType { get; private set; }


    public MouseEvent()
    {
        InitDic();
        CurrentState = stateDic[MouseStateType.DefaultState];
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
    }

    /// <summary>
    /// 改变当前鼠标状态(带参数: 实体单位)
    /// </summary>
    /// <param name="_type"></param>
    /// <param name="para"></param>
    public void ChangeState(MouseStateType _type, object para = null)
    {
        MouseState nextState = null;
        CurrentStateType = _type;

        switch (_type)
        {
            case MouseStateType.DefaultState:
                nextState = stateDic[MouseStateType.DefaultState];
                break;
            case MouseStateType.AttackRangeState:
                nextState = stateDic[MouseStateType.AttackRangeState];
                break;
            case MouseStateType.AirDefenceState:
                nextState = stateDic[MouseStateType.AirDefenceState];
                break;
            case MouseStateType.ArtilleryRangeState:
                nextState = stateDic[MouseStateType.ArtilleryRangeState];
                break;
            case MouseStateType.CreateArmyState:
                nextState = stateDic[MouseStateType.CreateArmyState];
                break;
            case MouseStateType.MoveState:
                nextState = stateDic[MouseStateType.MoveState];
                break;
            case MouseStateType.AttackState:
                nextState = stateDic[MouseStateType.AttackState];
                break;
            default: return;
        }

        CurrentState.OnDisactive();  
        CurrentState = nextState;          // 更新状态
        CurrentState.OnActive(para);
    }

    /// <summary>
    /// 初始化字典
    /// </summary>
    private void InitDic()
    {
        stateDic = new Dictionary<MouseStateType, MouseState>
        {
            { MouseStateType.DefaultState, new MouseDefaultState() },
            { MouseStateType.AirDefenceState, new MouseAirDefenceState() },
            { MouseStateType.ArtilleryRangeState, new MouseArtilleryRangeState() },
            { MouseStateType.AttackRangeState, new MouseAttackRangeState() },
            { MouseStateType.CreateArmyState, new MouseCreateArmyState() },
            { MouseStateType.AttackState, new MouseAttackState() },
            { MouseStateType.MoveState, new MouseMoveState() }
        };
    }
}


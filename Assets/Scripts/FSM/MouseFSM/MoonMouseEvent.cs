using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

/// <summary>
/// 继承Mono单例 挂在场景中 控制鼠标事件状态
/// </summary>
public class MoonMouseEvent : MonoSingleton<MoonMouseEvent>
{

    /// <summary>
    /// 当前鼠标状态
    /// </summary>
    private MouseState currentState = new MouseState_Defalt();
    /// <summary>
    /// 当前状态对于的枚举
    /// </summary>
    private MouseStateType currentStateType = MouseStateType.Defalt;
    /// <summary>
    /// 存储状态和枚举对应关系的字典
    /// </summary>
    private Dictionary<MouseStateType, MouseState> mouseStateDic;

    public MoonMouseEvent()
    {
        singletonType = SingletonType.GlobalInstance;
        InitDic();
    }

    // Use this for initialization
    void Start ()
    {
        //InitDic();
    }    

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            //处理鼠标事件
            if (Input.GetMouseButtonDown(0))
            {
                currentState.OnLeftButtonDown();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                currentState.OnLeftButtonUp();
            }

            if (Input.GetMouseButtonDown(1))
            {
                currentState.OnRightButtonDown();
            }

            if (Input.GetMouseButtonUp(1))
            {
                currentState.OnRightButtonUp();
            }

            if (Input.GetMouseButtonDown(2))
            {
                //OnMouseRollDown();
            }
        }

        currentState.Update();
    }

    /// <summary>
    /// 改变当前鼠标状态
    /// </summary>
    /// <param name="_type">目标状态</param>
    /// <param name="para">可以是场景中的实体单位</param>
    public void ChangeState(MouseStateType _type, object para = null)
    {
        currentStateType = _type;
        currentState.OnDisactive();
        currentState = mouseStateDic[currentStateType];
        currentState.OnActive();
    }

    /// <summary>
    /// 初始化状态字典
    /// </summary>
    private void InitDic()
    {
        mouseStateDic = new Dictionary<MouseStateType, MouseState>();
        mouseStateDic.Add(MouseStateType.Defalt, new MouseState_Defalt());
        mouseStateDic.Add(MouseStateType._A, new MouseState_A());
        mouseStateDic.Add(MouseStateType._B, new MouseState_B());
    }
}
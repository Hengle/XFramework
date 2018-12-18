using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RCXC
{
    public class MouseEvent : Singleton<MouseEvent>
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
        public void ChangeState(MouseStateType _type, object para = null, params object[] args)
        {
            // 状态未改变
            if (CurrentStateType == _type)
            {
                CurrentState.OnActive(para, args);
                return;
            }

            CurrentState.OnDisactive();
            CurrentState = stateDic[_type];          // 更新状态
            CurrentState.OnActive(para, args);
            CurrentStateType = _type;                // 赋值当前状态类型
        }

        /// <summary>
        /// 初始化字典
        /// </summary>
        private void InitDic()
        {
            stateDic = new Dictionary<MouseStateType, MouseState>
            {
                { MouseStateType.DefaultState, new MouseDefaultState() },
            };
        }
    }
}


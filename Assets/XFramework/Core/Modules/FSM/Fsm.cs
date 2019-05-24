using System;
using System.Collections.Generic;

namespace XFramework
{
    /// <summary>
    /// 状态机
    /// </summary>
    /// <typeparam name="TState">子类状态机对应的状态基类</typeparam>
    public class Fsm<TState> : FsmBase where TState : FsmState
    {
        public Fsm()
        {
            stateDic = new Dictionary<string, FsmState>();
            IsActive = false;
        }

        internal override void OnUpdate()
        {
            if (CurrentState != null)
            {
                CurrentState.OnUpdate();
            }
        }

        /// <summary>
        /// 获取一个状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected override FsmState GetState<T>()
        {
            return GetState(typeof(T));
        }

        protected override FsmState GetState(Type type)
        {
            if (stateDic.ContainsKey(type.Name))
            {
                return stateDic[type.Name];
            }
            else
            {
                FsmState tempstate = CreateState(type);
                stateDic.Add(type.Name, tempstate);
                return tempstate;
            }
        }

        /// <summary>
        /// 创建一个状态
        /// </summary>
        protected override FsmState CreateState<T>()
        {
            return CreateState(typeof(T));
        }

        protected override FsmState CreateState(Type type)
        {
            FsmState state = Utility.Reflection.CreateInstance<FsmState>(type);

            if (!(state is TState))
                throw new System.Exception("状态类型设置错误");

            state.Init();
            return state;
        }

        /// <summary>
        /// 获取当前状态
        /// </summary>
        /// <returns></returns>
        public FsmState GetCurrentState()
        {
            return CurrentState;
        }

        public T GetCurrentState<T>() where T : TState
        {
            return CurrentState as T;
        }

        /// <summary>
        /// 开启状态机
        /// </summary>
        /// <typeparam name="KState"></typeparam>
        public override void StartFsm<KState>()
        {
            StartFsm(typeof(KState));
        }

        public override void StartFsm(Type type)
        {
            if (!IsActive)
                GetState(type).OnEnter();
            IsActive = true;
        }

        /// <summary>
        /// 状态切换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public override void ChangeState<T>()
        {
            ChangeState(typeof(T));
        }

        public override void ChangeState(Type type)
        {
            if (IsActive)
            {
                FsmState tempstate = GetState(type);

                if (CurrentState != tempstate)
                {
                    CurrentState?.OnExit();
                    CurrentState = tempstate;
                    CurrentState.OnEnter();
                }
            }
        }
    }
}
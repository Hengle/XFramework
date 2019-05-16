using System.Collections.Generic;

namespace XDEDZL
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
            if (stateDic.ContainsKey(typeof(T).Name))
            {
                return stateDic[typeof(T).Name];
            }
            else
            {
                FsmState tempstate = CreateState<T>();
                stateDic.Add(typeof(T).Name, tempstate);
                return tempstate;
            }
        }

        /// <summary>
        /// 创建一个状态
        /// </summary>
        protected override FsmState CreateState<T>()
        {
            FsmState state = Utility.Reflection.CreateInstance<T>();

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
            if (!IsActive)
                GetState<KState>().OnEnter();
            IsActive = true;
        }

        /// <summary>
        /// 状态切换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public override void ChangeState<T>()
        {
            if (IsActive)
            {
                FsmState tempstate = GetState<T>();

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
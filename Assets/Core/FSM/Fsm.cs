using System.Collections.Generic;
using XDEDZL.Utility;

/// <summary>
/// 状态机
/// </summary>
/// <typeparam name="TState">子类状态机对应的状态基类</typeparam>
public class Fsm<TState> : FsmBase where TState : FsmState
{
    public Fsm()
    {
        stateDic = new Dictionary<string, FsmState>();
        IsActive = true;
    }

    /// <summary>
    /// 状态切换
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public override FsmState ChangeState<T>()
    {
        FsmState tempstate = GetState<T>();

        if (currentState != tempstate)
        {
            currentState?.OnExit();
            currentState = tempstate;
            currentState.OnEnter();
        }
        return currentState;
    }

    /// <summary>
    /// 创建对应状态
    /// </summary>
    protected override FsmState CreateState<T>()
    {
        FsmState state = ReflectionUtility.CreateInstance<T>();

        if (!(state is TState))
            throw new System.Exception("状态类型设置错误");

        state.Init();
        return state;
    }
}
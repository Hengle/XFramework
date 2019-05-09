using System.Collections.Generic;

/// <summary>
/// 状态机不要基础这个类，请继承Fsm<TState>
/// 这个类是为了解决泛型类Fsm<TState>在FsmManager中不好管理
/// </summary>
public abstract class FsmBase
{
    /// <summary>
    /// 存储该状态机包含的所有状态
    /// </summary>
    protected Dictionary<string, FsmState> stateDic;
    /// <summary>
    /// 状态机当前状态
    /// </summary>
    protected FsmState currentState;
    /// <summary>
    /// 状态机是否激活
    /// </summary>
    public virtual bool IsActive { get; protected set; }

    /// <summary>
    /// 创建一个状态
    /// </summary>
    protected abstract FsmState CreateState<T>() where T : FsmState;

    /// <summary>
    /// 状态切换
    /// </summary>
    public abstract FsmState ChangeState<T>() where T : FsmState;

    /// <summary>
    /// 获取一个状态
    /// </summary>
    public FsmState GetState<T>() where T : FsmState
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
    /// 没帧调用
    /// </summary>
    public virtual void OnUpdate()
    {
        if (currentState != null)
        {
            currentState.OnUpdate();
        }
    }
}
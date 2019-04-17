using System.Collections.Generic;

/// <summary>
/// 状态机基类
/// 状态机不要基础这个类，请继承Fsm<TState>
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
    /// 状态切换
    /// </summary>
    public abstract FsmState ChangeState<T>() where T : FsmState;

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
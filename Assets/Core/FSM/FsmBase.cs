using System.Collections.Generic;

/// <summary>
/// 状态机不要基础这个类，请继承Fsm<TState>
/// 这个类是为了解决泛型类Fsm<TState>在FsmManager中不好管理,定义数据和方法
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
    public FsmState CurrentState { get; protected set; }
    /// <summary>
    /// 状态机是否激活
    /// </summary>
    public bool IsActive { get; protected set; }

    /// <summary>
    /// 每帧调用
    /// </summary>
    internal abstract void OnUpdate();

    /// <summary>
    /// 创建一个状态
    /// </summary>
    protected abstract FsmState CreateState<T>() where T : FsmState;

    /// <summary>
    /// 从某一状态开始一个状态机
    /// </summary>
    /// <typeparam name="T"></typeparam>

    /// <summary>
    /// 获取一个状态
    /// </summary>
    protected abstract FsmState GetState<T>() where T : FsmState;

    public abstract void StartFsm<TState>() where TState : FsmState;

    /// <summary>
    /// 状态切换
    /// </summary>
    public abstract void ChangeState<T>() where T : FsmState;
}
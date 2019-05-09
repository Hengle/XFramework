using System;
using System.Collections.Generic;
using XDEDZL.Utility;

/// <summary>
/// 状态机管理类
/// </summary>
public class FsmManager : Singleton<FsmManager>
{
    /// <summary>
    /// 存储所有状态机的字典
    /// </summary>
    private readonly Dictionary<string, FsmBase> fsmDic;

    public FsmManager()
    {
        fsmDic = new Dictionary<string, FsmBase>();
        MonoEvent.Instance.UPDATE += OnUpdate;     //TODO 日后删除MonoEvent，这里交给游戏系统管理
    }

    /// <summary>
    /// 每帧调用处于激活状态的状态机
    /// </summary>
    public void OnUpdate()
    {
        foreach (var fsm in fsmDic.Values)
        {
            if (fsm.IsActive)
                fsm.OnUpdate();
        }
    }

    /// <summary>
    /// 状态机的数量
    /// </summary>
    public int Count
    {
        get
        {
            return fsmDic.Count;
        }
    }

    /// <summary>
    /// 是否包含某种状态机
    /// </summary>
    public bool HasFsm<T>() where T : FsmBase
    {
        return HasFsm(typeof(T));
    }

    /// <summary>
    /// 是否包含某种状态机
    /// </summary>
    public bool HasFsm(Type type)
    {
        return fsmDic.ContainsKey(type.Name);
    }

    /// <summary>
    /// 切换对应状态机到对应状态
    /// </summary>
    /// <typeparam name="TFsm">状态机类型</typeparam>
    /// <typeparam name="KState">目标状态</typeparam>
    public void ChangeState<TFsm,KState>() where TFsm : FsmBase where KState : FsmState
    {
        if (!HasFsm<TFsm>())
        {
            CreateFsm<TFsm>();
        }
        fsmDic[typeof(TFsm).Name].ChangeState<KState>();
    }

    public void ChanegState(Type typeFsm,Type typeState)
    {
        if(!typeFsm.IsSubclassOf(typeof(FsmBase)) || !typeState.IsSubclassOf(typeof(FsmState)))
        {
            throw new System.Exception("类型传入错误");
        }

        if (!HasFsm(typeFsm))
        {

        }
    }

    /// <summary>
    /// 根据类型创建一个状态机
    /// </summary>
    public void CreateFsm<T>() where T : FsmBase
    {
        fsmDic.Add(typeof(T).Name, ReflectionUtility.CreateInstance<T>());
    }
}
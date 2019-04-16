using System;
using System.Collections;
using System.Collections.Generic;
using XDEDZL.Utility;

public class FsmManager : Singleton<FsmManager>
{
    private readonly Dictionary<string, FsmBase<FsmState>> fsmDic;

    public FsmManager()
    {
        fsmDic = new Dictionary<string, FsmBase<FsmState>>();
        MonoEvent.Instance.UPDATE += OnUpdate;
    }

    public void OnUpdate()
    {
        foreach (var fsm in fsmDic.Values)
        {
            if (fsm.IsActive)
                fsm.OnUpdate();
        }
    }

    public int Count
    {
        get
        {
            return fsmDic.Count;
        }
    }

    public void CreateFsm<T>() where T : FsmBase<FsmState>
    {
        fsmDic.Add(typeof(T).Name, ReflectionUtility.CreateInstance<T>());
    }

    public bool HasFsm<T>() where T : FsmBase<FsmState>
    {
        return fsmDic.ContainsKey(typeof(T).Name);
    }

    public bool HasFsm(Type type)
    {
        return fsmDic.ContainsKey(type.Name);
    }

    public void ChangeState<TFsm,KState>() where TFsm : FsmBase<FsmState> where KState : FsmState
    {
        if (!HasFsm<TFsm>())
        {
            CreateFsm<TFsm>();
        }
        fsmDic[typeof(TFsm).Name].ChangeState<KState>();
    }
}